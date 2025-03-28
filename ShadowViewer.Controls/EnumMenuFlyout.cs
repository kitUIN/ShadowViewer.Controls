using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using System.Linq;
using System;
using System.Reflection; 
using FluentIcons.WinUI;
using ShadowViewer.Controls.Attributes;

namespace ShadowViewer.Controls;

/// <summary>
/// 
/// </summary>
public class EnumMenuFlyout : MenuFlyout
{
    /// <summary>
    /// 
    /// </summary>
    public EnumMenuFlyout() : base()
    {
    }


    /// <summary>
    /// 
    /// </summary>
    public static readonly DependencyProperty DefaultStyleProperty =
        DependencyProperty.RegisterAttached(
            nameof(DefaultStyle),
            typeof(Style),
            typeof(EnumMenuFlyout),
            new PropertyMetadata(null));

    /// <summary>
    /// 
    /// </summary>
    public Style? DefaultStyle
    {
        get => (Style)GetValue(DefaultStyleProperty);
        set => SetValue(DefaultStyleProperty, value);
    }

    /// <summary>
    /// 
    /// </summary>
    public static readonly DependencyProperty TextConverterProperty =
        DependencyProperty.RegisterAttached(
            nameof(TextConverter),
            typeof(IValueConverter),
            typeof(EnumMenuFlyout),
            new PropertyMetadata(null));

    /// <summary>
    /// 
    /// </summary>
    public IValueConverter TextConverter
    {
        get => (IValueConverter)GetValue(TextConverterProperty);
        set => SetValue(TextConverterProperty, value);
    }


    /// <summary>
    /// 
    /// </summary>
    public static readonly DependencyProperty EnumSourceProperty =
        DependencyProperty.RegisterAttached(
            nameof(EnumSource),
            typeof(Type),
            typeof(EnumMenuFlyout),
            new PropertyMetadata(null, OnEnumSourceChanged));

    /// <summary>
    /// 
    /// </summary>
    public Type EnumSource
    {
        get => (Type)GetValue(EnumSourceProperty);
        set => SetValue(EnumSourceProperty, value);
    }

    private static void OnEnumSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not EnumMenuFlyout menuFlyout || e.NewValue is not Type enumType) return;
        if (!enumType.IsEnum)
        {
            throw new ArgumentException("EnumSource must be an Enum type.");
        }

        menuFlyout.Items.Clear();
        var converter = menuFlyout.TextConverter;
        foreach (var value in Enum.GetValues(enumType))
        {
            var field = enumType.GetField(value.ToString()!);
            var icon = field?.GetCustomAttribute<MenuFlyoutItemIconAttribute>();
            var menuItem = new RadioMenuFlyoutItem
            {
                Text = converter != null
                    ? converter.Convert(value, typeof(string), null, null)?.ToString()
                    : value.ToString(),
                Tag = value,
            };
            if (icon is { Icon: { } iconIcon })
            {
                menuItem.Icon = new FluentIcon()
                {
                    Icon = iconIcon,
                    IconVariant = icon.IconVariant,
                };
            }

            menuItem.Click += (_, _) =>
            {
                menuFlyout.SelectedValue = value;
                var command = menuFlyout.EnumCommand;
                if (command != null && command.CanExecute(value))
                {
                    command.Execute(value);
                }
            };

            menuFlyout.Items.Add(menuItem);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public static readonly DependencyProperty EnumCommandProperty =
        DependencyProperty.RegisterAttached(
            nameof(EnumCommand),
            typeof(System.Windows.Input.ICommand),
            typeof(EnumMenuFlyout),
            new PropertyMetadata(null));

    /// <summary>
    /// 
    /// </summary>
    public System.Windows.Input.ICommand? EnumCommand
    {
        get => (System.Windows.Input.ICommand)GetValue(EnumCommandProperty);
        set => SetValue(EnumCommandProperty, value);
    }

    /// <summary>
    /// 
    /// </summary>
    public static readonly DependencyProperty SelectedValueProperty =
        DependencyProperty.RegisterAttached(
            "SelectedValue",
            typeof(object),
            typeof(EnumMenuFlyout),
            new PropertyMetadata(null, OnSelectedValueChanged));

    /// <summary>
    /// 
    /// </summary>
    public object SelectedValue
    {
        get => (object)GetValue(SelectedValueProperty);
        set => SetValue(SelectedValueProperty, value);
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="d"></param>
    /// <param name="e"></param>
    private static void OnSelectedValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not EnumMenuFlyout menuFlyout || e.NewValue is not { } selectedValue) return;
        foreach (var item in menuFlyout.Items.OfType<RadioMenuFlyoutItem>())
        {
            item.IsChecked = Equals(item.Tag, selectedValue);
        }
    }
}