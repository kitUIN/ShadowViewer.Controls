using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ShadowViewer.Controls;

/// <summary>
/// 
/// </summary>
public enum PingButtonState
{
    /// <summary>
    /// 默认
    /// </summary>
    Normal,

    /// <summary>
    /// Ping
    /// </summary>
    Ping,

    /// <summary>
    /// 成功
    /// </summary>
    Success,

    /// <summary>
    /// 失败
    /// </summary>
    Error
}

/// <summary>
/// 
/// </summary>
public class PingButton : Button
{
    /// <summary>
    /// 
    /// </summary>
    public static readonly DependencyProperty PingTextProperty =
        DependencyProperty.Register(nameof(PingText), typeof(string), typeof(PingButton),
            new PropertyMetadata("Ping"));

    /// <summary>
    /// 
    /// </summary>
    public string PingText
    {
        get => (string)GetValue(PingTextProperty);
        set => SetValue(PingTextProperty, value);
    }

    /// <summary>
    /// 
    /// </summary>
    public static readonly DependencyProperty SuccessTextProperty =
        DependencyProperty.Register(nameof(SuccessText), typeof(string), typeof(PingButton),
            new PropertyMetadata("Success"));

    /// <summary>
    /// 
    /// </summary>
    public string SuccessText
    {
        get => (string)GetValue(SuccessTextProperty);
        set => SetValue(SuccessTextProperty, value);
    }

    /// <summary>
    /// 
    /// </summary>
    public static readonly DependencyProperty ErrorTextProperty =
        DependencyProperty.Register(nameof(ErrorText), typeof(string), typeof(PingButton),
            new PropertyMetadata("Error"));

    /// <summary>
    /// 
    /// </summary>
    public string ErrorText
    {
        get => (string)GetValue(ErrorTextProperty);
        set => SetValue(ErrorTextProperty, value);
    }


    /// <summary>
    /// 
    /// </summary>
    public static readonly DependencyProperty StateProperty =
        DependencyProperty.Register(nameof(State), typeof(PingButtonState), typeof(PingButton),
            new PropertyMetadata(PingButtonState.Normal, OnStateChanged));

    /// <summary>
    /// 
    /// </summary>
    public PingButtonState State
    {
        get => (PingButtonState)GetValue(StateProperty);
        set => SetValue(StateProperty, value);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="d"></param>
    /// <param name="e"></param>
    private static void OnStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is PingButton button)
        {
            VisualStateManager.GoToState(button, e.NewValue.ToString(), true);
            switch ((PingButtonState)e.NewValue)
            {
                case PingButtonState.Ping:
                    button.Text = button.PingText;
                    break;
                case PingButtonState.Success:
                    button.Text = button.SuccessText;
                    break;
                case PingButtonState.Error:
                    button.Text = button.ErrorText;
                    break;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public static readonly DependencyProperty IconProperty =
        DependencyProperty.Register(nameof(Icon), typeof(string), typeof(PingButton), new PropertyMetadata(""));

    /// <summary>
    /// 
    /// </summary>
    public string Icon
    {
        get => (string)GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    /// <summary>
    /// 
    /// </summary>
    public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register(nameof(Text), typeof(string), typeof(PingButton), new PropertyMetadata(""));

    /// <summary>
    /// 
    /// </summary>
    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    /// <summary>
    /// 
    /// </summary>
    public PingButton()
    {
        DefaultStyleKey = typeof(PingButton);
        Loaded += PingButton_Loaded;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void PingButton_Loaded(object sender, RoutedEventArgs e)
    {
        VisualStateManager.GoToState(this, State.ToString(), true);
    }
}