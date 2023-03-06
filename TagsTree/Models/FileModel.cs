using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json.Serialization;
using TagsTree.Interfaces;
using TagsTree.Services.ExtensionMethods;
using TagsTree.Views.ViewModels;

namespace TagsTree.Models;

public class FileModel : FileBase, IFileModel
{
    private static int Num { get; set; }

    /// <summary>
    /// <see cref="FileViewModel"/>用的复制构造
    /// </summary>
    /// <param name="fileModel"></param>
    protected FileModel(FileModel fileModel) : this(fileModel.Id, fileModel.Name, fileModel.Path)
    {
    }

    /// <summary>
    /// <see cref="FileViewModel"/>用的虚拟构造
    /// </summary>
    /// <param name="fullName"></param>
    protected FileModel(string fullName) : this(-1, fullName.GetName(), fullName.GetPath())
    {
    }

    /// <summary>
    /// 反序列化专用，不要从外部调用该构造器
    /// </summary>
    [JsonConstructor]
    public FileModel(int id, string name, string path) : base(name, path, id) => Num = Math.Max(Num, id + 1);

    /// <summary>
    /// 从<paramref name="fullName"/>构造的<see cref="FileModel"/>，并生成<see cref="FileModel.Id"/>
    /// </summary>
    public static FileModel FromFullName(string fullName)
    {
        var ret = new FileModel(Num, fullName.GetName(), fullName.GetPath());
        ++Num;
        return ret;
    }

    public void Reload(string fullName)
    {
        FileSystemInfo info = IsFolder ? new DirectoryInfo(fullName) : new FileInfo(fullName);
        Name = info.Name;
        Path = fullName.GetPath();
    }

    public static bool IsValidPath(string path) => path.Contains(AppContext.AppConfig.LibraryPath);

    /// <summary>
    /// <see langword="null"/>表示拥有的标签是<paramref name="tag"/>的子标签
    /// </summary>
    /// <param name="tag"></param>
    /// <returns></returns>
    public bool? HasTag(TagViewModel tag)
    {
        foreach (var tagPossessed in Tags.GetTagViewModels())
            if (tag == tagPossessed)
                return true;
            else if (tag.HasChildTag(tagPossessed))
                return null;
        return false;
    }

    public IEnumerable<TagViewModel> GetAncestorTags(TagViewModel parentTag) => Tags.GetTagViewModels().Where(parentTag.HasChildTag);

    [JsonIgnore] public string Extension => IsFolder ? "文件夹" : Name.Split('.', StringSplitOptions.RemoveEmptyEntries)[^1].ToUpper(CultureInfo.CurrentCulture);

    [JsonIgnore] public bool Exists => Directory.Exists(FullName) || File.Exists(FullName);
    [JsonIgnore]
    public string Tags
    {
        get
        {
            var tags = AppContext.Relations.GetTags(Id).Select(tag => tag.Name).Aggregate("", (current, tag) => current + " " + tag);
            return tags is "" ? "" : tags[1..];
        }
    }
    [JsonIgnore] public IEnumerable<string> PathTags => PartialPath is "..." ? Enumerable.Empty<string>() : PartialPath[4..].Split('\\', StringSplitOptions.RemoveEmptyEntries); //PartialPath不会是空串

    public bool PathContains(PathTagModel pathTag) => PartialPath is not "..." && (PartialPath[3..] + "\\").Contains($"\\{pathTag.Name}\\");
}
