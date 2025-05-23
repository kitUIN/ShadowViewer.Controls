﻿// Copyright (c) Files Community
// Licensed under the MIT License.

using Microsoft.UI.Xaml.Controls;

namespace ShadowViewer.Controls
{
	public record class BreadcrumbBarItemClickedEventArgs(BreadcrumbBarItem Item, int Index, bool IsRootItem = false);

	public record class BreadcrumbBarItemDropDownFlyoutEventArgs(MenuFlyout Flyout, BreadcrumbBarItem? Item = null, int Index = -1, bool IsRootItem = false);
}
