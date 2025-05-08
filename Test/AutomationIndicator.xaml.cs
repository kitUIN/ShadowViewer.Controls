 
using Windows.UI;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Test
{
    public sealed partial class AutomationIndicator : UserControl
    {
        private readonly Storyboard _rotateStoryboard;

        public bool Status
        {
            get => (bool)GetValue(StatusProperty);
            set => SetValue(StatusProperty, value);
        }

        public static readonly DependencyProperty StatusProperty =
            DependencyProperty.Register(nameof(Status), typeof(bool), typeof(AutomationIndicator),
                new PropertyMetadata(false, OnStatusChanged));

        private static void OnStatusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not AutomationIndicator control) return;
            if (e.NewValue is true) control.StartRefresh();
            else control.StopRefresh();
        }
         

        public AutomationIndicator()
        {
            this.InitializeComponent();
            _rotateStoryboard = (Storyboard)Resources["RotateStoryboard"];
            this.Loaded += AutomationIndicator_Loaded;
        }

        private void AutomationIndicator_Loaded(object sender, RoutedEventArgs e)
        {
            if (Status) StartRefresh();
            else StopRefresh(); 
        }

        public void StartRefresh()
        {
            RotatingIcon.Visibility = Visibility.Visible;
            _rotateStoryboard.Begin();
        }

        public void StopRefresh()
        {
            RotatingIcon.Visibility = Visibility.Collapsed;
            _rotateStoryboard.Stop();
        }
    }
}