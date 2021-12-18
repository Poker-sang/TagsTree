using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TagsTree.Services;
using TagsTree.Services.ExtensionMethods;

namespace TagsTree.Controls;

/// <summary>
/// InputContentDialog.xaml 的交互逻辑
/// </summary>
[INotifyPropertyChanged]
public partial class InputContentDialog : ContentDialog
{
    public InputContentDialog() => InitializeComponent();
    /// <summary>
    /// 输入的内容
    /// </summary>
    [ObservableProperty] private string _text = "";
    /// <summary>
    /// 输入的格式
    /// </summary>
    [ObservableProperty] private string _warningText = "";
    public void Load(FileSystemHelper.InvalidMode mode, string text = "")
    {
        switch (mode)
        {
            case FileSystemHelper.InvalidMode.Name:
                WarningText = @"不能包含\/:*?""<>|和除空格外的空白字符";
                _invalidRegex = FileSystemHelper.GetInvalidNameChars;
                break;
            case FileSystemHelper.InvalidMode.Path:
                WarningText = @"不能包含/:*?""<>|和除空格外的空白字符";
                _invalidRegex = FileSystemHelper.GetInvalidPathChars;
                break;
            default: throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
        }
        Text = text;
    }
    /// <summary>
    /// 是否取消这次输入
    /// </summary>
    public bool Canceled;
    private bool _exceptionThrown;
    private string _invalidRegex = "";

    public new async Task ShowAsync()
    {
        Canceled = true;
        _exceptionThrown = false;
        _ = await base.ShowAsync();
        if (_exceptionThrown)
            await ShowMessageDialog.Information(true, WarningText);
    }

    #region 事件处理

    private void InputName_OnPrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs e)
    {
        if (!new Regex($@"^[^{_invalidRegex}]+$").IsMatch(Text))
            _exceptionThrown = true;
        else Canceled = false;
    }

    private void InputName_OnCloseButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args) => Canceled = true;

    #endregion
}