using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using TagsTree.Interfaces;
using TagsTree.Services;
using TagsTree.Services.ExtensionMethods;
using TagsTree.Views.ViewModels;
using WinUI3Utilities;

namespace TagsTree.Views;

public partial class TagsManagerPage : Page, ITypeGetter
{
    public TagsManagerPage()
    {
        Current = this;
        InitializeComponent();
    }

    public static Type TypeGetter => typeof(TagsManagerPage);

    public static TagsManagerPage Current { get; private set; } = null!;

    private readonly TagsManagerViewModel _vm = new();

    #region 事件处理

    private TagViewModel? _tempPath;

    private void OnDragItemsCompleted(TreeView sender, TreeViewDragItemsCompletedEventArgs e)
    {
        if ((e.NewParentItem.To<TagViewModel>()) == _tempPath)
            SnackBarHelper.Show($"移动标签 {e.Items[0].To<TagViewModel>().Name} 到原位置", Severity.Information);
        else
        {
            _vm.IsSaveEnabled = true;
            SnackBarHelper.Show($"移动标签 {e.Items[0].To<TagViewModel>().Name}");
        }

        _tempPath = null;
    }

    private void OnDragItemsStarting(TreeView sender, TreeViewDragItemsStartingEventArgs e) => _tempPath = e.Items[0].To<TagViewModel>().Parent;

    private void NameChanged(object sender, TextChangedEventArgs e) => _vm.Name = Regex.Replace(_vm.Name, $@"[{FileSystemHelper.GetInvalidNameChars}]+", "");

    private void TagsOnItemInvoked(TreeView sender, TreeViewItemInvokedEventArgs e) => _vm.Path = (e.InvokedItem as TagViewModel)?.FullName ?? _vm.Path;

    private void NewTapped(object sender, TappedRoutedEventArgs e)
    {
        if (ExistenceCheck(_vm.Path, "标签路径") is not { } pathTagModel)
            return;

        var result = NewTagCheck(_vm.Name);
        if (result is not null)
        {
            SnackBarHelper.Show(result, Severity.Error);
            return;
        }

        NewTag(_vm.Name, pathTagModel);
        _vm.Name = "";
    }

    private void MoveTapped(object sender, TappedRoutedEventArgs e)
    {
        if (ExistenceCheck(_vm.Path, "标签路径") is not { } pathTagModel)
            return;

        if (ExistenceCheck(_vm.Name, "标签名称") is not { } nameTagModel)
            return;

        MoveTag(nameTagModel, pathTagModel);
        _vm.Name = "";
    }

    private void RenameTapped(object sender, TappedRoutedEventArgs e)
    {
        if (_vm.Path is "")
        {
            SnackBarHelper.Show("未输入希望重命名的标签！", Severity.Error);
            return;
        }

        if (ExistenceCheck(_vm.Path, "标签路径") is not { } pathTagModel)
            return;

        var result = NewTagCheck(_vm.Name);
        if (result is not null)
        {
            SnackBarHelper.Show(result, Severity.Error);
            return;
        }

        RenameTag(_vm.Name, pathTagModel);
        _vm.Name = "";
        _vm.Path = "";
    }

    private void DeleteTapped(object sender, TappedRoutedEventArgs e)
    {
        if (_vm.Path is "")
        {
            SnackBarHelper.Show("未输入希望删除的标签！", Severity.Error);
            return;
        }

        if (ExistenceCheck(_vm.Path, "标签路径") is not { } pathTagModel)
            return;

        DeleteTag(pathTagModel);
        _vm.Name = "";
    }

    private async void SaveTapped(object sender, TappedRoutedEventArgs e)
    {
        await Task.Yield();
        AppContext.Tags = _vm.TagsSource;
        AppContext.SaveTags();
        foreach (var (mode, tagViewModel) in _buffer)
            if (mode)
                AppContext.Relations.NewTag(tagViewModel);
            else
                AppContext.Relations.DeleteTag(tagViewModel);
        _buffer.Clear();
        AppContext.SaveRelations();
        _vm.IsSaveEnabled = false;
        SnackBarHelper.Show("已保存");
    }

    private async void ContextNewTapped(object sender, TappedRoutedEventArgs e)
    {
        InputName.Load($"新建子标签 {sender.GetTag<TagViewModel>().Name}", cd => NewTagCheck(cd.Text), FileSystemHelper.InvalidMode.Name);
        if (!await InputName.ShowAsync())
            NewTag(InputName.Text, sender.GetTag<TagViewModel>());
    }

    private async void RootContextNewTapped(object sender, TappedRoutedEventArgs e)
    {
        InputName.Load("新建根标签", cd => NewTagCheck(cd.Text), FileSystemHelper.InvalidMode.Name);
        if (!await InputName.ShowAsync())
            NewTag(InputName.Text, _vm.TagsSource.TagsTree);
    }

    private void ContextCutTapped(object sender, TappedRoutedEventArgs e) => _vm.ClipBoard = sender.GetTag<TagViewModel>();

    private async void ContextRenameTapped(object sender, TappedRoutedEventArgs e)
    {
        InputName.Load($"标签重命名 {sender.GetTag<TagViewModel>().Name}", cd => NewTagCheck(cd.Text), FileSystemHelper.InvalidMode.Name);
        if (!await InputName.ShowAsync())
            RenameTag(InputName.Text, sender.GetTag<TagViewModel>());
    }

    private void ContextPasteTapped(object sender, TappedRoutedEventArgs e) => MoveTag(_vm.ClipBoard!, sender.GetTag<TagViewModel>());

    private void RootContextPasteTapped(object sender, TappedRoutedEventArgs e) => MoveTag(_vm.ClipBoard!, _vm.TagsSource.TagsTree);

    private void ContextDeleteTapped(object sender, TappedRoutedEventArgs e) => DeleteTag(sender.GetTag<TagViewModel>());

    #endregion

    /// <summary>
    /// 暂存关系表的变化<br/>
    /// <see langword="true"/>表示添加，<see langword="false"/>表示删除
    /// </summary>
    private readonly List<(bool, TagViewModel)> _buffer = new();

    #region 操作

    private void NewTag(string name, TagViewModel path)
    {
        _buffer.Add(new(true, _vm.TagsSource.AddTag(path, name)));
        _vm.IsSaveEnabled = true;
        SnackBarHelper.Show($"新建标签 {name}");
    }

    private void MoveTag(TagViewModel name, TagViewModel path)
    {
        if (path == name.Parent)
        {
            SnackBarHelper.Show($"移动标签 {name.Name} 到原位置", Severity.Information);
            return;
        }

        if (name == path || _vm.TagsSource.TagsDictionary.GetValueOrDefault(name.Id)!.HasChildTag(_vm.TagsSource.TagsDictionary.GetValueOrDefault(path.Id)!))
        {
            SnackBarHelper.Show("禁止将标签移动到自己目录下！", Severity.Error);
            return;
        }

        _vm.ClipBoard = null;
        _vm.TagsSource.MoveTag(name, path);
        _vm.IsSaveEnabled = true;
        SnackBarHelper.Show($"移动标签 {name.Name}");
    }

    private void RenameTag(string name, TagViewModel path)
    {
        _vm.TagsSource.RenameTag(path, name);
        _vm.IsSaveEnabled = true;
        SnackBarHelper.Show($"重命名标签 {name}");
    }

    private void DeleteTag(TagViewModel path)
    {
        _vm.TagsSource.DeleteTag(path);
        _buffer.Add(new(false, path));
        _vm.IsSaveEnabled = true;
        SnackBarHelper.Show($"删除标签 {path.Name}");
    }

    private string? NewTagCheck(string name)
        => name is ""
            ? "标签名称不能为空！"
            : name.GetTagViewModel(_vm.TagsSource) is not null
                ? "与现有标签重名！"
                : null;

    private TagViewModel? ExistenceCheck(string path, string label)
    {
        var pathTagModel = path.GetTagViewModel(_vm.TagsSource);
        if (pathTagModel is null)
            SnackBarHelper.Show($"「{label}」不存在！", Severity.Error);
        return pathTagModel;
    }

    #endregion
}
