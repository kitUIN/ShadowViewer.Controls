using Microsoft.UI.Xaml.Controls;
using System;
using Microsoft.UI.Xaml;
using System.Text.RegularExpressions;

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
    /// 获取或设置Content的值
    /// </summary>  
    public string LogoSource
    {
        get => (string)GetValue(LogoSourceProperty);
        set
        {
            SetValue(LogoSourceProperty, value);
            var uri = new Uri(value);
            string glyph;
            switch (uri.Scheme)
            {
                case "ms-appx" or "http" or "https":
                    BitmapIcon.Visibility = Visibility.Visible;
                    BitmapIcon.UriSource = uri;
                    break;
                case "font":
                    FontIcon.Visibility = Visibility.Visible;
                    glyph = value.Replace("font://", "");
                    if(glyph.StartsWith("\\")) glyph = Regex.Unescape(glyph);
                    FontIcon.Glyph = glyph;
                    break;
                case "fluent":
                    switch (uri.Host)
                    {
                        case "regular":
                            FluentRegularIcon.Visibility = Visibility.Visible;
                            glyph = value.Replace("fluent://regular/", "");
                            if (glyph.StartsWith("\\")) glyph = Regex.Unescape(glyph);
                            FluentRegularIcon.Glyph = glyph;
                            break;
                        case "filled":
                            FluentFilledIcon.Visibility = Visibility.Visible;
                            glyph = value.Replace("fluent://filled/", "");
                            if (glyph.StartsWith("\\")) glyph = Regex.Unescape(glyph);
                            FluentFilledIcon.Glyph = glyph;
                            break;
                    }
                    break;
            }
        }
    }

    /// <summary>
    /// 标识 Content 依赖属性。
    /// </summary>
    public static readonly DependencyProperty LogoSourceProperty =
        DependencyProperty.Register(nameof(LogoSource), typeof(string), typeof(PluginLogo),
            new PropertyMetadata(null, OnLogoSourceChanged));

    private static void OnLogoSourceChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
        
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
            new PropertyMetadata(20, OnFontIconSizeChanged));

    private static void OnFontIconSizeChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
        
    }
}