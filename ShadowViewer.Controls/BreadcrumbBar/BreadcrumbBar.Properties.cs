// Copyright (c) Files Community
// Licensed under the MIT License.

using CommunityToolkit.WinUI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace ShadowViewer.Controls
{
	public partial class BreadcrumbBar : Control
	{
		[GeneratedDependencyProperty]
		public partial FrameworkElement? RootItem { get; set; }

		[GeneratedDependencyProperty]
		public partial object? ItemsSource { get; set; }

		[GeneratedDependencyProperty]
		public partial object? ItemTemplate { get; set; }
	}
}
