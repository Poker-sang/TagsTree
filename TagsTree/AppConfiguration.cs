using TagsTree.Attributes;

namespace TagsTree;

[GenerateConstructor]
public partial record AppConfiguration
{
    public int Theme { get; set; }
    public string LibraryPath { get; set; }
    public bool PathTagsEnabled { get; set; }
    public bool FilesObserverEnabled { get; set; }
};