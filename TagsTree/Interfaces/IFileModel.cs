using System.Collections.Generic;
using TagsTree.Models;
using TagsTree.ViewModels;

namespace TagsTree.Interfaces;

public interface IFileModel : IFullName
{
    public string PartialPath { get; }
    public bool IsFolder { get; }
    public IEnumerable<TagViewModel> GetAncestorTags(TagViewModel parentTag);
    public string Extension { get; }
    public bool Exists { get; }
    public string Tags { get; }
    public IEnumerable<string> PathTags { get; }
    public bool PathContains(PathTagModel pathTag);
}
