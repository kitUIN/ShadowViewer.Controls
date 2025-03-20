using System.Diagnostics;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Windows.UI;
using Windows.UI.Text;
using Microsoft.UI;
using Microsoft.UI.Text;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ShadowViewer.Controls
{
    /// <summary>
    /// 
    /// </summary>
    public enum BadgeType
    {
        /// <summary>
        /// 
        /// </summary>
        Flat,

        /// <summary>
        /// 
        /// </summary>
        FlatSquare,

        /// <summary>
        /// 
        /// </summary>
        Plastic,

        /// <summary>
        /// 
        /// </summary>
        ForTheBadge,

        /// <summary>
        /// 
        /// </summary>
        Social
    }

    /// <summary>
    /// 仿照shields.io的Badge
    /// </summary>
    public sealed class ShieldsBadge : Control
    {
        /// <summary>
        /// 
        /// </summary>
        public ShieldsBadge()
        {
            this.DefaultStyleKey = typeof(ShieldsBadge);
            Loaded += ShieldsBadge_Loaded;  
        }

        /// <summary>
        ///  Badge 类型属性
        /// </summary>
        public BadgeType Type
        {
            get => (BadgeType)GetValue(TypeProperty);
            set => SetValue(TypeProperty, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty TypeProperty =
            DependencyProperty.Register(nameof(Type), typeof(BadgeType), typeof(ShieldsBadge),
                new PropertyMetadata(BadgeType.Flat, OnStateChanged));

        private static void OnStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ShieldsBadge button)
            {
                VisualStateManager.GoToState(button, e.NewValue.ToString(), true);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShieldsBadge_Loaded(object sender, RoutedEventArgs e)
        {
            if (RightBackground is SolidColorBrush rightBackground)
                RightTextForeground = CalculateForeground(rightBackground.Color);
            if (LeftBackground is SolidColorBrush leftBackground)
                LeftTextForeground = CalculateForeground(leftBackground.Color);
            VisualStateManager.GoToState(this, Type.ToString(), true);
        }

        /// <summary>
        /// 
        /// </summary>
        public string LeftText
        {
            get => (string)GetValue(LeftTextProperty);
            set => SetValue(LeftTextProperty, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty LeftTextProperty =
            DependencyProperty.Register(nameof(LeftText), typeof(string), typeof(ShieldsBadge),
                new PropertyMetadata(string.Empty));

        /// <summary>
        /// 左侧背景属性
        /// </summary>
        public Brush LeftBackground
        {
            get => (Brush)GetValue(LeftBackgroundProperty);
            set => SetValue(LeftBackgroundProperty, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty LeftBackgroundProperty =
            DependencyProperty.Register(nameof(LeftBackground), typeof(Brush), typeof(ShieldsBadge),
                new PropertyMetadata(new SolidColorBrush(new Color { A = 255, R = 85, G = 85, B = 85 }),
                    OnLeftBackgroundChanged));

        /// <summary>
        /// 右侧文字属性
        /// </summary>
        public string RightText
        {
            get => (string)GetValue(RightTextProperty);
            set => SetValue(RightTextProperty, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty RightTextProperty =
            DependencyProperty.Register(nameof(RightText), typeof(string), typeof(ShieldsBadge),
                new PropertyMetadata(string.Empty));

        /// <summary>
        /// 右侧背景属性
        /// </summary>
        public Brush RightBackground
        {
            get => (Brush)GetValue(RightBackgroundProperty);
            set => SetValue(RightBackgroundProperty, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty RightBackgroundProperty =
            DependencyProperty.Register(nameof(RightBackground), typeof(Brush), typeof(ShieldsBadge),
                new PropertyMetadata(new SolidColorBrush(new Color { A = 255, R = 136, G = 182, B = 0 }),
                    OnRightBackgroundChanged));

        /// <summary>
        /// 左侧文字前景色
        /// </summary>
        public Brush LeftTextForeground
        {
            get => (Brush)GetValue(LeftTextForegroundProperty);
            set => SetValue(LeftTextForegroundProperty, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty LeftTextForegroundProperty =
            DependencyProperty.Register(nameof(LeftTextForeground), typeof(Brush), typeof(ShieldsBadge),
                new PropertyMetadata(new SolidColorBrush(Colors.Black)));

        /// <summary>
        /// 右侧文字前景色
        /// </summary>
        public Brush RightTextForeground
        {
            get => (Brush)GetValue(RightTextForegroundProperty);
            set => SetValue(RightTextForegroundProperty, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty RightTextForegroundProperty =
            DependencyProperty.Register(nameof(RightTextForeground), typeof(Brush), typeof(ShieldsBadge),
                new PropertyMetadata(new SolidColorBrush(Colors.Black)));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void OnLeftBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ShieldsBadge control && e.NewValue is SolidColorBrush newBrush)
            {
                control.LeftTextForeground = CalculateForeground(newBrush.Color);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void OnRightBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ShieldsBadge control && e.NewValue is SolidColorBrush newBrush)
            {
                control.RightTextForeground = CalculateForeground(newBrush.Color);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="backgroundColor"></param>
        /// <returns></returns>
        private static Brush CalculateForeground(Color backgroundColor)
        {
            // 亮度计算公式：0.299*R + 0.587*G + 0.114*B
            var luminance = 0.299 * backgroundColor.R + 0.587 * backgroundColor.G + 0.114 * backgroundColor.B;
            // 如果亮度高（浅色背景），返回深色；否则返回浅色
            Debug.WriteLine(luminance);
            return luminance > 192 ? new SolidColorBrush(Colors.Black) : new SolidColorBrush(Colors.White);
        }
    }
}