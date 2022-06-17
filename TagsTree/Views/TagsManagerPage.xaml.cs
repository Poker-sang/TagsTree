using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TagsTree.Services.ExtensionMethods;
using TagsTree.ViewModels;

namespace TagsTree.Views;

[INotifyPropertyChanged]
public partial class TagsManagerPage : Page
{
    public TagsManagerPage()
    {
        Current = this;
        _vm = new TagsManagerViewModel();
        InitializeComponent();
    }
    public static Type TypeGetter => typeof(TagsManagerPage);

    public static TagsManagerPage Current = null!;

    private readonly TagsManagerViewModel _vm;

    [ObservableProperty] private bool _canPaste;

    private TagViewModel? _clipBoard;
    private TagViewModel? ClipBoard
    {
        get => _clipBoard;
        set
        {
            if (Equals(value, _clipBoard))
                return;
            _clipBoard = value;
            CanPaste = CmPPasteX.IsEnabled = value is not null;
        }
    }

    #region 事件处理

    //TODO 条目拖拽
    //private void TvTags_OnDragItemsCompleted(TreeView sender, TreeViewDragItemsCompletedEventArgs e)
    //{
    //    if (e.DropResult is DataPackageOperation.Move)
    //        MoveTag((TagViewModel)e.Items[0], (TagViewModel)e.NewParentItem);
    //}

    private void NameChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs e) => _vm.Name = Regex.Replace(_vm.Name, $@"[{FileSystemHelper.GetInvalidNameChars}]+", "");

    private void Tags_OnItemInvoked(TreeView sender, TreeViewItemInvokedEventArgs e) => TbPath.Path = ((TagViewModel?)e.InvokedItem)?.FullName ?? TbPath.Path;

    private void NewBClick(object sender, RoutedEventArgs e)
    {
        if (TbPath.Path.GetTagViewModel(_vm.TagsSource) is not { } pathTagModel)
        {
            InfoBar.Title = "错误";
            InfoBar.Message = "「标签路径」不存在！";
            InfoBar.Severity = InfoBarSeverity.Error;
            InfoBar.IsOpen = true;
            return;
        }

        var result = NewTagCheck(_vm.Name);
        if (result is not null)
        {
            InfoBar.Title = "错误";
            InfoBar.Message = result;
            InfoBar.Severity = InfoBarSeverity.Error;
            InfoBar.IsOpen = true;
            return;
        }

        NewTag(_vm.Name, pathTagModel);
        _vm.Name = "";
    }
    private void MoveBClick(object sender, RoutedEventArgs e)
    {
        if (TbPath.Path.GetTagViewModel(_vm.TagsSource) is not { } pathTagModel)
        {
            InfoBar.Title = "错误";
            InfoBar.Message = "「标签路径」不存在！";
            InfoBar.Severity = InfoBarSeverity.Error;
            InfoBar.IsOpen = true;
            return;
        }

        if (_vm.Name.GetTagViewModel(_vm.TagsSource) is not { } nameTagModel)
        {
            InfoBar.Title = "错误";
            InfoBar.Message = "「标签名称」不存在！";
            InfoBar.Severity = InfoBarSeverity.Error;
            InfoBar.IsOpen = true;
            return;
        }

        MoveTag(nameTagModel, pathTagModel);
        _vm.Name = "";
    }
    private void RenameBClick(object sender, RoutedEventArgs e)
    {
        if (TbPath.Path is "")
        {
            InfoBar.Title = "错误";
            InfoBar.Message = "未输入希望重命名的标签";
            InfoBar.Severity = InfoBarSeverity.Error;
            InfoBar.IsOpen = true;
            return;
        }

        if (TbPath.Path.GetTagViewModel(_vm.TagsSource) is not { } pathTagModel)
        {
            InfoBar.Title = "错误";
            InfoBar.Message = "「标签路径」不存在！";
            InfoBar.Severity = InfoBarSeverity.Error;
            InfoBar.IsOpen = true;
            return;
        }

        var result = NewTagCheck(_vm.Name);
        if (result is not null)
        {
            InfoBar.Title = "错误";
            InfoBar.Message = result;
            InfoBar.Severity = InfoBarSeverity.Error;
            InfoBar.IsOpen = true;
            return;
        }

        RenameTag(_vm.Name, pathTagModel);
        _vm.Name = "";
        TbPath.Path = "";
    }
    private void DeleteBClick(object sender, RoutedEventArgs e)
    {
        if (TbPath.Path is "")
        {
            InfoBar.Title = "错误";
            InfoBar.Message = "未输入希望删除的标签";
            InfoBar.Severity = InfoBarSeverity.Error;
            InfoBar.IsOpen = true;
            return;
        }

        if (TbPath.Path.GetTagViewModel(_vm.TagsSource) is not { } pathTagModel)
        {
            InfoBar.Title = "错误";
            InfoBar.Message = "「标签路径」不存在！";
            InfoBar.Severity = InfoBarSeverity.Error;
            InfoBar.IsOpen = true;
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
            else
                App.Relations.DeleteTag(tagViewModel);
        _buffer.Clear();
        App.SaveRelations();
        BSave.IsEnabled = false;
        InfoBar.Title = "成功";
        InfoBar.Message = "已保存";
        InfoBar.Severity = InfoBarSeverity.Success;
        InfoBar.IsOpen = true;
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
    private void PasteCmClick(object sender, RoutedEventArgs e)
    {
        MoveTag(ClipBoard!, (TagViewModel)((MenuFlyoutItem)sender).Tag!);
        ClipBoard = null;
    }
    private void PasteXCmClick(object sender, RoutedEventArgs e)
    {
        MoveTag(ClipBoard!, _vm.TagsSource.TagsTree);
        ClipBoard = null;
    }

    private void DeleteCmClick(object sender, RoutedEventArgs e) => DeleteTag((TagViewModel)((MenuFlyoutItem)sender).Tag!);

    #endregion

    /// <summary>
    /// 暂存关系表的变化
    /// <see langword="true"/>表示添加，<see langword="false"/>表示删除
    /// </summary>
    private readonly List<(bool, TagViewModel)> _buffer = new();

    #region 操作

    private void NewTag(string name, TagViewModel path)
    {
        _buffer.Add(new(true, _vm.TagsSource.AddTag(path, name)));
        BSave.IsEnabled = true;
        InfoBar.Title = "成功";
        InfoBar.Message = $"新建标签 {name}";
        InfoBar.Severity = InfoBarSeverity.Success;
        InfoBar.IsOpen = true;
    }

    private void MoveTag(TagViewModel name, TagViewModel path)
    {
        if (name == path || _vm.TagsSource.TagsDictionary.GetValueOrDefault(name.Id)!.HasChildTag(_vm.TagsSource.TagsDictionary.GetValueOrDefault(path.Id)!))
        {
            InfoBar.Title = "错误";
            InfoBar.Message = "禁止将标签移动到自己目录下";
            InfoBar.Severity = InfoBarSeverity.Error;
            InfoBar.IsOpen = true;
            return;
        }

        _vm.TagsSource.MoveTag(name, path);
        BSave.IsEnabled = true;
        InfoBar.Title = "成功";
        InfoBar.Message = $"移动标签 {name.Name}";
        InfoBar.Severity = InfoBarSeverity.Success;
        InfoBar.IsOpen = true;
    }

    private void RenameTag(string name, TagViewModel path)
    {
        _vm.TagsSource.RenameTag(path, name);
        BSave.IsEnabled = true;
        InfoBar.Title = "成功";
        InfoBar.Message = $"重命名标签 {name}";
        InfoBar.Severity = InfoBarSeverity.Success;
        InfoBar.IsOpen = true;
    }
    private void DeleteTag(TagViewModel path)
    {
        _vm.TagsSource.DeleteTag(path);
        _buffer.Add(new(false, path));
        BSave.IsEnabled = true;
        InfoBar.Title = "成功";
        InfoBar.Message = $"删除标签 {path.Name}";
        InfoBar.Severity = InfoBarSeverity.Success;
        InfoBar.IsOpen = true;
    }

    private string? NewTagCheck(string name) =>
        name is ""
            ? "标签名称不能为空！"
            : name.GetTagViewModel(_vm.TagsSource) is not null
                ? "与现有标签重名！"
                : null;

    #endregion
}