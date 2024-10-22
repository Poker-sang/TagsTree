using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using TagsTree.Interfaces;
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

    // TODO: TextBox绑定更新慢
    public readonly TagsManagerViewModel Vm = new();

    #region 事件处理

    private TagViewModel? _tempPath;

    private void OnDragItemsCompleted(TreeView sender, TreeViewDragItemsCompletedEventArgs e)
    {
        if (e.NewParentItem.To<TagViewModel>() == _tempPath)
            this.CreateTeachingTip().ShowAndHide($"移动标签 {e.Items[0].To<TagViewModel>().Name} 到原位置", TeachingTipSeverity.Information);
        else
        {
            Vm.IsSaveEnabled = true;
            this.CreateTeachingTip().ShowAndHide($"移动标签 {e.Items[0].To<TagViewModel>().Name}");
        }

        _tempPath = null;
    }

    private void OnDragItemsStarting(TreeView sender, TreeViewDragItemsStartingEventArgs e) => _tempPath = e.Items[0].To<TagViewModel>().Parent;

    private void NameChanged(object sender, TextChangedEventArgs e) => Vm.Name = Regex.Replace(Vm.Name, $@"[{FileSystemHelper.GetInvalidNameChars}]+", "");

    private void TagsOnItemInvoked(TreeView sender, TreeViewItemInvokedEventArgs e) => Vm.Path = (e.InvokedItem as TagViewModel)?.FullName ?? Vm.Path;

    private void NewClicked(object sender, RoutedEventArgs e)
    {
        if (ExistenceCheck(Vm.Path, "标签路径") is not { } pathTagModel)
            return;

        var result = NewTagCheck(Vm.Name);
        if (result is not null)
        {
            this.CreateTeachingTip().ShowAndHide(result, TeachingTipSeverity.Error);
            return;
        }

        NewTag(Vm.Name, pathTagModel);
        Vm.Name = "";
    }

    private void MoveClicked(object sender, RoutedEventArgs e)
    {
        if (ExistenceCheck(Vm.Path, "标签路径") is not { } pathTagModel)
            return;

        if (ExistenceCheck(Vm.Name, "标签名称") is not { } nameTagModel)
            return;

        MoveTag(nameTagModel, pathTagModel);
        Vm.Name = "";
    }

    private void RenameClicked(object sender, RoutedEventArgs e)
    {
        if (Vm.Path is "")
        {
            this.CreateTeachingTip().ShowAndHide("未输入希望重命名的标签！", TeachingTipSeverity.Error);
            return;
        }

        if (ExistenceCheck(Vm.Path, "标签路径") is not { } pathTagModel)
            return;

        var result = NewTagCheck(Vm.Name);
        if (result is not null)
        {
            this.CreateTeachingTip().ShowAndHide(result, TeachingTipSeverity.Error);
            return;
        }

        RenameTag(Vm.Name, pathTagModel);
        Vm.Name = "";
        Vm.Path = "";
    }

    private void DeleteClicked(object sender, RoutedEventArgs e)
    {
        if (Vm.Path is "")
        {
            this.CreateTeachingTip().ShowAndHide("未输入希望删除的标签！", TeachingTipSeverity.Error);
            return;
        }

        if (ExistenceCheck(Vm.Path, "标签路径") is not { } pathTagModel)
            return;

        DeleteTag(pathTagModel);
        Vm.Name = "";
    }

    private async void SaveClicked(object sender, RoutedEventArgs e)
    {
        await Task.Yield();
        AppContext.Tags = Vm.TagsSource;
        AppContext.SaveTags();
        foreach (var (mode, tagViewModel) in _buffer)
            if (mode)
                AppContext.Relations.NewTag(tagViewModel);
            else
                AppContext.Relations.DeleteTag(tagViewModel);
        _buffer.Clear();
        AppContext.SaveRelations();
        Vm.IsSaveEnabled = false;
        this.CreateTeachingTip().ShowAndHide("已保存");
    }

    private async void ContextNewClicked(object sender, RoutedEventArgs e)
    {
        InputName.Load($"新建子标签 {sender.To<FrameworkElement>().GetTag<TagViewModel>().Name}", cd => NewTagCheck(cd.Text), FileSystemHelper.InvalidMode.Name);
        if (!await InputName.ShowAsync())
            NewTag(InputName.Text, sender.To<FrameworkElement>().GetTag<TagViewModel>());
    }

    private async void RootContextNewClicked(object sender, RoutedEventArgs e)
    {
        InputName.Load("新建根标签", cd => NewTagCheck(cd.Text), FileSystemHelper.InvalidMode.Name);
        if (!await InputName.ShowAsync())
            NewTag(InputName.Text, Vm.TagsSource.TagsTree);
    }

    private void ContextCutClicked(object sender, RoutedEventArgs e) => Vm.ClipBoard = sender.To<FrameworkElement>().GetTag<TagViewModel>();

    private async void ContextRenameClicked(object sender, RoutedEventArgs e)
    {
        InputName.Load($"标签重命名 {sender.To<FrameworkElement>().GetTag<TagViewModel>().Name}", cd => NewTagCheck(cd.Text), FileSystemHelper.InvalidMode.Name);
        if (!await InputName.ShowAsync())
            RenameTag(InputName.Text, sender.To<FrameworkElement>().GetTag<TagViewModel>());
    }

    private void ContextPasteClicked(object sender, RoutedEventArgs e) => MoveTag(Vm.ClipBoard!, sender.To<FrameworkElement>().GetTag<TagViewModel>());

    private void RootContextPasteClicked(object sender, RoutedEventArgs e) => MoveTag(Vm.ClipBoard!, Vm.TagsSource.TagsTree);

    private void ContextDeleteClicked(object sender, RoutedEventArgs e) => DeleteTag(sender.To<FrameworkElement>().GetTag<TagViewModel>());

    #endregion

    /// <summary>
    /// 暂存关系表的变化<br/>
    /// <see langword="true"/>表示添加，<see langword="false"/>表示删除
    /// </summary>
    private readonly List<(bool, TagViewModel)> _buffer = [];

    #region 操作

    private void NewTag(string name, TagViewModel path)
    {
        _buffer.Add(new(true, Vm.TagsSource.AddTag(path, name)));
        Vm.IsSaveEnabled = true;
        this.CreateTeachingTip().ShowAndHide($"新建标签 {name}");
    }

    private void MoveTag(TagViewModel name, TagViewModel path)
    {
        if (path == name.Parent)
        {
            this.CreateTeachingTip().ShowAndHide($"移动标签 {name.Name} 到原位置", TeachingTipSeverity.Information);
            return;
        }

        if (name == path || Vm.TagsSource.TagsDictionary.GetValueOrDefault(name.Id)!.HasChildTag(Vm.TagsSource.TagsDictionary.GetValueOrDefault(path.Id)!))
        {
            this.CreateTeachingTip().ShowAndHide("禁止将标签移动到自己目录下！", TeachingTipSeverity.Error);
            return;
        }

        Vm.ClipBoard = null;
        Vm.TagsSource.MoveTag(name, path);
        Vm.IsSaveEnabled = true;
        this.CreateTeachingTip().ShowAndHide($"移动标签 {name.Name}");
    }

    private void RenameTag(string name, TagViewModel path)
    {
        Vm.TagsSource.RenameTag(path, name);
        Vm.IsSaveEnabled = true;
        this.CreateTeachingTip().ShowAndHide($"重命名标签 {name}");
    }

    private void DeleteTag(TagViewModel path)
    {
        Vm.TagsSource.DeleteTag(path);
        _buffer.Add(new(false, path));
        Vm.IsSaveEnabled = true;
        this.CreateTeachingTip().ShowAndHide($"删除标签 {path.Name}");
    }

    private string? NewTagCheck(string name)
        => name is ""
            ? "标签名称不能为空！"
            : name.GetTagViewModel(Vm.TagsSource) is not null
                ? "与现有标签重名！"
                : null;

    private TagViewModel? ExistenceCheck(string path, string label)
    {
        var pathTagModel = path.GetTagViewModel(Vm.TagsSource);
        if (pathTagModel is null)
            this.CreateTeachingTip().ShowAndHide($"「{label}」不存在！", TeachingTipSeverity.Error);
        return pathTagModel;
    }

    #endregion
}
