using Microsoft.UI.Xaml.Controls;
using System;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ShadowViewer.Controls
{
    public sealed class NotificationPanel : StackPanel
    {
        public NotificationPanel():base()
        {
        }

        public async void Show(TipPopup popup)
        {
            Visibility = Visibility.Visible;
            Children.Add(popup);
            await Task.Delay(TimeSpan.FromSeconds(popup.DisplaySeconds));
            Children.Remove(popup);
            if (Children.Count == 0) Visibility = Visibility.Collapsed;
        }

        public void Show(string text = "", InfoBarSeverity type = InfoBarSeverity.Informational, double displaySeconds = 2)
        {
            Show(new TipPopup(text, type, displaySeconds));
        }
    }
}
