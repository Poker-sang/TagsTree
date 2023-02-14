using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using TagsTree.Interfaces;
using TagsTree.Models;
using TagsTree.Services;
using TagsTree.Services.ExtensionMethods;
using WinUI3Utilities;

namespace TagsTree.ViewModels;

public class FileViewModel : ObservableObject, IFileModel
{
    public static readonly FileViewModel DefaultFileViewModel = new("");

    public FileModel FileModel { get; }

    /// <summary>
    /// 复制构造，可从后端<see cref="Models.FileModel"/>创建的对象
    /// </summary>
    /// <param name="fileModel">后端FileModel</param>
    /// <param name="tag">如果指定tag，则判断有无tag</param>
    public FileViewModel(FileModel fileModel, TagViewModel? tag = null)
    {
        FileModel = fileModel;
        if (tag is not null)
            Selected = SelectedOriginal = FileModel.HasTag(tag);
    }

    /// <summary>
    /// 虚拟构造（文件对象不存在于<see cref="AppContext.IdFile"/>）
    /// </summary>
    /// <param name="fullName">文件路径</param>
    public FileViewModel(string fullName) => FileModel = new(-1, fullName.GetName(), fullName.GetPath());

    private readonly WeakReference<FileSystemInfo?> _fileSystemInfo = new(null);

    #region FileModel

    public int Id => FileModel.Id is -1 ? ThrowHelper.Exception<int>() : FileModel.Id;
    public string Name => FileModel.Name;
    public string Path => FileModel.Path;
    public string FullName => FileModel.FullName;
    public string Extension => FileModel.Extension;
    public bool Exists => FileModel.Exists;
    public string Tags => FileModel.Tags;
    public IEnumerable<string> PathTags => FileModel.PathTags;
    public string PartialPath => FileModel.PartialPath;
    public bool IsFolder => FileModel.IsFolder;
    public bool PathContains(PathTagModel pathTag) => FileModel.PathContains(pathTag);
    public IEnumerable<TagViewModel> GetAncestorTags(TagViewModel parentTag) => FileModel.GetAncestorTags(parentTag);

    #endregion

    private FileSystemInfo FileSystemInfo
    {
        get
        {
            if (!_fileSystemInfo.TryGetTarget(out var value) || value.FullName != FileModel.FullName)
                _fileSystemInfo.SetTarget(value = FileModel.IsFolder ? new DirectoryInfo(FileModel.FullName) : new FileInfo(FileModel.FullName));
            return value;
        }
    }
    public ImageSource Icon => this.GetIcon();

    public string DateOfModification => FileModel.Exists ? FileSystemInfo.LastWriteTime.ToString(CultureInfo.CurrentCulture) : "";

    public string Size => FileModel is { Exists: true, IsFolder: false } ? FileSystemHelper.CountSize((FileInfo)FileSystemInfo) : "";

    public static bool IsValidPath(string path) => FileModel.IsValidPath(path);

    public void IconChanged() => OnPropertyChanged(nameof(Icon));

    public void TagsChanged() => OnPropertyChanged(nameof(Tags));

    /// <summary>
    /// <see langword="true"/>表示拥有提供的标签<br/>
    /// <see langword="null"/>表示拥有的标签是提供的标签的子标签<br/>
    /// <see langword="false"/>表示既不拥有提供的标签，拥有的标签也不是提供标签的子标签
    /// </summary>
    public bool? Selected { get; private set; }

    /// <summary>
    /// <see langword="true"/>表示拥有提供的标签<br/>
    /// <see langword="null"/>表示拥有的标签是提供的标签的子标签<br/>
    /// <see langword="false"/>表示既不拥有提供的标签，拥有的标签也不是提供标签的子标签
    /// </summary>
    public bool? SelectedOriginal { get; }

    public void SelectedFlip()
    {
        Selected = Selected == SelectedOriginal ? Selected is false : SelectedOriginal;
        OnPropertyChanged(nameof(Selected));
    }
}
