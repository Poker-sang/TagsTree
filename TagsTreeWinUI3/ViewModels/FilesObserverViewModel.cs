using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using TagsTreeWinUI3.Models;

namespace TagsTreeWinUI3.ViewModels
{
	public class FilesObserverViewModel : ObservableObject
	{
		public ObservableCollection<FileChanged> FilesChangedList { get; }

		public FilesObserverViewModel(ObservableCollection<FileChanged> filesChangedList)
		{
			FilesChangedList = filesChangedList;
			FilesChangedList.CollectionChanged += (_, e) =>
			{
				if (e.Action is NotifyCollectionChangedAction.Remove)
					FileChanged.Num--;
				FileChanged.Serialize(App.FilesChangedPath, FilesChangedList); //或App.FilesChangedList
			};
		}
	}
}
