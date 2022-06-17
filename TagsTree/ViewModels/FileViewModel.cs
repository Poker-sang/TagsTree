using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Media.Imaging;
using System.Globalization;
using System.IO;
using TagsTree.Models;
using TagsTree.Services;
using TagsTree.Services.ExtensionMethods;

namespace TagsTree.ViewModels;

[INotifyPropertyChanged]
public partial class FileViewModel : FileModel
{
    /// <summary>
    /// 复制构造，可从后端<see cref="FileModel"/>创建的对象
    /// </summary>
    /// <param name="fileModel">后端FileModel</param>
    /// <param name="tag">如果指定tag，则判断有无tag</param>
    public FileViewModel(FileModel fileModel, TagViewModel? tag = null) : base(fileModel)
    {
        _fileSystemInfo = IsFolder ? new DirectoryInfo(FullName) : new FileInfo(FullName);
        if (tag is not null)
            Selected = SelectedOriginal = HasTag(tag);
    }

    /// <summary>
    /// 虚拟构造，无后端<see cref="FileModel"/>的对象（不存在于<see cref="App.IdFile"/>）
    /// </summary>
    /// <param name="fullName">文件路径</param>
    public FileViewModel(string fullName) : base(fullName) => _fileSystemInfo = IsFolder ? new DirectoryInfo(FullName) : new FileInfo(FullName);

    /// <summary>
    /// 从<see cref="App.IdFile"/>中获取<see cref="FileModel"/>
    /// </summary>
    public FileModel GetFileModel() => App.IdFile[Id];
    public FileModel NewFileModel() => new(this);

    public new void Reload(string fullName)
    {
        base.Reload(fullName);
        GetFileModel().Reload(fullName); //更新UI
        OnPropertyChanged(nameof(Name));
        OnPropertyChanged(nameof(Extension));
        OnPropertyChanged(nameof(Path));
        OnPropertyChanged(nameof(PartialPath));
        OnPropertyChanged(nameof(Icon));
        OnPropertyChanged(nameof(DateOfModification));
        OnPropertyChanged(nameof(Size));
    }

    private readonly FileSystemInfo _fileSystemInfo;

    public void IconChange() => OnPropertyChanged(nameof(Icon));
    public BitmapImage Icon => this.GetIcon();

    public string DateOfModification => Exists ? _fileSystemInfo.LastWriteTime.ToString(CultureInfo.CurrentCulture) : "";

    public string Size => Exists && !IsFolder ? FileSystemHelper.CountSize((FileInfo)_fileSystemInfo) : "";
    public bool Exists => _fileSystemInfo.Exists;

    public static new bool IsValidPath(string path) => FileModel.IsValidPath(path);
    public void TagsUpdated() => OnPropertyChanged(nameof(Tags));

    public bool? Selected { get; private set; }
    public bool? SelectedOriginal { get; }
    public new string Tags => base.Tags;
    public new string PartialPath => base.PartialPath;

    public void SelectedFlip()
    {
        Selected = Selected == SelectedOriginal ? Selected is false : SelectedOriginal;
        OnPropertyChanged(nameof(Selected));
    }
}