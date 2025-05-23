using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using ShadowViewer.Controls;
using InfoBar = Microsoft.UI.Xaml.Controls.InfoBar;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Test
{
    public sealed class PathBoxItem
    {
        public string Title { get; set; }

        public string Path { get; set; }
    }
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public ObservableCollection<PathBoxItem> ItemsList =
        [
            new PathBoxItem{Title = "Home"},
            new PathBoxItem{Title = "Download"},
            new PathBoxItem{Title = "Desktop"},
            new PathBoxItem{Title = "Folder"},
        ];
        public MainWindow()
        {
            this.InitializeComponent();
        }

        private void myButton_Click(object sender, RoutedEventArgs e)
        {
            ((PingButton)sender).State = PingButtonState.Ping;
        }

        // private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        // {
        //     TipContainerCenter.Notify("测试success",InfoBarSeverity.Success);
        // }
        //
        // private void ButtonBase_OnClick2(object sender, RoutedEventArgs e)
        // {
        //     TipContainerCenter.Notify("测试Info", InfoBarSeverity.Informational);
        // }
        // private void ButtonBase_OnClick3(object sender, RoutedEventArgs e)
        // {
        //     TipContainerCenter.Notify("测试Warn长长的", InfoBarSeverity.Warning);
        // }
        // private void ButtonBase_OnClick4(object sender, RoutedEventArgs e)
        // {
        //     TipContainerCenter.Notify("测试Error", InfoBarSeverity.Error);
        // }
        //
        // private void ButtonBase_OnClick5(object sender, RoutedEventArgs e)
        // {
        //     TipContainerCenter.Notify(new InfoBar
        //     {
        //         Message = "Test",
        //         Severity = InfoBarSeverity.Error,
        //         IsClosable = true,
        //         IsIconVisible = true,
        //         IsOpen = true,
        //         Content = new Grid()
        //         {
        //           Children  =
        //           {
        //               new ProgressBar()
        //               {
        //                   IsIndeterminate = true
        //               }
        //           }
        //         },
        //         HorizontalAlignment = HorizontalAlignment.Center,
        //         FlowDirection = FlowDirection.LeftToRight
        //     }, displaySeconds: 0);
        // }
    }
}