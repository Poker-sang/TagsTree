using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using TagsTreeWinUI3.Models;

namespace TagsTreeWinUI3.ViewModels
{
	public sealed class TagsManagerViewModel : ObservableObject
	{
		public ObservableCollection<TagModel> TagsSource { get; set; } = App.Tags.TagsTree.SubTags;


		private string _name = "";
		public string Name
		{
			get => _name;
			set
			{
				if (Equals(value, _name)) return;
				_name = value;
				OnPropertyChanged(nameof(Name));
			}
		}
	}
}
