using System;
using FluentIcons.Common;

namespace ShadowViewer.Controls.Attributes;

/// <summary>
/// 
/// </summary>
[AttributeUsage(AttributeTargets.Field)]
public class MenuFlyoutItemIconAttribute: Attribute
{
    /// <summary>
    /// Icon
    /// </summary>
    public Icon Icon { get; init; }

    /// <summary>
    /// IconVariant
    /// </summary>
    public IconVariant IconVariant { get; init; } = IconVariant.Regular;
}