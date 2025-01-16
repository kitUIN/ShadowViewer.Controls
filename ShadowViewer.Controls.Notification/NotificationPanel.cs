using Microsoft.UI.Xaml.Controls;
using System;
using System.Numerics;
using System.Threading.Tasks;
using CommunityToolkit.WinUI.Animations;
using Microsoft.UI.Xaml;

namespace ShadowViewer.Controls;
/// <summary>
/// 
/// </summary>
public enum NotificationTipPosition
{
    /// <summary>
    /// 
    /// </summary>
    Left,
    /// <summary>
    /// 
    /// </summary>
    Top,
    /// <summary>
    /// 
    /// </summary>
    Right,
    /// <summary>
    /// 
    /// </summary>
    Bottom,
}
/// <summary>
/// 
/// </summary>
public sealed class NotificationPanel : StackPanel
{
    /// <summary>
    /// 
    /// </summary>
    public NotificationPanel() : base()
    {
        Visibility = Visibility.Collapsed;
    }

    /// <summary>
    /// 
    /// </summary>
    private async void Show(InfoBar popup, double displaySeconds = 2)
    {
        Visibility = Visibility.Visible;
        popup.Loaded += Popup_Loaded;
        Children.Add(popup);
        await Task.Delay(TimeSpan.FromSeconds(displaySeconds));
        await HideTip(popup);
        Children.Remove(popup);
        if (Children.Count == 0) Visibility = Visibility.Collapsed;
    }

    private void Popup_Loaded(object sender, RoutedEventArgs e)
    {
        ShowTip(sender as InfoBar);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="text"></param>
    /// <param name="type"></param>
    /// <param name="displaySeconds"></param>
    public void Show(string text = "", 
        InfoBarSeverity type = InfoBarSeverity.Informational,
        double displaySeconds = 2)
    {
        Show(new InfoBar
        {
            Message = text, 
            Severity = type,
            IsClosable = false,
            IsIconVisible = true,
            IsOpen = true,
            FlowDirection = FlowDirection.LeftToRight
        }, displaySeconds);
    }


    /// <summary>
    /// Hide
    /// </summary>
    private async Task HideTip(InfoBar bar)
    {
        await AnimationBuilder.Create()
            .Opacity(
                to: 0,
                from: 1.0,
                duration: TimeSpan.FromSeconds(0.5))
            .StartAsync(bar);
        bar.Visibility = Visibility.Collapsed;
    }

    private void ShowTip(InfoBar? bar)
    {
        if (bar == null) return;
        Vector3 to;
        Vector3 from;
        switch (TipPosition)
        {
            case NotificationTipPosition.Bottom:
                to = new Vector3(0, 0, 0);
                from = new Vector3(0, 40, 0);
                break;
            case NotificationTipPosition.Top:
                to = new Vector3(0, 0, 0);
                from = new Vector3(0, -40, 0);
                break;
            case NotificationTipPosition.Left:
                to = new Vector3(0, 0, 0);
                from = new Vector3(-40, 0, 0);
                break;
            case NotificationTipPosition.Right:
                to = new Vector3(0, 0, 0);
                from = new Vector3(40, 0, 0);
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
    public NotificationTipPosition TipPosition
    {
        get => (NotificationTipPosition)GetValue(TipPositionProperty);
        set => SetValue(TipPositionProperty, value);
    }

    /// <summary>
    /// 标识 Content 依赖属性。
    /// </summary>
    public static readonly DependencyProperty TipPositionProperty =
        DependencyProperty.Register(nameof(TipPosition), typeof(NotificationTipPosition), typeof(NotificationPanel),
            new PropertyMetadata(NotificationTipPosition.Bottom, null));
}