using Microsoft.UI.Xaml.Controls;
using System;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ShadowViewer.Controls
{
    /// <summary>
    /// 通知窗体
    /// </summary>
    public sealed class NotificationPanel : StackPanel
    {
        /// <summary>
        /// 通知窗体
        /// </summary>
        public NotificationPanel(): base()
        {
            Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="popup"></param>
        public async void Show(TipPopup popup)
        {
            Visibility = Visibility.Visible;
            Children.Add(popup);
            await Task.Delay(TimeSpan.FromSeconds(popup.DisplaySeconds));
            Children.Remove(popup);
            if (Children.Count == 0) Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="type"></param>
        /// <param name="displaySeconds"></param>
        public void Show(string text = "", InfoBarSeverity type = InfoBarSeverity.Informational, double displaySeconds = 2)
        {
            Show(new TipPopup(text, type, displaySeconds));
        }
    }
}
