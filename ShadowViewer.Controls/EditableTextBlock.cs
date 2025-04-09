using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ShadowViewer.Controls;

/// <summary>
/// 
/// </summary>
/// <param name="Text"></param>
public class EditEndedEventArgs(string Text);


/// <summary>
/// 
/// </summary>
public sealed class EditableTextBlock : Control
{
    /// <summary>
    /// 
    /// </summary>
    public event EventHandler? EditStarted;
    /// <summary>
    /// 
    /// </summary>
    public event EventHandler<EditEndedEventArgs>? EditEnded;
    /// <summary>
    /// 
    /// </summary>
    public EditableTextBlock()
    {
        this.DefaultStyleKey = typeof(EditableTextBlock);
    }

    /// <summary>
    /// 
    /// </summary>
    public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register(nameof(Text), typeof(string), typeof(EditableTextBlock),
            new PropertyMetadata(string.Empty));

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
    private TextBox? _textBox;

    /// <summary>
    /// 
    /// </summary>
    private TextBlock? _textBlock;

    /// <summary>
    /// 
    /// </summary>
    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        _textBox = GetTemplateChild("PART_TextBox") as TextBox;
        _textBlock = GetTemplateChild("PART_TextBlock") as TextBlock;
        var grid = GetTemplateChild("PART_TextBlockGrid") as Grid;

        if (_textBlock != null)
        {
                
                
        }

        if (grid != null)
        {
            grid.Tapped += (s, e) => EnterEditMode();
            grid.PointerEntered += (s, e) => GoToState("PointerOver");
            grid.PointerExited += (s, e) => GoToState("Normal");
        }

        if (_textBox != null)
        {
            _textBox.LostFocus += (s, e) => ExitEditMode();
            _textBox.KeyDown += (s, e) =>
            {
                if (e.Key == Windows.System.VirtualKey.Enter)
                {
                    ExitEditMode();
                }
            };
        }

        UpdateVisualState();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="state"></param>
    private void GoToState(string state)
    {
        VisualStateManager.GoToState(this, state, true);
    }

    /// <summary>
    /// 
    /// </summary>
    private bool _isEditing = false;

    /// <summary>
    /// 
    /// </summary>
    private void EnterEditMode()
    {
        _isEditing = true;
        if (_textBlock != null)
            _textBlock.Visibility = _isEditing ? Visibility.Collapsed : Visibility.Visible;
        if (_textBox != null)
            _textBox.Visibility = _isEditing ? Visibility.Visible : Visibility.Collapsed;

        _textBox?.Focus(FocusState.Programmatic);
        // _textBox?.SelectAll();
        EditStarted?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// 
    /// </summary>
    private void ExitEditMode()
    {
        _isEditing = false;
        UpdateVisualState();
        EditEnded?.Invoke(this, new EditEndedEventArgs(Text));
    }

    /// <summary>
    /// 
    /// </summary>
    private void UpdateVisualState()
    {
        if (_textBox != null)
            _textBox.Visibility = _isEditing ? Visibility.Visible : Visibility.Collapsed;

        if (_textBlock != null)
            _textBlock.Visibility = _isEditing ? Visibility.Collapsed : Visibility.Visible;
    }
}