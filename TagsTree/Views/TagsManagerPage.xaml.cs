using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TagsTree.Commands;
using TagsTree.Services;
using TagsTree.Services.ExtensionMethods;
using TagsTree.ViewModels;

namespace TagsTree.Views;

/// <summary>
/// TagsManagerPage.xaml 的交互逻辑
/// </summary>
public partial class TagsManagerPage : Page
{
    public TagsManagerPage()
    {
        Current = this;
        _vm = new TagsManagerViewModel();
        RPasteCmClick = new RelayCommand(_ => _clipBoard is not null, PasteCmClick);
        InitializeComponent();
    }

    public static TagsManagerPage Current = null!;

    private readonly TagsManagerViewModel _vm;
    public RelayCommand RPasteCmClick { get; }

    private TagViewModel? _clipBoard;
    private TagViewModel? ClipBoard
    {
        get => _clipBoard;
        set
        {
            if (Equals(value, _clipBoard)) return;
            _clipBoard = value;
            CmPPasteX.IsEnabled = value is not null;
            RPasteCmClick.OnCanExecuteChanged();
        }
    }

    #region 事件处理

    //private void TvTags_OnDragItemsCompleted(TreeView sender, TreeViewDragItemsCompletedEventArgs e)
    //{
    //    if (e.DropResult is DataPackageOperation.Move)
    //        MoveTag((TagViewModel)e.Items[0], (TagViewModel)e.NewParentItem);
    //}

    private void NameChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs e) => _vm.Name = Regex.Replace(_vm.Name, $@"[{FileSystemHelper.GetInvalidNameChars}]+", "");

    private void Tags_OnItemInvoked(TreeView sender, TreeViewItemInvokedEventArgs e) => TbPath.Path = ((TagViewModel?)e.InvokedItem)?.FullName ?? TbPath.Path;

    private async void NewBClick(object sender, RoutedEventArgs e)
    {
        if (TbPath.Path.GetTagViewModel(_vm.TagsSource) is not { } pathTagModel)
        {
            await ShowMessageDialog.Information(true, "「标签路径」不存在！");
            return;
        }
        var result = NewTagCheck(_vm.Name);
        if (result is not null)
        {
            await ShowMessageDialog.Information(true, result);
            return;
        }
        NewTag(_vm.Name, pathTagModel);
        _vm.Name = "";
    }
    private async void MoveBClick(object sender, RoutedEventArgs e)
    {
        if (TbPath.Path.GetTagViewModel(_vm.TagsSource) is not { } pathTagModel)
        {
            await ShowMessageDialog.Information(true, "「标签路径」不存在！");
            return;
        }
        if (_vm.Name.GetTagViewModel(_vm.TagsSource) is not { } nameTagModel)
        {
            await ShowMessageDialog.Information(true, "「标签名称」不存在！");
            return;
        }
        MoveTag(nameTagModel, pathTagModel);
        _vm.Name = "";
    }
    private async void RenameBClick(object sender, RoutedEventArgs e)
    {
        if (TbPath.Path is "")
        {
            await ShowMessageDialog.Information(true, "未输入希望重命名的标签");
            return;
        }
        if (TbPath.Path.GetTagViewModel(_vm.TagsSource) is not { } pathTagModel)
        {
            await ShowMessageDialog.Information(true, "「标签路径」不存在！");
            return;
        }

        var result = NewTagCheck(_vm.Name);
        if (result is not null)
        {
            await ShowMessageDialog.Information(true, result);
            return;
        }
        RenameTag(_vm.Name, pathTagModel);
        _vm.Name = "";
        TbPath.Path = "";
    }
    private async void DeleteBClick(object sender, RoutedEventArgs e)
    {
        if (TbPath.Path is "")
        {
            await ShowMessageDialog.Information(true, "未输入希望删除的标签");
            return;
        }
        if (TbPath.Path.GetTagViewModel(_vm.TagsSource) is not { } pathTagModel)
        {
            await ShowMessageDialog.Information(true, "「标签路径」不存在！");
            return;
        }
        DeleteTag(pathTagModel);
        _vm.Name = "";
    }
    private void SaveBClick(object sender, RoutedEventArgs e)
    {
        App.Tags = _vm.TagsSource;
        App.SaveTags();
        foreach (var (mode, tagViewModel) in _buffer)
            if (mode)
                App.Relations.NewTag(tagViewModel);
            else App.Relations.DeleteTag(tagViewModel);
        _buffer.Clear();
        App.SaveRelations();
        BSave.IsEnabled = false;
    }

    private async void NewCmClick(object sender, RoutedEventArgs e)
    {
        InputName.Load($"新建子标签 {((TagViewModel)((MenuFlyoutItem)sender).Tag!).Name}", () => NewTagCheck(InputName.Text), FileSystemHelper.InvalidMode.Name);
        if (!await InputName.ShowAsync())
            NewTag(InputName.Text, (TagViewModel)((MenuFlyoutItem)sender).Tag!);
    }
    private async void NewXCmClick(object sender, RoutedEventArgs e)
    {
        InputName.Load("新建根标签", () => NewTagCheck(InputName.Text), FileSystemHelper.InvalidMode.Name);
        if (!await InputName.ShowAsync())
            NewTag(InputName.Text, _vm.TagsSource.TagsTree);
    }
    private void CutCmClick(object sender, RoutedEventArgs e) => ClipBoard = (TagViewModel)((MenuFlyoutItem)sender).Tag!;
    private async void RenameCmClick(object sender, RoutedEventArgs e)
    {
        InputName.Load($"标签重命名 {((TagViewModel)((MenuFlyoutItem)sender).Tag!).Name}", () => NewTagCheck(InputName.Text), FileSystemHelper.InvalidMode.Name);
        if (!await InputName.ShowAsync())
            RenameTag(InputName.Text, (TagViewModel)((MenuFlyoutItem)sender).Tag!);
    }
    private void PasteXCmClick(object sender, RoutedEventArgs e)
    {
        MoveTag(ClipBoard!, _vm.TagsSource.TagsTree);
        ClipBoard = null;
    }

    private void DeleteCmClick(object sender, RoutedEventArgs e) => DeleteTag((TagViewModel)((MenuFlyoutItem)sender).Tag!);

    #endregion

    #region 命令

    private void PasteCmClick(object? parameter)
    {
        MoveTag(ClipBoard!, (TagViewModel)parameter!);
        ClipBoard = null;
    }

    #endregion

    /// <summary>
    /// 暂存关系表的变化
    /// true表示添加，false表示删除
    /// </summary>
    private readonly List<(bool, TagViewModel)> _buffer = new();

    #region 操作

    private void NewTag(string name, TagViewModel path)
    {
        _buffer.Add(new(true, _vm.TagsSource.AddTag(path, name)));
        BSave.IsEnabled = true;
    }

    private async void MoveTag(TagViewModel name, TagViewModel path)
    {
        if (name == path || _vm.TagsSource.TagsDictionary.GetValueOrDefault(name.Id)!.HasChildTag(_vm.TagsSource.TagsDictionary.GetValueOrDefault(path.Id)!))
        {
            await ShowMessageDialog.Information(true, "禁止将标签移动到自己目录下");
            return;
        }
        _vm.TagsSource.MoveTag(name, path);
        BSave.IsEnabled = true;
    }

    private void RenameTag(string name, TagViewModel path)
    {
        _vm.TagsSource.RenameTag(path, name);
        BSave.IsEnabled = true;
    }
    private void DeleteTag(TagViewModel path)
    {
        _vm.TagsSource.DeleteTag(path);
        _buffer.Add(new(false, path));
        BSave.IsEnabled = true;
    }

    private string? NewTagCheck(string name) =>
        name is ""
            ? "标签名称不能为空！"
            : name.GetTagViewModel(_vm.TagsSource) is not null
                ? "与现有标签重名！"
                : null;

    #endregion
}