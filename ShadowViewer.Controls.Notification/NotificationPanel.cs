using Microsoft.UI.Xaml.Controls;
using System;
using System.Diagnostics;
using System.Numerics;
using System.Threading.Tasks;
using CommunityToolkit.WinUI.Animations;
using Microsoft.UI.Xaml;
using System.Reflection;
using Microsoft.UI.Xaml.Controls.Primitives;

namespace ShadowViewer.Controls;

/// <summary>
/// 通知出现方式
/// </summary>
public enum NotificationShowDirection
{
    /// <summary>
    /// 从左向右出现
    /// </summary>
    LeftToRight,

    /// <summary>
    /// 从上向下出现
    /// </summary>
    TopToBottom,

    /// <summary>
    /// 从右向左出现
    /// </summary>
    RightToLeft,

    /// <summary>
    /// 从下向上出现
    /// </summary>
    BottomToTop,
}

/// <summary>
/// 通知窗体
/// </summary>
public sealed class NotificationPanel : StackPanel
{
    /// <summary>
    /// 通知窗体
    /// </summary>
    public NotificationPanel() : base()
    {
        Visibility = Visibility.Collapsed;
    }

    /// <summary>
    /// 发送通知
    /// </summary>
    /// <param name="popup">通知控件</param>
    /// <param name="displaySeconds">持续时间，小于等于0即为不自动关闭</param>
    // ReSharper disable once AsyncVoidMethod
    public async void Notify(InfoBar popup, double displaySeconds = 2)
    {
        Visibility = Visibility.Visible;
        popup.Loaded += (sender, _) => ShowTip(sender as InfoBar);
        if (displaySeconds <= 0 || popup.IsClosable)
        {
            popup.IsClosable = true;
            popup.CloseButtonClick += Popup_CloseButtonClick;
        }

        Children.Add(popup);
        if ((displaySeconds <= 0)) return;
        await Task.Delay(TimeSpan.FromSeconds(displaySeconds));
        Popup_CloseButtonClick(popup, null);
    }

    /// <summary>
    /// 关闭时触发动画，在父控件中删除自身
    /// </summary>
    /// <param name="popup">通知控件</param>
    /// <param name="args"></param>
    // ReSharper disable once AsyncVoidMethod
    private async void Popup_CloseButtonClick(InfoBar popup, object? args)
    {
        await AnimationBuilder.Create()
            .Opacity(
                to: 0,
                from: 1.0,
                duration: TimeSpan.FromSeconds(0.5))
            .StartAsync(popup);
        popup.Visibility = Visibility.Collapsed;
        Children.Remove(popup);
        if (Children.Count == 0) Visibility = Visibility.Collapsed;
    }

    /// <summary>
    /// 发送通知（简化版）
    /// </summary>
    /// <param name="text">通知内容</param>
    /// <param name="type">通知的重要级别</param>
    /// <param name="popupHorizontalAlignment">通知控件的横向布局</param>
    /// <param name="displaySeconds">持续时间，小于等于0即为不自动关闭</param>
    public void Notify(string text = "",
        InfoBarSeverity type = InfoBarSeverity.Informational,
        HorizontalAlignment popupHorizontalAlignment = HorizontalAlignment.Center,
        double displaySeconds = 2)
    {
        Notify(new InfoBar
        {
            Message = text,
            Severity = type,
            IsClosable = false,
            IsIconVisible = true,
            IsOpen = true,
            HorizontalAlignment = popupHorizontalAlignment,
            FlowDirection = FlowDirection.LeftToRight
        }, displaySeconds);
    }

    /// <summary>
    /// 展示通知控件，触发动画效果
    /// </summary>
    /// <param name="bar">通知控件</param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    private void ShowTip(InfoBar? bar)
    {
        if (bar == null) return;
        Vector3 to;
        Vector3 from;
        switch (ShowDirection)
        {
            case NotificationShowDirection.BottomToTop:
                to = new Vector3(0, 0, 0);
                from = new Vector3(0, 40, 0);
                break;
            case NotificationShowDirection.TopToBottom:
                to = new Vector3(0, 0, 0);
                from = new Vector3(0, -40, 0);
                break;
            case NotificationShowDirection.LeftToRight:
                to = new Vector3(0, 0, 0);
                from = new Vector3(40, 0, 0);
                break;
            case NotificationShowDirection.RightToLeft:
                to = new Vector3(0, 0, 0);
                from = new Vector3(-40, 0, 0);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        AnimationBuilder.Create()
            .Opacity(
                to: 1.0,
                from: 0,
                duration: TimeSpan.FromSeconds(0.5))
            .Translation(
                to: to,
                from: from,
                duration: TimeSpan.FromSeconds(0.5))
            .Start(bar);
    }


    /// <summary>
    /// 获取或设置Content的值
    /// </summary>  
    public NotificationShowDirection ShowDirection
    {
        get => (NotificationShowDirection)GetValue(ShowDirectionProperty);
        set => SetValue(ShowDirectionProperty, value);
    }

    /// <summary>
    /// 标识 Content 依赖属性。
    /// </summary>
    public static readonly DependencyProperty ShowDirectionProperty =
        DependencyProperty.Register(nameof(ShowDirection), typeof(NotificationShowDirection), typeof(NotificationPanel),
            new PropertyMetadata(NotificationShowDirection.BottomToTop, null));
}