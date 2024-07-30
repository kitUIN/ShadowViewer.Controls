using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Markup;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ShadowViewer.Controls;
/// <summary>
/// 
/// </summary>
[ContentProperty(Name = "Content")]
public class ShadowSettingHeader : Control
{
    /// <summary>
    /// 
    /// </summary>
    public ShadowSettingHeader()
    {
        this.DefaultStyleKey = typeof(ShadowSettingHeader);
    }
    /// <summary>
    /// 获取或设置Content的值
    /// </summary>  
    public object Content
    {
        get => (object)GetValue(ContentProperty);
        set => SetValue(ContentProperty, value);
    }

    /// <summary>
    /// 标识 Content 依赖属性。
    /// </summary>
    public static readonly DependencyProperty ContentProperty =
        DependencyProperty.Register(nameof(Content), typeof(object), typeof(ShadowSettingHeader), 
            new PropertyMetadata(null));


    /// <summary>
    /// 获取或设置ContentTemplate的值
    /// </summary>  
    public DataTemplate ContentTemplate
    {
        get => (DataTemplate)GetValue(ContentTemplateProperty);
        set => SetValue(ContentTemplateProperty, value);
    }

    /// <summary>
    /// 标识 ContentTemplate 依赖属性。
    /// </summary>
    public static readonly DependencyProperty ContentTemplateProperty =
        DependencyProperty.Register(nameof(ContentTemplate), typeof(DataTemplate), 
            typeof(ShadowSettingHeader), new PropertyMetadata(null ));

    /// <summary>
    /// 
    /// </summary>
    public static readonly DependencyProperty DescriptionProperty =
       DependencyProperty.Register(nameof(Description),
           typeof(string),
           typeof(ShadowSettingHeader),
           new PropertyMetadata("", OnDescriptionChanged));
    /// <summary>
    /// 
    /// </summary>
    public static readonly DependencyProperty HeaderProperty =
       DependencyProperty.Register(nameof(Header),
           typeof(string),
           typeof(ShadowSettingHeader),
           new PropertyMetadata("", OnHeaderChanged));
    /// <summary>
    /// 
    /// </summary>
    public static readonly DependencyProperty IsShowDescriptionProperty =
       DependencyProperty.Register(nameof(IsShowDescription),
           typeof(Visibility),
           typeof(ShadowSettingHeader),
           new PropertyMetadata(Visibility.Collapsed, OnIsShowDescriptionChanged));

    /// <summary>
    /// 
    /// </summary>
    public string Description
    {
        get => (string)GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }
    /// <summary>
    /// 
    /// </summary>
    public Visibility IsShowDescription
    {
        get => (Visibility)GetValue(IsShowDescriptionProperty);
        set => SetValue(IsShowDescriptionProperty, value);
    }
    /// <summary>
    /// 
    /// </summary>
    public string Header
    {
        get => (string)GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }


    private static void OnHeaderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (ShadowSettingHeader)d;
        control.Header = (string)e.NewValue;
    }
    private static void OnDescriptionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (ShadowSettingHeader)d;
        control.Description = (string)e.NewValue;
        control.IsShowDescription = !string.IsNullOrEmpty(control.Description) ? Visibility.Visible : Visibility.Collapsed;
    }
    private static void OnIsShowDescriptionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (ShadowSettingHeader)d;
        control.IsShowDescription = (Visibility)e.NewValue;
    }
}

