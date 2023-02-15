using WinUI3Utilities.Attributes;

namespace TagsTree;

[GenerateConstructor]
public partial record AppConfig
{
    public int Theme { get; set; }
    
    public string LibraryPath { get; set; } = "";

    public bool PathTagsEnabled { get; set; } = true;

    [SettingsViewModelExclusion]
    public bool FilesObserverEnabled { get; set; }

    public AppConfig()
    {

    }
}
