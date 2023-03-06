using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Controls;
using TagsTree.Services.ExtensionMethods;
using TagsTree.Views.ViewModels.Controls;
using WinUI3Utilities;
using WinUI3Utilities.Attributes;

namespace TagsTree.Controls;

[DependencyProperty<string>("Text", DefaultValue = @"""""")]
public partial class InputContentDialog : UserControl
{
    private readonly InputContentDialogViewModels _vm = new();

    public InputContentDialog() => InitializeComponent();

    public void Load(string title, Func<InputContentDialog, string?> judge, FileSystemHelper.InvalidMode mode, string text = "")
    {
        Content.To<ContentDialog>().Title = title;
        _judge = judge;
        switch (mode)
        {
            case FileSystemHelper.InvalidMode.Name:
                _vm.WarningText = @"不能包含\/:*?""<>|和除空格外的空白字符";
                _invalidRegex = FileSystemHelper.GetInvalidNameChars;
                break;
            case FileSystemHelper.InvalidMode.Path:
                _vm.WarningText = @"不能包含/:*?""<>|和除空格外的空白字符";
                _invalidRegex = FileSystemHelper.GetInvalidPathChars;
                break;
            default:
                ThrowHelper.ArgumentOutOfRange(mode);
                return;
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

    public async Task<bool> ShowAsync()
    {
        _canceled = true;
        _ = await Content.To<ContentDialog>().ShowAsync();
        return _canceled;
    }

    private void OnPrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs e)
    {
        e.Cancel = true;
        if (!new Regex($@"^[^{_invalidRegex}]+$").IsMatch(Text))
        {
            _vm.Message = _vm.WarningText;
            _vm.IsOpen = true;
            return;
        }

        var result = _judge(this);
        if (result is not null)
        {
            _vm.Message = result;
            _vm.IsOpen = true;
            return;
        }

        e.Cancel = false;
        _canceled = false;
    }

    private void OnCloseButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs e) => _canceled = true;

    #endregion
}
