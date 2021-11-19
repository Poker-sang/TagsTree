using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json.Serialization;
using TagsTreeWinUI3.Interfaces;
using TagsTreeWinUI3.Services;
using TagsTreeWinUI3.Services.ExtensionMethods;

namespace TagsTreeWinUI3.Models
{
    public class FileChanged : IFullName
    {
        public static int Num { get; set; } = 1;

        public int Id { get; }
        public string Name { get; }
        public string Path { get; }
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
        [JsonIgnore] public string FullName => Path + '\\' + Name; //Path必然包含文件路径
        [JsonIgnore] public string PartialPath => this.GetPartialPath(); //Path必然包含文件路径
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
        public FileChanged(string fullName, ChangedType type, string remark = "")
        {
            Id = Num;
            Num++;
            Name = fullName.GetName();
            Path = fullName.GetPath();
            Type = type;
            Remark = remark;
        }
        /// <summary>
        /// 反序列化专用，不要调用该构造器
        /// </summary>
        [JsonConstructor]
        public FileChanged(int id, string name, string path, ChangedType type, string remark = "")
        {
            Num = Math.Max(Num, id + 1);
            Id = id;
            Name = name;
            Path = path;
            Type = type;
            Remark = remark;
        }
    }
}