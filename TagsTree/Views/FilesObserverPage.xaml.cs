using System;
using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.WinUI.UI.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using TagsTree.Interfaces;
using TagsTree.Models;
using TagsTree.Services;
using TagsTree.Services.ExtensionMethods;
using TagsTree.Views.ViewModels;
using WinUI3Utilities;

namespace TagsTree.Views;

public sealed partial class FilesObserverPage : Page, ITypeGetter
{
    public FilesObserverPage() => InitializeComponent();

    public static Type TypeGetter => typeof(FilesObserverPage);

#pragma warning disable CA1822, IDE1006
    // ReSharper disable once InconsistentNaming, MemberCanBeMadeStatic.Local
    private FilesObserverViewModel _vm => Vm;
#pragma warning restore CA1822, IDE1006

    public static FilesObserverViewModel Vm { get; set; } = null!;

    #region 事件处理

    private async void ContextApplyClicked(object sender, RoutedEventArgs e)
    {
        var fileChanged = sender.To<FrameworkElement>().GetDataContext<FileChanged>();
        if (FileViewModel.IsValidPath(fileChanged.Path))
            if (fileChanged.Type is FileChanged.ChangedType.Create)
                Apply(fileChanged, null);
            else if (AppContext.IdFile.Values.FirstOrDefault(f => f.FullName == fileChanged.OldFullName) is { } fileModel)
                Apply(fileChanged, fileModel);
            else
            {
                await ShowContentDialog.Information(true, $"文件列表中不存在：{fileChanged.OldFullName}");
                return;
            }
        else
        {
            await ShowContentDialog.Information(true, $"不在指定文件路径下：{fileChanged.FullName}");
            return;
        }

        _ = _vm.FilesChangedList.Remove(fileChanged);
        AppContext.SaveFiles();
        if (fileChanged.Type is FileChanged.ChangedType.Create or FileChanged.ChangedType.Delete)
            AppContext.SaveRelations();
        Save();
        this.CreateTeachingTip().ShowAndHide("已应用一项并保存");
    }

    private void ContextDeleteClicked(object sender, RoutedEventArgs e)
    {
        _ = _vm.FilesChangedList.Remove(sender.To<FrameworkElement>().GetDataContext<FileChanged>());
        this.CreateTeachingTip().ShowAndHide("已删除一项");
    }

    private void ContextDeleteBeforeClicked(object sender, RoutedEventArgs e)
    {
        if (sender.To<FrameworkElement>().GetDataContext<FileChanged>() == _vm.FilesChangedList[^1])
        {
            _vm.FilesChangedList.Clear();
            return;
        }

        var id = sender.To<FrameworkElement>().GetDataContext<FileChanged>().Id;
        while (_vm.FilesChangedList[0].Id <= id)
            _vm.FilesChangedList.RemoveAt(0);
        this.CreateTeachingTip().ShowAndHide($"已删除序号{id}及之前项");
    }

    private void ContextDeleteAfterClicked(object sender, RoutedEventArgs e)
    {
        if (sender.To<FrameworkElement>().GetDataContext<FileChanged>() == _vm.FilesChangedList[0])
        {
            _vm.FilesChangedList.Clear();
            return;
        }

        var id = sender.To<FrameworkElement>().GetDataContext<FileChanged>().Id;
        while (_vm.FilesChangedList[^1].Id >= id)
            _ = _vm.FilesChangedList.Remove(_vm.FilesChangedList[^1]);
        this.CreateTeachingTip().ShowAndHide($"已删除序号{id}及之后项");
    }

    private void DeleteRangeConfirmClicked(ContentDialog sender, ContentDialogButtonClickEventArgs e)
    {
        var rs = sender.Content.To<RangeSelector>();
        var count = 0;
        for (var i = 0; i < _vm.FilesChangedList.Count;)
            if (rs.RangeStart > _vm.FilesChangedList[i].Id)
                ++i;
            else if (_vm.FilesChangedList[i].Id > rs.RangeEnd)
                break;
            else
            {
                _vm.FilesChangedList.RemoveAt(i);
                ++count;
            }

        this.CreateTeachingTip().ShowAndHide($"已删除{count}项");
    }

    private async void DeleteRangeClicked(object sender, RoutedEventArgs e) => await CdDeleteRange.ShowAsync();

    private async void ApplyAllClicked(object sender, RoutedEventArgs e)
    {
        MergeAll();
        var nameFile = new Dictionary<string, FileModel>();
        foreach (var fileModel in AppContext.IdFile.Values)
            nameFile[fileModel.FullName] = fileModel;
        var deleteList = new List<FileChanged>();
        var invalidExceptions = new List<FileChanged>();
        var notExistExceptions = new List<FileChanged>();
        foreach (var fileChanged in _vm.FilesChangedList)
            if (FileViewModel.IsValidPath(fileChanged.Path))
                if (fileChanged.Type is FileChanged.ChangedType.Create)
                {
                    Apply(fileChanged, null);
                    deleteList.Add(fileChanged);
                }
                else if (nameFile.TryGetValue(fileChanged.OldFullName, out var value))
                {
                    Apply(fileChanged, value);
                    deleteList.Add(fileChanged);
                }
                else
                    notExistExceptions.Add(fileChanged);
            else
                invalidExceptions.Add(fileChanged);
        foreach (var deleteItem in deleteList)
            _ = _vm.FilesChangedList.Remove(deleteItem);
        var exception = "";
        if (invalidExceptions.Count is not 0)
        {
            exception += "*以下文件（夹）不在指定文件路径下：\n";
            exception = invalidExceptions.Aggregate(exception, (current, invalidException) => current + invalidException.FullName + "\n");
        }

        if (notExistExceptions.Count is not 0)
        {
            exception += "*以下文件（夹）不存在于文件列表中：\n";
            exception = notExistExceptions.Aggregate(exception, (current, notExistException) => current + notExistException.OldFullName + "\n");
        }

        if (exception is not "")
            await ShowContentDialog.Information(true, exception);
        AppContext.SaveFiles();
        AppContext.SaveRelations();
        Save();
        this.CreateTeachingTip().ShowAndHide("已全部应用并保存");
    }

    private void MergeAllClicked(object sender, RoutedEventArgs e)
    {
        MergeAll();
        Save();
        this.CreateTeachingTip().ShowAndHide("已全部合并并保存");
    }

    private void DeleteAllClicked(object sender, RoutedEventArgs e)
    {
        ClearAll();
        Save();
        this.CreateTeachingTip().ShowAndHide("已全部清除并保存");
    }

    private void SaveAllClicked(object sender, RoutedEventArgs e)
    {
        Save();
        this.CreateTeachingTip().ShowAndHide("已保存");
    }

    #endregion

    #region 操作

    private static void Apply(FileChanged fileChanged, FileModel? fileModel)
    {
        switch (fileChanged.Type)
        {
            case FileChanged.ChangedType.Create: FileModel.FromFullName(fileChanged.OldFullName).AddNew(); break;
            case FileChanged.ChangedType.Move: fileModel!.MoveOrRename(fileChanged.FullName); break;
            case FileChanged.ChangedType.Rename: fileModel!.MoveOrRename(fileChanged.FullName); break;
            case FileChanged.ChangedType.Delete: fileModel!.Remove(); break;
            default: ThrowHelper.ArgumentOutOfRange(fileChanged); break;
        }
    }

    private void MergeAll()
    {
        var fileChangedMergers = new List<FileChangedMerger>();
        foreach (var fileChanged in _vm.FilesChangedList)
            if (!fileChangedMergers.Any(fileChangedMerger => fileChangedMerger.CanMerge(fileChanged)))
                fileChangedMergers.Add(new(fileChanged));

        ClearAll();
        foreach (var fileChangedMerger in fileChangedMergers)
            switch (fileChangedMerger.GetMergeResult())
            {
                case FileChangedMerger.MergeResult.Move:
                    _vm.FilesChangedList.Add(new(fileChangedMerger.CurrentFullName, FileChanged.ChangedType.Move, fileChangedMerger.OriginalPath));
                    break;
                case FileChangedMerger.MergeResult.Rename:
                    _vm.FilesChangedList.Add(new(fileChangedMerger.CurrentFullName, FileChanged.ChangedType.Rename, fileChangedMerger.OriginalName));
                    break;
                case FileChangedMerger.MergeResult.MoveRename:
                    _vm.FilesChangedList.Add(new($"{fileChangedMerger.CurrentPath}\\{fileChangedMerger.OriginalName}", FileChanged.ChangedType.Move, fileChangedMerger.OriginalPath));
                    _vm.FilesChangedList.Add(new(fileChangedMerger.CurrentFullName, FileChanged.ChangedType.Rename, fileChangedMerger.OriginalName));
                    break;
                case FileChangedMerger.MergeResult.Create:
                    _vm.FilesChangedList.Add(new(fileChangedMerger.CurrentFullName, FileChanged.ChangedType.Create));
                    break;
                case FileChangedMerger.MergeResult.Delete:
                    _vm.FilesChangedList.Add(new(fileChangedMerger.OriginalFullName, FileChanged.ChangedType.Delete));
                    break;
                // MergeResult.Nothing或者不合法数据都不进行操作
                case FileChangedMerger.MergeResult.Nothing:
                default: break;
            }
    }

    private void ClearAll()
    {
        _vm.FilesChangedList.Clear();
        FileChanged.Num = 1;
    }

    private void Save()
    {
        // 或AppContext.FilesChangedList
        FileChanged.Serialize(AppContext.FilesChangedPath, _vm.FilesChangedList);
        _vm.IsSaveEnabled = false;
    }

    #endregion
}
