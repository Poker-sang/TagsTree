using System.Text.Json.Serialization;
using WinUI3Utilities.Attributes;

namespace TagsTree;

[GenerateConstructor]
public partial record AppConfig
{
    public int Theme { get; set; }

    public string LibraryPath { get; set; } = "";

    public bool PathTagsEnabled { get; set; } = true;

    [AttributeIgnore(typeof(SettingsViewModelAttribute<>))]
    public bool FilesObserverEnabled { get; set; }

    public AppConfig()
    {

    }
}

[JsonSerializable(typeof(string))]
public partial class ConfigSerializeContext : JsonSerializerContext;
