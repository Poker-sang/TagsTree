using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TagsTree.Services.ExtensionMethods;

namespace TagsTree.Controls;

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
    public void Load(string title, Func<InputContentDialog, string?> judge, FileSystemHelper.InvalidMode mode, string text = "")
    {
        Title = title;
        _judge = judge;
        InfoBar.IsOpen = false;
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
    private bool _canceled;
    private Func<InputContentDialog, string?> _judge = null!;
    private string _invalidRegex = "";

    #region 事件处理

    public new async Task<bool> ShowAsync()
    {
        _canceled = true;
        _ = await base.ShowAsync();
        return _canceled;
    }

    private void OnPrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs e)
    {
        e.Cancel = true;
        if (!new Regex($@"^[^{_invalidRegex}]+$").IsMatch(Text))
        {
            InfoBar.Message = WarningText;
            InfoBar.IsOpen = true;
            return;
        }

        var result = _judge(this);
        if (result is not null)
        {
            InfoBar.Message = result;
            InfoBar.IsOpen = true;
            return;
        }

        e.Cancel = false;
        _canceled = false;
    }

    private void OnCloseButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args) => _canceled = true;

    #endregion
}
