using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json.Serialization;
using TagsTree.Interfaces;
using TagsTree.Services.ExtensionMethods;
using TagsTree.ViewModels;

namespace TagsTree.Models;

public class FileModel : IFullName
{
    private static int Num { get; set; }

    public int Id { get; private set; }
    public string Name { get; private set; }
    public string Path { get; private set; }

    /// <summary>
    /// <see cref="FileViewModel"/>用的复制构造
    /// </summary>
    /// <param name="fileModel"></param>
    protected FileModel(FileModel fileModel)
    {
        Id = fileModel.Id;
        Name = fileModel.Name;
        Path = fileModel.Path;
    }

    /// <summary>
    /// <see cref="FileViewModel"/>用的虚拟构造
    /// </summary>
    /// <param name="fullName"></param>
    protected FileModel(string fullName)
    {
        Id = -1;
        Name = fullName.GetName();
        Path = fullName.GetPath();
    }

    /// <summary>
    /// 反序列化专用，不要调用该构造器
    /// </summary>
    [JsonConstructor]
    public FileModel(int id, string name, string path)
    {
        Num = Math.Max(Num, id + 1);
        Id = id;
        Name = name;
        Path = path;
    }

    /// <summary>
    /// 由虚拟构造的<see cref="FileViewModel"/>构成的<see cref="FileModel"/>，并生成Id
    /// </summary>
    public FileModel GenerateAndUseId()
    {
        Id = Num;
        Num++;
        return this;
    }

    public void Reload(string fullName)
    {
        FileSystemInfo info = IsFolder ? new DirectoryInfo(fullName) : new FileInfo(fullName);
        Name = info.Name;
        Path = fullName.GetPath();
    }

    protected static bool IsValidPath(string path) => path.Contains(App.AppConfiguration.LibraryPath);

    /// <summary>
    /// <see langword="null"/>表示拥有的标签是<paramref name="tag"/>的子标签
    /// </summary>
    /// <param name="tag"></param>
    /// <returns></returns>
    protected bool? HasTag(TagViewModel tag)
    {
        foreach (var tagPossessed in Tags.GetTagViewModels())
            if (tag == tagPossessed)
                return true;
            else if (tag.HasChildTag(tagPossessed))
                return null;
        return false;
    }

    /// <summary>
    /// 获取
    /// </summary>
    /// <param name="parentTag"></param>
    /// <returns></returns>
    public IEnumerable<TagViewModel> GetAncestorTags(TagViewModel parentTag) => Tags.GetTagViewModels().Where(parentTag.HasChildTag);

    [JsonIgnore] public string Extension => IsFolder ? "文件夹" : Name.Split('.', StringSplitOptions.RemoveEmptyEntries)[^1].ToUpper(CultureInfo.CurrentCulture);
    /// <remarks>
    /// Path必然包含文件路径
    /// </remarks>
    [JsonIgnore] protected string PartialPath => this.GetPartialPath();
    /// <remarks>
    /// Path必然包含文件路径
    /// </remarks>
    [JsonIgnore] public string FullName => Path + '\\' + Name;
    [JsonIgnore] public bool IsFolder => Directory.Exists(FullName);
    [JsonIgnore]
    protected string Tags
    {
        get
        {
            var tags = App.Relations.GetTags(this).Select(tag => tag.Name).Aggregate("", (current, tag) => current + " " + tag);
            return tags is "" ? "" : tags[1..];
        }
    }
    [JsonIgnore] public IEnumerable<string> PathTags => PartialPath is "..." ? Enumerable.Empty<string>() : PartialPath[4..].Split('\\', StringSplitOptions.RemoveEmptyEntries); //PartialPath不会是空串

    public bool PathContains(PathTagModel pathTag) => PartialPath is not "..." && (PartialPath[3..] + "\\").Contains($"\\{pathTag.Name}\\");
}
