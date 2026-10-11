using System;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Windows.Foundation;
using VirtualKey = Windows.System.VirtualKey;

namespace ShadowViewer.Controls;

/// <summary>透明说明按钮：悬浮临时展开，点击或键盘激活后保持展开，支持任意说明内容。</summary>
public sealed partial class HelpFlyoutButton : UserControl
{
    public static readonly DependencyProperty FlyoutContentProperty = DependencyProperty.Register(
        nameof(FlyoutContent), typeof(object), typeof(HelpFlyoutButton), new PropertyMetadata(null));

    /// <summary>弹出内容，可传入文本或任意 UIElement。</summary>
    public object? FlyoutContent
    {
        get => GetValue(FlyoutContentProperty);
        set => SetValue(FlyoutContentProperty, value);
    }

    public static readonly DependencyProperty FlyoutContentTemplateProperty = DependencyProperty.Register(
        nameof(FlyoutContentTemplate), typeof(DataTemplate), typeof(HelpFlyoutButton), new PropertyMetadata(null));

    /// <summary>内容为数据对象时使用的模板。</summary>
    public DataTemplate? FlyoutContentTemplate
    {
        get => (DataTemplate?)GetValue(FlyoutContentTemplateProperty);
        set => SetValue(FlyoutContentTemplateProperty, value);
    }

    public static readonly DependencyProperty AccessibleNameProperty = DependencyProperty.Register(
        nameof(AccessibleName), typeof(string), typeof(HelpFlyoutButton), new PropertyMetadata("Help"));

    /// <summary>按钮的屏幕阅读器名称，由使用方提供本地化文本。</summary>
    public string AccessibleName
    {
        get => (string)GetValue(AccessibleNameProperty);
        set => SetValue(AccessibleNameProperty, value);
    }

    public static readonly DependencyProperty PlacementProperty = DependencyProperty.Register(
        nameof(Placement), typeof(FlyoutPlacementMode), typeof(HelpFlyoutButton),
        new PropertyMetadata(FlyoutPlacementMode.Top));

    public FlyoutPlacementMode Placement
    {
        get => (FlyoutPlacementMode)GetValue(PlacementProperty);
        set => SetValue(PlacementProperty, value);
    }

    private readonly DispatcherQueueTimer closeTimer;
    private const double PageInset = 20;
    private const double ButtonGap = 8;
    private FlyoutPresenter? presenter;
    private Popup? popup;
    private Rect lastPageBounds;
    private Rect lastButtonBounds;
    private Size lastPresenterSize;
    private double defaultMaxWidth;
    private double defaultMaxHeight;
    private bool updatingPlacement;
    private bool placementUpdateQueued;
    private bool isOpen;
    private bool isPinned;
    private bool promoteAfterClose;
    private bool pointerOverButton;
    private bool pointerOverFlyout;

    public HelpFlyoutButton()
    {
        InitializeComponent();
        closeTimer = DispatcherQueue.CreateTimer();
        closeTimer.Interval = TimeSpan.FromMilliseconds(180);
        closeTimer.IsRepeating = false;
        closeTimer.Tick += (_, _) =>
        {
            if (!isPinned && !pointerOverButton && !pointerOverFlyout) HelpFlyout.Hide();
        };
        // 悬浮弹出后，原按钮仍能收到点击，从临时说明升级为保持展开。
        HelpFlyout.OverlayInputPassThroughElement = HelpButton;
    }

    private void OnButtonPointerEntered(object sender, PointerRoutedEventArgs e)
    {
        if (e.Pointer.PointerDeviceType != PointerDeviceType.Mouse) return;
        pointerOverButton = true;
        closeTimer.Stop();
        if (!isOpen && !promoteAfterClose) Show(pinned: false);
    }

    private void OnButtonPointerExited(object sender, PointerRoutedEventArgs e)
    {
        pointerOverButton = false;
        ScheduleClose();
    }

    private void OnClick(object sender, RoutedEventArgs e)
    {
        if (!IsLoaded || FlyoutContent == null) return;
        closeTimer.Stop();
        if (isPinned) return;
        isPinned = true;
        if (isOpen)
        {
            // ShowMode 在打开时决定焦点与轻触关闭行为，重新打开才能完整切换成 Standard。
            promoteAfterClose = true;
            HelpFlyout.Hide();
        }
        else Show(pinned: true);
    }

    private void Show(bool pinned)
    {
        if (!IsLoaded || !IsEnabled || FlyoutContent == null) return;
        isPinned = pinned;
        isOpen = true;
        HelpFlyout.AllowFocusOnInteraction = pinned;
        HelpFlyout.ShowAt(HelpButton, new FlyoutShowOptions
        {
            Placement = Placement,
            ExclusionRect = new Rect(-ButtonGap, -ButtonGap,
                HelpButton.ActualWidth + ButtonGap * 2, HelpButton.ActualHeight + ButtonGap * 2),
            ShowMode = pinned ? FlyoutShowMode.Standard : FlyoutShowMode.Transient
        });
    }

    private void OnFlyoutOpened(object sender, object e)
    {
        // 监听整个 presenter（含内边距），在按钮与说明面板之间移动时保留短暂宽限。
        DependencyObject? parent = HelpContentPresenter;
        while (parent != null && parent is not FlyoutPresenter) parent = VisualTreeHelper.GetParent(parent);
        presenter = parent as FlyoutPresenter;
        if (presenter == null) return;
        presenter.PointerEntered += OnFlyoutPointerEntered;
        presenter.PointerExited += OnFlyoutPointerExited;
        foreach (Popup openPopup in VisualTreeHelper.GetOpenPopupsForXamlRoot(XamlRoot))
        {
            if (openPopup.Child != presenter) continue;
            popup = openPopup;
            break;
        }
        defaultMaxWidth = presenter.MaxWidth;
        defaultMaxHeight = presenter.MaxHeight;
        lastPageBounds = Rect.Empty;
        lastButtonBounds = Rect.Empty;
        LayoutUpdated += OnLayoutUpdated;
        UpdatePlacement();
    }

    private void OnLayoutUpdated(object? sender, object e)
    {
        if (placementUpdateQueued) return;
        // 等本轮布局完成，TransformToVisual 才包含父容器滚动或移动后的坐标。
        placementUpdateQueued = DispatcherQueue.TryEnqueue(DispatcherQueuePriority.Low, () =>
        {
            placementUpdateQueued = false;
            UpdatePlacement();
        });
    }

    private void UpdatePlacement()
    {
        if (updatingPlacement || presenter == null || popup == null || XamlRoot == null) return;
        UIElement root = XamlRoot.Content;
        Rect pageBounds = new(0, 0, XamlRoot.Size.Width, XamlRoot.Size.Height);
        // 使用最近的 Page，并与窗口相交，避免弹层进入导航栏或窗口外。
        for (DependencyObject? parent = this; parent != null; parent = VisualTreeHelper.GetParent(parent))
        {
            if (parent is not Page page) continue;
            pageBounds.Intersect(page.TransformToVisual(root)
                .TransformBounds(new Rect(0, 0, page.ActualWidth, page.ActualHeight)));
            break;
        }
        Rect buttonBounds = HelpButton.TransformToVisual(root)
            .TransformBounds(new Rect(0, 0, HelpButton.ActualWidth, HelpButton.ActualHeight));
        if (pageBounds == lastPageBounds && buttonBounds == lastButtonBounds
            && presenter.ActualWidth == lastPresenterSize.Width
            && presenter.ActualHeight == lastPresenterSize.Height) return;
        lastPageBounds = pageBounds;
        lastButtonBounds = buttonBounds;
        if (pageBounds.Width <= PageInset * 2 || pageBounds.Height <= PageInset * 2)
        {
            HelpFlyout.Hide();
            return;
        }

        updatingPlacement = true;
        try
        {
            Rect safeBounds = new(pageBounds.X + PageInset, pageBounds.Y + PageInset,
                pageBounds.Width - PageInset * 2, pageBounds.Height - PageInset * 2);
            presenter.MaxWidth = Math.Min(defaultMaxWidth, safeBounds.Width);
            presenter.MaxHeight = Math.Min(defaultMaxHeight, safeBounds.Height);
            presenter.UpdateLayout();
            double desiredWidth = presenter.ActualWidth;
            double desiredHeight = presenter.ActualHeight;

            double top = Math.Clamp(buttonBounds.Top - ButtonGap, safeBounds.Top, safeBounds.Bottom);
            double bottom = Math.Clamp(buttonBounds.Bottom + ButtonGap, safeBounds.Top, safeBounds.Bottom);
            double left = Math.Clamp(buttonBounds.Left - ButtonGap, safeBounds.Left, safeBounds.Right);
            double right = Math.Clamp(buttonBounds.Right + ButtonGap, safeBounds.Left, safeBounds.Right);
            Rect[] regions =
            [
                new(safeBounds.Left, safeBounds.Top, safeBounds.Width, top - safeBounds.Top),
                new(safeBounds.Left, bottom, safeBounds.Width, safeBounds.Bottom - bottom),
                new(safeBounds.Left, safeBounds.Top, left - safeBounds.Left, safeBounds.Height),
                new(right, safeBounds.Top, safeBounds.Right - right, safeBounds.Height)
            ];
            int preferred = Placement switch
            {
                FlyoutPlacementMode.Bottom or FlyoutPlacementMode.BottomEdgeAlignedLeft
                    or FlyoutPlacementMode.BottomEdgeAlignedRight => 1,
                FlyoutPlacementMode.Left or FlyoutPlacementMode.LeftEdgeAlignedTop
                    or FlyoutPlacementMode.LeftEdgeAlignedBottom => 2,
                FlyoutPlacementMode.Right or FlyoutPlacementMode.RightEdgeAlignedTop
                    or FlyoutPlacementMode.RightEdgeAlignedBottom => 3,
                _ => 0
            };
            int selected = preferred;
            double bestArea = -1;
            // 优先指定方向及其反方向；都放不下时选能显示最多内容的一侧。
            foreach (int side in new[] { preferred, preferred ^ 1, (preferred + 2) % 4, ((preferred + 2) % 4) ^ 1 })
            {
                Rect candidate = regions[side];
                if (candidate.Width >= desiredWidth && candidate.Height >= desiredHeight)
                {
                    selected = side;
                    break;
                }
                double area = Math.Min(candidate.Width, desiredWidth) * Math.Min(candidate.Height, desiredHeight);
                if (area <= bestArea) continue;
                bestArea = area;
                selected = side;
            }

            Rect region = regions[selected];
            if (region.Width <= 0 || region.Height <= 0)
            {
                HelpFlyout.Hide();
                return;
            }
            presenter.MaxWidth = Math.Min(defaultMaxWidth, region.Width);
            presenter.MaxHeight = Math.Min(defaultMaxHeight, region.Height);
            presenter.UpdateLayout();
            double width = presenter.ActualWidth;
            double height = presenter.ActualHeight;
            double x = selected switch
            {
                2 => region.Right - width,
                3 => region.Left,
                _ => buttonBounds.Left + (buttonBounds.Width - width) / 2
            };
            double y = selected switch
            {
                0 => region.Bottom - height,
                1 => region.Top,
                _ => buttonBounds.Top + (buttonBounds.Height - height) / 2
            };
            x = Math.Clamp(x, region.Left, Math.Max(region.Left, region.Right - width));
            y = Math.Clamp(y, region.Top, Math.Max(region.Top, region.Bottom - height));
            Point current = presenter.TransformToVisual(root).TransformPoint(new Point());
            popup.HorizontalOffset += x - current.X;
            popup.VerticalOffset += y - current.Y;
            lastPresenterSize = new Size(width, height);
        }
        finally
        {
            updatingPlacement = false;
        }
    }

    private void OnFlyoutPointerEntered(object sender, PointerRoutedEventArgs e)
    {
        pointerOverFlyout = true;
        closeTimer.Stop();
    }

    private void OnFlyoutPointerExited(object sender, PointerRoutedEventArgs e)
    {
        pointerOverFlyout = false;
        ScheduleClose();
    }

    private void ScheduleClose()
    {
        if (isOpen && !isPinned && !pointerOverButton && !pointerOverFlyout) closeTimer.Start();
    }

    private void OnFlyoutClosed(object sender, object e)
    {
        closeTimer.Stop();
        LayoutUpdated -= OnLayoutUpdated;
        popup = null;
        if (presenter != null)
        {
            presenter.MaxWidth = defaultMaxWidth;
            presenter.MaxHeight = defaultMaxHeight;
            presenter.PointerEntered -= OnFlyoutPointerEntered;
            presenter.PointerExited -= OnFlyoutPointerExited;
            presenter = null;
        }
        isOpen = false;
        pointerOverFlyout = false;
        bool reopen = promoteAfterClose;
        promoteAfterClose = false;
        isPinned = false;
        if (reopen) Show(pinned: true);
    }

    private void OnKeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key != VirtualKey.Escape || !isOpen) return;
        promoteAfterClose = false;
        HelpFlyout.Hide();
        e.Handled = true;
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        closeTimer.Stop();
        promoteAfterClose = false;
        pointerOverButton = false;
        HelpFlyout.Hide();
    }
}
