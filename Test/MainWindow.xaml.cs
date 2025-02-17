using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using ShadowViewer.Controls;
using InfoBar = Microsoft.UI.Xaml.Controls.InfoBar;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Test
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
        }

        private void myButton_Click(object sender, RoutedEventArgs e)
        {
           
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            TipContainerCenter.Show("≤‚ ‘success",InfoBarSeverity.Success);
        }

        private void ButtonBase_OnClick2(object sender, RoutedEventArgs e)
        {
            TipContainerCenter.Show("≤‚ ‘Info", InfoBarSeverity.Informational);
        }
        private void ButtonBase_OnClick3(object sender, RoutedEventArgs e)
        {
            TipContainerCenter.Show("≤‚ ‘Warn", InfoBarSeverity.Warning);
        }
        private void ButtonBase_OnClick4(object sender, RoutedEventArgs e)
        {
            TipContainerCenter.Show("≤‚ ‘Error", InfoBarSeverity.Error);
        }
    }
}
