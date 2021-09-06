using System.IO;
using System.Linq;
using TagsTreeWinUI3.Models;

namespace TagsTreeWinUI3.Services
{
	public class FilesObserver : FileSystemWatcher
	{
		public FilesObserver()
		{
			IncludeSubdirectories = true;
			NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName;
			base.Created += Created;
			base.Renamed += Renamed;
			base.Deleted += Deleted;
		}

		public void Initialize(string path)
		{
			Path = path;
			App.AppConfigurations.FilesObserverEnabled = App.AppConfigurations.FilesObserverEnabled;
		}

		private new static void Created(object sender, FileSystemEventArgs e)
		{
			_ = App.Window.DispatcherQueue.TryEnqueue(() =>
			{
				if (App.FilesChangedList.LastOrDefault() is { Type: FileChanged.ChangedType.Delete } item && item.Name == e.FullPath.GetName() && item.FullName != e.FullPath)
				{
					_ = App.FilesChangedList.Remove(item);
					App.FilesChangedList.Add(new FileChanged(e.FullPath, FileChanged.ChangedType.Move, item.Path));
				}
				else App.FilesChangedList.Add(new FileChanged(e.FullPath, FileChanged.ChangedType.Create));
			});
		}

		private new static void Renamed(object sender, RenamedEventArgs e)
		{
			_ = App.Window.DispatcherQueue.TryEnqueue
			(
				() => App.FilesChangedList.Add(new FileChanged(e.FullPath, FileChanged.ChangedType.Rename, e.OldFullPath.GetName()))
			);
		}

		private new static void Deleted(object sender, FileSystemEventArgs e)
		{
			_ = App.Window.DispatcherQueue.TryEnqueue
			(
				() => App.FilesChangedList.Add(new FileChanged(e.FullPath, FileChanged.ChangedType.Delete))
			);
		}
	}
}
