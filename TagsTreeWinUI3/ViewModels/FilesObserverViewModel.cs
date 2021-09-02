using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
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
				if(e.Action is NotifyCollectionChangedAction.Remove)
					FileChanged.Num--;
				FileChanged.Serialize(App.FilesChangedPath, FilesChangedList); //或App.FilesChangedList
			};
		}
	}
}
