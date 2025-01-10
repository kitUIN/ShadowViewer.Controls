using Microsoft.UI.Xaml.Controls;
using System;
using Microsoft.UI.Xaml;

namespace ShadowViewer.Controls;

/// <summary>
/// 
/// </summary>
public sealed partial class PluginLogo : UserControl
{
    /// <summary>
    /// 
    /// </summary>
    public PluginLogo()
    {
        InitializeComponent();
    }
    /// <summary>
    /// 
    /// </summary>
    public string Logo
    {
        set
        {
            var url = new Uri(value);
            switch (url.Scheme)
            {
                case "ms-appx" or "http" or "https":
                    BitmapIcon.Visibility = Visibility.Visible;
                    BitmapIcon.UriSource = url;
                    break;
                case "font":
                    FontIcon.Visibility = Visibility.Visible;
                    FontIcon.Glyph = value.Replace("font://", "");
                    break;
                case "fluent":
                    switch (url.Host)
                    {
                        case "regular":
                            FluentRegularIcon.Visibility = Visibility.Visible;
                            FluentRegularIcon.Glyph = value.Replace("fluent://regular/", "");
                            break;
                        case "filled":
                            FluentFilledIcon.Visibility = Visibility.Visible;
                            FluentFilledIcon.Glyph = value.Replace("fluent://filled/", "");
                            break;
                    }

                    break;
            }
        }
    }

    /// <summary>
    /// 获取或设置Content的值
    /// </summary>  
    public int FontIconSize
    {
        get => (int)GetValue(FontIconSizeProperty);
        set => SetValue(FontIconSizeProperty, value);
    }

    /// <summary>
    /// 标识 Content 依赖属性。
    /// </summary>
    public static readonly DependencyProperty FontIconSizeProperty =
        DependencyProperty.Register(nameof(FontIconSize), typeof(int), typeof(PluginLogo),
            new PropertyMetadata(16, OnFontIconSizeChanged));

    private static void OnFontIconSizeChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
        var target = obj as PluginLogo;
        if (target != null)  
            target.FontIconSize = (int)args.NewValue;
    }
}