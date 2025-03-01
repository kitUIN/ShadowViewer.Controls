using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.WinUI.Animations;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace ShadowViewer.Controls.Extensions;
/// <summary>
/// 
/// </summary>
public static class InfoBarExtension
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="popup"></param>
    /// <param name="displaySeconds"></param>
    public static async Task Close(this InfoBar popup, double displaySeconds = 2)
    {

        if (popup.Parent is NotificationPanel panel)
        {
            if ((displaySeconds > 0)) await Task.Delay(TimeSpan.FromSeconds(displaySeconds));
            await AnimationBuilder.Create()
                .Opacity(
                    to: 0,
                    from: 1.0,
                    duration: TimeSpan.FromSeconds(0.5))
                .StartAsync(popup);
            popup.Visibility = Visibility.Collapsed;
            panel.Children.Remove(popup);
            if (panel.Children.Count == 0) panel.Visibility = Visibility.Collapsed;
        }
    }
}