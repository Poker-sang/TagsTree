using System.IO;
using System.Text.Json.Serialization;
using TagsTree.Interfaces;
using TagsTree.Services.ExtensionMethods;

namespace TagsTree.Models;

public abstract class FileBase : IFullName
{
    protected FileBase(string name, string path, int id)
    {
        Id = id;
        Name = name;
        Path = path;
    }

    public int Id { get; }

    public string Name { get; protected set; }

    public string Path { get; protected set; }

    /// <remarks>
    /// Path必然包含文件路径
    /// </remarks>
    [JsonIgnore] public string FullName => @$"{Path}\{Name}";

    [JsonIgnore] public string PartialPath => Path.GetPartialPath();

    [JsonIgnore] public bool IsFolder => Directory.Exists(FullName);
}
