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

    public int Id { get; }
    public string Name { get; private set; }
    public string Path { get; private set; }

    /// <summary>
    /// FileViewModel用的复制构造
    /// </summary>
    /// <param name="fileModel"></param>
    protected FileModel(FileModel fileModel)
    {
        Id = fileModel.Id;
        Name = fileModel.Name;
        Path = fileModel.Path;
    }
    /// <summary>
    /// FileViewModel用的虚拟构造
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
    /// 由虚拟构造的FileViewModel复制而成
    /// </summary>
    /// <param name="fileViewModel"></param>
    public FileModel(FileViewModel fileViewModel)
    {
        Id = Num;
        Num++;
        Name = fileViewModel.Name;
        Path = fileViewModel.Path;
    }
    public void Reload(string fullName)
    {
        FileSystemInfo info = IsFolder ? new DirectoryInfo(fullName) : new FileInfo(fullName);
        Name = info.Name;
        Path = fullName.GetPath();
    }

    protected static bool IsValidPath(string path) => path.Contains(App.AppConfiguration.LibraryPath);

    /// <summary>
    /// null表示拥有标签的上级标签存在本标签
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
    public IEnumerable<TagViewModel> GetRelativeTags(TagViewModel parentTag) => Tags.GetTagViewModels().Where(parentTag.HasChildTag);

    [JsonIgnore] public string Extension => IsFolder ? "文件夹" : Name.Split('.', StringSplitOptions.RemoveEmptyEntries)[^1].ToUpper(CultureInfo.CurrentCulture);
    [JsonIgnore] protected string PartialPath => this.GetPartialPath(); //Path必然包含文件路径
    [JsonIgnore] public string FullName => Path + '\\' + Name; //Path必然包含文件路径
    [JsonIgnore] public string UniqueName => IsFolder + FullName;
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