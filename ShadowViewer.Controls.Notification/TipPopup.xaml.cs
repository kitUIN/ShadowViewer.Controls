// Copyright (c) Richasy. All rights reserved.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace ShadowViewer.Controls;

/// <summary>
/// ��Ϣ����.
/// </summary>
public sealed partial class TipPopup : UserControl
{
    /// <summary>
    /// ��ʾ�ı�.
    /// </summary>
    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }
    /// <summary>
    /// <see cref="Text"/>����������.
    /// </summary>
    public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register(nameof(Text), typeof(string), typeof(TipPopup), new PropertyMetadata(string.Empty));

    /// <summary>
    /// ��ʾ�ı�.
    /// </summary>
    public InfoBarSeverity Severity
    {
        get => (InfoBarSeverity)GetValue(SeverityProperty);
        set => SetValue(SeverityProperty, value);
    }
    /// <summary>
    /// <see cref="Text"/>����������.
    /// </summary>
    public static readonly DependencyProperty SeverityProperty =
        DependencyProperty.Register(nameof(Severity), typeof(InfoBarSeverity), typeof(TipPopup), new PropertyMetadata(string.Empty));

    /// <summary>
    /// Initializes a new instance of the <see cref="TipPopup"/> class.
    /// </summary>
    public TipPopup() => InitializeComponent();

    /// <summary>
    /// ��ʾ��ʱ��
    /// </summary>
    public double DisplaySeconds { get; set; } = 2;
    /// <summary>
    /// Initializes a new instance of the <see cref="TipPopup"/> class.
    /// </summary>
    /// <param name="text">Ҫ��ʾ���ı�.</param>
    /// <param name="type">��Ϣ����</param>
    /// <param name="displaySeconds">��ʾ��ʱ��</param>
    public TipPopup(string text="", InfoBarSeverity type = InfoBarSeverity.Informational, double displaySeconds = 2)
        : this()
    {
        Text = text;
        DisplaySeconds = displaySeconds;
        Severity = type;
    }

}