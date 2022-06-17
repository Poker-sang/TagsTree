using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Text.Json.Serialization;
using TagsTree.Models;
using TagsTree.Services.ExtensionMethods;

namespace TagsTree.ViewModels;

[INotifyPropertyChanged]
public partial class TagViewModel : TagModel
{
    public new string Name
    {
        get => base.Name;
        set
        {
            if (base.Name == value)
                return;
            base.Name = value;
            OnPropertyChanged(nameof(Name));
        }
    }
    public ObservableCollection<TagViewModel> SubTags { get; set; }

    /// <summary>
    /// 反序列化专用，不要调用该构造器
    /// </summary>
    [JsonConstructor]
    public TagViewModel(int id, string name, ObservableCollection<TagViewModel>? subTags = null) : base(id, name) => SubTags = subTags ?? new ObservableCollection<TagViewModel>();
    /// <summary>
    /// 创建新的<see cref="TagViewModel"/>
    /// </summary>
    /// <param name="name">标签名</param>
    /// <param name="path">标签路径</param>
    /// <param name="subTags">子标签</param>
    public TagViewModel(string name, string path, ObservableCollection<TagViewModel>? subTags = null) : base(name, path) => SubTags = subTags ?? new ObservableCollection<TagViewModel>();

    public TagViewModel GetParentTag(TagsTreeDictionary? range = null) => Path.GetTagViewModel(range)!;
}