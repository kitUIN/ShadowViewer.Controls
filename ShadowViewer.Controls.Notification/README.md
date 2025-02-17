# ShadowViewer.Controls.Notification

弹出通知控件 

![nuget](https://img.shields.io/nuget/v/ShadowViewer.Controls.Notification?style=flat-square)

## 快速开始

首先需要在你要显示通知的页面初始化NotificationPanel
```xml
<Window
    x:Class="Test.MainWindow"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:scontrols="using:ShadowViewer.Controls"
    xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
    xmlns:local="using:Test"
    xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
    mc:Ignorable="d">
    <Grid x:Name="Top" Background="{ThemeResource ApplicationPageBackgroundThemeBrush}"> 
        <scontrols:NotificationPanel
            x:Name="TipContainerCenter"
            Margin="0,0,0,52"
            HorizontalAlignment="Center"
            VerticalAlignment="Bottom"
            Canvas.ZIndex="1"
            FlowDirection="RightToLeft"
            Orientation="Vertical"
            Visibility="Collapsed" />
    </Grid>
</Window>
```
后续只要调用通知方法即可
```csharp
// 简单字符串内容
private void ButtonBase_OnClick4(object sender, RoutedEventArgs e)
{
    TipContainerCenter.Notify("测试Error", InfoBarSeverity.Error);
}
// 自定义弹出控件内容
private void ButtonBase_OnClick5(object sender, RoutedEventArgs e)
{
    TipContainerCenter.Notify(new InfoBar
    {
        Message = "Test",
        Severity = InfoBarSeverity.Error,
        IsClosable = false,
        IsIconVisible = true,
        IsOpen = true,
        Content = new Grid()
        {
            Children  =
            {
                new ProgressBar()
                {
                    IsIndeterminate = true
                }
            }
        },
        HorizontalAlignment = HorizontalAlignment.Center,
        FlowDirection = FlowDirection.LeftToRight
    }, displaySeconds:0);
}
```

## 鸣谢
[Bili.Uwp](https://github.com/Richasy/Bili.Uwp) - 哔哩客户端 - 参考了TipPopup实现
