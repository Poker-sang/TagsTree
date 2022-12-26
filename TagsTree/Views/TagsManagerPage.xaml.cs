using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TagsTree.Interfaces;
using TagsTree.Services.ExtensionMethods;
using TagsTree.ViewModels;

namespace TagsTree.Views;

[INotifyPropertyChanged]
public partial class TagsManagerPage : Page, ITypeGetter
{
    public TagsManagerPage()
    {
        Current = this;
        _vm = new();
        InitializeComponent();
    }
    public static Type TypeGetter => typeof(TagsManagerPage);

    public static TagsManagerPage Current { get; private set; } = null!;

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
            CanPaste = value is not null;
        }
    }

    #region 事件处理

    private TagViewModel? _tempPath;

    private void OnDragItemsCompleted(TreeView sender, TreeViewDragItemsCompletedEventArgs e)
    {
        if ((e.NewParentItem as TagViewModel) == _tempPath)
            InfoBarShow($"移动标签 {((TagViewModel)e.Items[0]).Name} 到原位置", InfoBarSeverity.Informational);
        else
        {
            BSave.IsEnabled = true;
            InfoBarShow($"移动标签 {((TagViewModel)e.Items[0]).Name}", InfoBarSeverity.Success);
        }

        _tempPath = null;
    }

    private void OnDragItemsStarting(TreeView sender, TreeViewDragItemsStartingEventArgs e) => _tempPath = ((TagViewModel)e.Items[0]).Parent;

    private void NameChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs e) => _vm.Name = Regex.Replace(_vm.Name, $@"[{FileSystemHelper.GetInvalidNameChars}]+", "");

    private void Tags_OnItemInvoked(TreeView sender, TreeViewItemInvokedEventArgs e) => TbPath.Path = (e.InvokedItem as TagViewModel)?.FullName ?? TbPath.Path;

    private void NewClick(object sender, RoutedEventArgs e)
    {
        if (ExistenceCheck(TbPath.Path, "标签路径") is not { } pathTagModel)
            return;

        var result = NewTagCheck(_vm.Name);
        if (result is not null)
        {
            InfoBarShow(result, InfoBarSeverity.Error);
            return;
        }

        NewTag(_vm.Name, pathTagModel);
        _vm.Name = "";
    }

    private void MoveClick(object sender, RoutedEventArgs e)
    {
        if (ExistenceCheck(TbPath.Path, "标签路径") is not { } pathTagModel)
            return;

        if (ExistenceCheck(_vm.Name, "标签名称") is not { } nameTagModel)
            return;

        MoveTag(nameTagModel, pathTagModel);
        _vm.Name = "";
    }

    private void RenameClick(object sender, RoutedEventArgs e)
    {
        if (TbPath.Path is "")
        {
            InfoBarShow("未输入希望重命名的标签！", InfoBarSeverity.Error);
            return;
        }

        if (ExistenceCheck(TbPath.Path, "标签路径") is not { } pathTagModel)
            return;

        var result = NewTagCheck(_vm.Name);
        if (result is not null)
        {
            InfoBarShow(result, InfoBarSeverity.Error);
            return;
        }

        RenameTag(_vm.Name, pathTagModel);
        _vm.Name = "";
        TbPath.Path = "";
    }

    private void DeleteClick(object sender, RoutedEventArgs e)
    {
        if (TbPath.Path is "")
        {
            InfoBarShow("未输入希望删除的标签！", InfoBarSeverity.Error);
            return;
        }

        if (ExistenceCheck(TbPath.Path, "标签路径") is not { } pathTagModel)
            return;

        DeleteTag(pathTagModel);
        _vm.Name = "";
    }

    private async void SaveClick(object sender, RoutedEventArgs e)
    {
        await Task.Yield();
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
        InfoBarShow("已保存", InfoBarSeverity.Success);
    }

    private async void ContextNewClick(object sender, RoutedEventArgs e)
    {
        InputName.Load($"新建子标签 {sender.GetTag<TagViewModel>().Name}", cd => NewTagCheck(cd.Text), FileSystemHelper.InvalidMode.Name);
        if (!await InputName.ShowAsync())
            NewTag(InputName.Text, sender.GetTag<TagViewModel>());
    }

    private async void ContextNewXClick(object sender, RoutedEventArgs e)
    {
        InputName.Load("新建根标签", cd => NewTagCheck(cd.Text), FileSystemHelper.InvalidMode.Name);
        if (!await InputName.ShowAsync())
            NewTag(InputName.Text, _vm.TagsSource.TagsTree);
    }

    private void ContextCutClick(object sender, RoutedEventArgs e) => ClipBoard = sender.GetTag<TagViewModel>();

    private async void ContextRenameClick(object sender, RoutedEventArgs e)
    {
        InputName.Load($"标签重命名 {sender.GetTag<TagViewModel>().Name}", cd => NewTagCheck(cd.Text), FileSystemHelper.InvalidMode.Name);
        if (!await InputName.ShowAsync())
            RenameTag(InputName.Text, sender.GetTag<TagViewModel>());
    }

    private void ContextPasteClick(object sender, RoutedEventArgs e) => MoveTag(ClipBoard!, sender.GetTag<TagViewModel>());

    private void ContextPasteXClick(object sender, RoutedEventArgs e) => MoveTag(ClipBoard!, _vm.TagsSource.TagsTree);

    private void ContextDeleteClick(object sender, RoutedEventArgs e) => DeleteTag(sender.GetTag<TagViewModel>());

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
        BSave.IsEnabled = true;
        InfoBarShow($"新建标签 {name}", InfoBarSeverity.Success);
    }

    private void MoveTag(TagViewModel name, TagViewModel path)
    {
        if (path == name.Parent)
        {
            InfoBarShow($"移动标签 {name.Name} 到原位置", InfoBarSeverity.Informational);
            return;
        }

        if (name == path || _vm.TagsSource.TagsDictionary.GetValueOrDefault(name.Id)!.HasChildTag(_vm.TagsSource.TagsDictionary.GetValueOrDefault(path.Id)!))
        {
            InfoBarShow("禁止将标签移动到自己目录下！", InfoBarSeverity.Error);
            return;
        }

        ClipBoard = null;
        _vm.TagsSource.MoveTag(name, path);
        BSave.IsEnabled = true;
        InfoBarShow($"移动标签 {name.Name}", InfoBarSeverity.Success);
    }

    private void RenameTag(string name, TagViewModel path)
    {
        _vm.TagsSource.RenameTag(path, name);
        BSave.IsEnabled = true;
        InfoBarShow($"重命名标签 {name}", InfoBarSeverity.Success);
    }

    private void DeleteTag(TagViewModel path)
    {
        _vm.TagsSource.DeleteTag(path);
        _buffer.Add(new(false, path));
        BSave.IsEnabled = true;
        InfoBar.Title = "成功";
        InfoBarShow($"删除标签 {path.Name}", InfoBarSeverity.Success);
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
            InfoBarShow($"「{label}」不存在！", InfoBarSeverity.Error);
        return pathTagModel;
    }

    private void InfoBarShow(string message, InfoBarSeverity severity)
    {
        InfoBar.Title = severity switch
        {
            InfoBarSeverity.Informational => "提示",
            InfoBarSeverity.Success => "成功",
            InfoBarSeverity.Warning => "警告",
            InfoBarSeverity.Error => "错误",
            _ => throw new ArgumentOutOfRangeException(nameof(severity), severity, null)
        };
        InfoBar.Message = message;
        InfoBar.Severity = severity;
        InfoBar.IsOpen = true;
    }

    #endregion
}
