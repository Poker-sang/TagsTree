using System.Text.Json.Serialization;

namespace TagsTree.Models;

public class FileModel : FileModelMiddleware
{
    /// <summary>
    /// 反序列化专用，不要从外部调用该构造器
    /// </summary>
    [JsonConstructor]
    public FileModel(int id, string name, string path) : base(id, name, path)
    {
    }
}
