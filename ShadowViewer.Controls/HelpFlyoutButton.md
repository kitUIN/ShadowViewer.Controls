# HelpFlyoutButton

控件位于 `ShadowViewer.Controls` 命名空间。在页面中声明 `xmlns:controls="using:ShadowViewer.Controls"` 后即可使用。

使用主题强调色（`AccentTextFillColorPrimaryBrush`）符号、透明背景的通用说明按钮，不包含具体设置的文案或图片。Flyout 边框距所在页面的可见边缘至少 20px，与原按钮保持 8px 间隔，不遮挡按钮。优先按 `Placement` 指定的方向展开；空间不足时换边，仍放不下则限制面板尺寸并滚动内容。窗口缩放或页面滚动时重新计算位置；页面小到无法保留边距时关闭弹层。

```xml
<controls:HelpFlyoutButton AccessibleName="功能说明" Placement="Top">
    <controls:HelpFlyoutButton.FlyoutContent>
        <StackPanel Width="320" Spacing="12">
            <TextBlock Text="标题" FontWeight="SemiBold" />
            <TextBlock Text="替换为当前功能的说明。" TextWrapping="Wrap" />
        </StackPanel>
    </controls:HelpFlyoutButton.FlyoutContent>
</controls:HelpFlyoutButton>
```

`FlyoutContent` 接受文本或任意 UIElement；数据对象可搭配 `FlyoutContentTemplate`。使用方通过 `AccessibleName` 提供本地化无障碍名称。

鼠标悬浮不会抢占焦点；移出按钮和弹出面板后关闭，180 ms 宽限允许跨过面板间隙。点击（含触摸、Enter 和空格）会保持面板展开，鼠标移出不关闭；点击外部或按 Escape 关闭。控件卸载时停止计时并关闭面板。
