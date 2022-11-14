using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json.Serialization;
using TagsTree.Interfaces;
using TagsTree.Services;
using TagsTree.Services.ExtensionMethods;

namespace TagsTree.Models;

public class FileChanged : FileBase
{
    public static int Num { get; set; } = 1;

    public ChangedType Type { get; }
    public string Remark { get; }

    public string DisplayType => Type switch
    {
        ChangedType.Create => "Create",
        ChangedType.Move => "Move",
        ChangedType.Rename => "Rename",
        ChangedType.Delete => "Delete",
        _ => ""
    };
    [JsonIgnore]
    public string DisplayRemark => Type switch
    {
        ChangedType.Move => "旧路径：" + Remark.GetPartialPath(),
        ChangedType.Rename => "旧名称：" + Remark,
        _ => Remark
    };
    [JsonIgnore]
    public string OldFullName => Type switch
    {
        ChangedType.Move => $"{Remark}\\{Name}",
        ChangedType.Rename => $"{Path}\\{Remark}",
        _ => FullName
    };
    [JsonIgnore] public bool IsFolder => Directory.Exists(FullName);

    public static ObservableCollection<FileChanged> Deserialize(string path) => Serialization.Deserialize<ObservableCollection<FileChanged>>(path);
    public static void Serialize(string path, ObservableCollection<FileChanged> collection) => Serialization.Serialize(path, collection);

    public enum ChangedType
    {
        Create = 0,
        Move = 1,
        Rename = 2,
        Delete = 3
    }

    /// <param name="fullName"></param>
    /// <param name="type"></param>
    /// <param name="remark">Move留旧路径，Rename留旧名称</param>
    public FileChanged(string fullName, ChangedType type, string remark = "") : base(fullName.GetName(), fullName.GetPath(), Num)
    {
        Num++;
        Type = type;
        Remark = remark;
    }
    /// <summary>
    /// 反序列化专用，不要调用该构造器
    /// </summary>
    [JsonConstructor]
    public FileChanged(int id, string name, string path, ChangedType type, string remark = "") : base(name, path, id)
    {
        Num = Math.Max(Num, id + 1);
        Type = type;
        Remark = remark;
    }
}
