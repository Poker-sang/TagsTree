using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using TagsTree.Models;
using TagsTree.Services.ExtensionMethods;

namespace TagsTree.ViewModels
{
    public class TagViewModel : TagModel, INotifyPropertyChanged
    {
        public new string Name
        {
            get => base.Name;
            set
            {
                if (base.Name == value) return;
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
        public TagViewModel(string name, string path, ObservableCollection<TagViewModel>? subTags = null) : base(name, path) => SubTags = subTags ?? new ObservableCollection<TagViewModel>();

        public TagViewModel GetParentTag() => Path.GetTagViewModel()!;

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
