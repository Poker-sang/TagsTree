using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using Windows.UI.Xaml.Interop;
using TagsTreeWinUI3.Services.ExtensionMethods;

namespace TagsTreeWinUI3.Models
{
	public class PathTagModel
	{
		public string Name { get; protected set; }
		public PathTagModel(string name) => Name = name;
	}

	public class TagModel : PathTagModel, INotifyPropertyChanged
	{
		[JsonIgnore]
		private static int Num { get; set; } = 1;

		public int Id { get; }
		public ObservableCollection<TagModel> SubTags { get; set; }
		[JsonIgnore]
		public string Path { get; set; }

		public TagModel(string name, string path, ObservableCollection<TagModel>? subTags = null) : base(name)
		{
			Id = Num;
			Num++;
			Path = path;
			SubTags = subTags ?? new ObservableCollection<TagModel>();
		}
		/// <summary>
		/// 反序列化专用，不要调用该构造器
		/// </summary>
		[JsonConstructor]
		public TagModel(int id, string name, string path, ObservableCollection<TagModel>? subTags = null) : base(name)
		{
			Num = Math.Max(Num, id + 1);
			Id = id;
			Path = path;
			SubTags = subTags ?? new ObservableCollection<TagModel>();
		}
		[JsonIgnore]
		public string FullName => (Path is "" ? "" : Path + '\\') + Name;
		public override string ToString() => Name;

		public bool HasChildTag(TagModel child) => $"\\\\{child.Path}\\".Contains($"\\{Name}\\"); //不包含自己
		[JsonIgnore]
		public TagModel GetParentTag => Path.GetTagModel()!; //根节点不能用

		public new string Name
		{
			get => base.Name;
			set
			{
				if (base.Name == value) return;
				base.Name = value;
				OnPropertyChanged(Name);
			}
		}



		public event PropertyChangedEventHandler? PropertyChanged;
		private void OnPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}
}
