using CommunityToolkit.Mvvm.ComponentModel;

namespace TagsTree.ViewModels.Controls;

public partial class InputContentDialogViewModels : ObservableObject
{
    /// <summary>
    /// 输入的格式
    /// </summary>
    [ObservableProperty] private string _warningText = "";

    /// <summary>
    /// 错误提示
    /// </summary>
    [ObservableProperty] private string _message = "";

    /// <summary>
    /// 打开错误提示
    /// </summary>
    [ObservableProperty] private bool _isOpen = false;
}
