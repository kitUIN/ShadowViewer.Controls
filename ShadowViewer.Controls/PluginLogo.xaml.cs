using Microsoft.UI.Xaml.Controls;
using System;
using Microsoft.UI.Xaml;
using System.Text.RegularExpressions;
using FluentIcons.Common;
using FluentIcons.WinUI;

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
                    TopViewBox.Child = new BitmapIcon()
                    {
                        UriSource = uri,
                    };
                    break;
                case "font":
                    glyph = value.Replace("font://", "");
                    if(glyph.StartsWith("\\")) glyph = Regex.Unescape(glyph);
                    TopViewBox.Child = new FontIcon()
                    {
                        Glyph = glyph,
                    };
                    break;
                case "fluent":
                    switch (uri.Host)
                    {
                        case "regular": 
                            glyph = value.Replace("fluent://regular/", "");
                            Enum.TryParse(glyph, out Icon regularIcon);
                            TopViewBox.Child = new FluentIcon()
                            {
                                Icon = regularIcon,
                                IconVariant = IconVariant.Regular
                            };
                            break;
                        case "filled":
                            glyph = value.Replace("fluent://filled/", "");
                            Enum.TryParse(glyph, out Icon filledIcon);
                            TopViewBox.Child = new FluentIcon()
                            {
                                Icon = filledIcon,
                                IconVariant = IconVariant.Filled
                            };
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
    
}