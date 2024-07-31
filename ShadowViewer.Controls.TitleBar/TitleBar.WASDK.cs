// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Windows.Graphics;
using Windows.UI;
using CommunityToolkit.WinUI.Helpers;
using Microsoft.UI;
using Microsoft.UI.Input;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Media;
using ShadowViewer.Controls;

namespace ShadowViewer.Controls;

[TemplatePart(Name = nameof(PART_FooterPresenter), Type = typeof(ContentPresenter))]
[TemplatePart(Name = nameof(PART_ContentPresenter), Type = typeof(ContentPresenter))]

public partial class TitleBar
{
    /// <summary>
    /// 主题变更事件
    /// </summary>
    public event EventHandler? ThemeChangedEvent;

    public void InvokeThemeChanged(object sender)
    {
        ThemeChangedEvent?.Invoke(sender,EventArgs.Empty);
    }

    WndProcHelper? WndProcHelper;
    MenuFlyout? MenuFlyout;
    ContentPresenter? PART_ContentPresenter;
    ContentPresenter? PART_FooterPresenter;

    private void SetWASDKTitleBar()
    {
        if (this.Window == null)
        {
            return;
            // TO DO: Throw exception that window has not been set? 
        }

        if (!AutoConfigureCustomTitleBar) return;
        Window.AppWindow.TitleBar.ExtendsContentIntoTitleBar = true;

        if (this.ContextFlyout != null && this.ContextFlyout is MenuFlyout menuFlyout)
        {
            this.MenuFlyout = menuFlyout;
            WndProcHelper = new WndProcHelper(this.Window);
            WndProcHelper.RegisterWndProc(WindowWndProc);
            WndProcHelper.RegisterInputNonClientPointerSourceWndProc(InputNonClientPointerSourceWndProc);
        }

        this.Window.SizeChanged -= Window_SizeChanged;
        this.Window.SizeChanged += Window_SizeChanged;
        this.Window.Activated -= Window_Activated;
        this.Window.Activated += Window_Activated;

        if (Window.Content is FrameworkElement rootElement)
        {
            UpdateCaptionButtons(rootElement);
            rootElement.ActualThemeChanged += (s, e) =>
            {
                UpdateCaptionButtons(rootElement);
            };
        }

        ThemeChangedEvent -= TitleBar_ThemeChangedEvent;
        ThemeChangedEvent += TitleBar_ThemeChangedEvent;
        PART_ContentPresenter = GetTemplateChild(nameof(PART_ContentPresenter)) as ContentPresenter;
        PART_FooterPresenter = GetTemplateChild(nameof(PART_FooterPresenter)) as ContentPresenter;

        // Get caption button occlusion information.
        var CaptionButtonOcclusionWidthRight = Window.AppWindow.TitleBar.RightInset;
        var CaptionButtonOcclusionWidthLeft = Window.AppWindow.TitleBar.LeftInset;
        PART_LeftPaddingColumn!.Width = new GridLength(CaptionButtonOcclusionWidthLeft);
        PART_RightPaddingColumn!.Width = new GridLength(CaptionButtonOcclusionWidthRight);

        // Choose a tall title bar to provide more room for interactive elements 
        // like search box or person picture controls.
        Window.AppWindow.TitleBar.PreferredHeightOption = DisplayMode == DisplayMode.Tall ? TitleBarHeightOption.Tall : TitleBarHeightOption.Standard;
        // Recalculate the drag region for the custom title bar 
        // if you explicitly defined new draggable areas.
        SetDragRegionForCustomTitleBar();

        _isAutoConfigCompleted = true;
    }

    private void TitleBar_ThemeChangedEvent(object? sender, EventArgs e)
    {
        if (Window.Content is FrameworkElement frameworkElement)
        {
            UpdateCaptionButtons(frameworkElement);
        }
    }

    private void Window_SizeChanged(object sender, WindowSizeChangedEventArgs args)
    {
        UpdateVisualStateAndDragRegion(args.Size);
    }

    private void UpdateCaptionButtons(FrameworkElement rootElement)
    {
        Window.AppWindow.TitleBar.ButtonBackgroundColor = Colors.Transparent;
        Window.AppWindow.TitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
        Window.AppWindow.TitleBar.ButtonInactiveForegroundColor = Colors.DarkGray;
        Window.AppWindow.TitleBar.ButtonHoverBackgroundColor = rootElement.RequestedTheme switch
        {
            ElementTheme.Dark => "#2D2D2D".ToColor(),
            ElementTheme.Light => "#E9E9E9".ToColor(),
            _ => null
        };
        Window.AppWindow.TitleBar.ButtonHoverForegroundColor = rootElement.RequestedTheme switch
        {
            ElementTheme.Dark => Colors.White,
            ElementTheme.Light => Colors.Black,
            _ => null
        };
        Window.AppWindow.TitleBar.ButtonForegroundColor = Window.AppWindow.TitleBar.ButtonHoverForegroundColor;
    }

    private void ResetWASDKTitleBar()
    {
        if (this.Window == null)
        {
            return;
            // TO DO: Throw exception that window has not been set? 
        }

        // Only reset if we were the ones who configured
        if (!_isAutoConfigCompleted) return;
        Window.AppWindow.TitleBar.ExtendsContentIntoTitleBar = false;
        this.Window.SizeChanged -= Window_SizeChanged;
        this.Window.Activated -= Window_Activated;
        SizeChanged -= this.TitleBar_SizeChanged;
        Window.AppWindow.TitleBar.ResetToDefault();
    }

    private void Window_Activated(object sender, WindowActivatedEventArgs args)
    {
        VisualStateManager.GoToState(this,
            args.WindowActivationState == WindowActivationState.Deactivated
                ? WindowDeactivatedState
                : WindowActivatedState, true);
    }

    public void SetDragRegionForCustomTitleBar()
    {
        if (!AutoConfigureCustomTitleBar || Window is null) return;
        ClearDragRegions(NonClientRegionKind.Passthrough);
        var items = new FrameworkElement?[] { PART_ContentPresenter, PART_FooterPresenter, PART_ButtonHolder };
        var validItems = items.Where(x => x is not null).Select(x => x!).ToArray(); // Prune null items

        SetDragRegion(NonClientRegionKind.Passthrough, validItems);
    }

    public double GetRasterizationScaleForElement(UIElement element)
    {
        return element.XamlRoot != null ? element.XamlRoot.RasterizationScale : 0.0;
    }

    public void SetDragRegion(NonClientRegionKind nonClientRegionKind, params FrameworkElement[] frameworkElements)
    {
        var nonClientInputSrc = InputNonClientPointerSource.GetForWindowId(Window.AppWindow.Id);
        List<Windows.Graphics.RectInt32> rects = new List<Windows.Graphics.RectInt32>();
        var scale = GetRasterizationScaleForElement(this);

        foreach (var frameworkElement in frameworkElements)
        {
            if (frameworkElement == null)
            {
                continue;
            }
            GeneralTransform transformElement = frameworkElement.TransformToVisual(null);
            Windows.Foundation.Rect bounds = transformElement.TransformBounds(new Windows.Foundation.Rect(0, 0, frameworkElement.ActualWidth, frameworkElement.ActualHeight));
            var transparentRect = new Windows.Graphics.RectInt32(
                _X: (int)Math.Round(bounds.X * scale),
                _Y: (int)Math.Round(bounds.Y * scale),
                _Width: (int)Math.Round(bounds.Width * scale),
                _Height: (int)Math.Round(bounds.Height * scale)
            );
            rects.Add(transparentRect);
        }
        if (rects.Count > 0)
        {
            nonClientInputSrc.SetRegionRects(nonClientRegionKind, rects.ToArray());
        }
    }

    public void ClearDragRegions(NonClientRegionKind nonClientRegionKind)
    {
        var noninputsrc = InputNonClientPointerSource.GetForWindowId(Window.AppWindow.Id);
        noninputsrc.ClearRegionRects(nonClientRegionKind);
    }

    private IntPtr InputNonClientPointerSourceWndProc(IntPtr hWnd, NativeMethods.WindowMessage Msg, IntPtr wParam, IntPtr lParam)
    {
        switch (Msg)
        {
            case NativeMethods.WindowMessage.WM_NCLBUTTONDOWN:
            {
                if (MenuFlyout?.IsOpen ?? false)
                {
                    MenuFlyout.Hide();
                }
                break;
            }
            case NativeMethods.WindowMessage.WM_NCRBUTTONDOWN:
            {
                PointInt32 pt = new PointInt32(lParam.ToInt32() & 0xFFFF, lParam.ToInt32() >> 16);
                FlyoutShowOptions options = new FlyoutShowOptions();
                options.ShowMode = FlyoutShowMode.Standard;
                options.Position = InfoHelper.SystemVersion.Build >= 22000 ?
                new Windows.Foundation.Point((pt.X - this.Window.AppWindow.Position.X - 8) / XamlRoot.RasterizationScale, (pt.Y - this.Window.AppWindow.Position.Y) / XamlRoot.RasterizationScale) :
                new Windows.Foundation.Point(pt.X - this.Window.AppWindow.Position.X - 8, pt.Y - this.Window.AppWindow.Position.Y);

                MenuFlyout?.ShowAt(this, options);
                return (IntPtr)0;
            }
        }

        if (WndProcHelper is null)
        {
            throw new InvalidOperationException($"Internal error: {nameof(WndProcHelper)} is missing.");
        }

        return WndProcHelper.CallInputNonClientPointerSourceWindowProc(hWnd, Msg, wParam, lParam);
    }

    private IntPtr WindowWndProc(IntPtr hWnd, NativeMethods.WindowMessage Msg, IntPtr wParam, IntPtr lParam)
    {
        switch (Msg)
        {
            case NativeMethods.WindowMessage.WM_SYSMENU:
            {
                return (IntPtr)0;
            }

            case NativeMethods.WindowMessage.WM_SYSCOMMAND:
            {
                NativeMethods.SystemCommand sysCommand = (NativeMethods.SystemCommand)(wParam.ToInt32() & 0xFFF0);

                if (sysCommand is NativeMethods.SystemCommand.SC_MOUSEMENU)
                {
                    FlyoutShowOptions options = new FlyoutShowOptions();
                    options.Position = new Windows.Foundation.Point(0, 15);
                    options.ShowMode = FlyoutShowMode.Standard;
                    MenuFlyout?.ShowAt(null, options);
                    return (IntPtr)0;
                }
                else if (sysCommand is NativeMethods.SystemCommand.SC_KEYMENU)
                {
                    FlyoutShowOptions options = new FlyoutShowOptions();
                    options.Position = new Windows.Foundation.Point(0, 45);
                    options.ShowMode = FlyoutShowMode.Standard;
                    MenuFlyout?.ShowAt(null, options);
                    return (IntPtr)0;
                }
                break;
            }
        }

        if (WndProcHelper is null)
        {
            throw new InvalidOperationException($"Internal error: {nameof(WndProcHelper)} is missing.");
        }

        return WndProcHelper.CallWindowProc(hWnd, Msg, wParam, lParam);
    }
}

