using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text.Json.Serialization;
using TagsTree.Models;

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

    [JsonIgnore]
    public TagViewModel? Parent
    {
        get => (TagViewModel?)BaseParent;
        set => BaseParent = value;
    }

    public ObservableCollection<TagViewModel> SubTags { get; set; } = new();

    /// <summary>
    /// 反序列化专用，不要调用该构造器
    /// </summary>
    [JsonConstructor]
    public TagViewModel(int id, string name, ObservableCollection<TagViewModel>? subTags = null) : base(id, name) => InitializeSubTags(subTags);

    /// <summary>
    /// 创建新的<see cref="TagViewModel"/>
    /// </summary>
    /// <param name="name">标签名</param>
    /// <param name="parent">标签路径</param>
    /// <param name="subTags">子标签</param>
    public TagViewModel(string name, TagViewModel? parent, ObservableCollection<TagViewModel>? subTags = null) : base(name, parent) => InitializeSubTags(subTags);

    private void InitializeSubTags(ObservableCollection<TagViewModel>? subTags = null)
    {
        SubTags.CollectionChanged += OnSubTagsOnCollectionChanged;
        if (subTags is not null)
            foreach (var subTag in subTags)
                SubTags.Add(subTag);
    }

    /// <summary>
    /// 集合改变时候更改父标签
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    private void OnSubTagsOnCollectionChanged(object? o, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                if (e.NewItems is not null)
                    foreach (TagViewModel newItem in e.NewItems)
                        newItem.Parent = this;
                break;
            case NotifyCollectionChangedAction.Remove:
                if (e.OldItems is not null)
                    foreach (TagViewModel newItem in e.OldItems)
                        newItem.Parent = null;
                break;
            case NotifyCollectionChangedAction.Replace:
            case NotifyCollectionChangedAction.Reset:
                if (e.OldItems is not null)
                    foreach (TagViewModel newItem in e.OldItems)
                        newItem.Parent = null;
                if (e.NewItems is not null)
                    foreach (TagViewModel newItem in e.NewItems)
                        newItem.Parent = this;
                break;
            case NotifyCollectionChangedAction.Move:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(e), e, null);
        }
    }
}
