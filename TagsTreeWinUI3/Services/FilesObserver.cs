using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
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
				if (App.FilesChangedList.LastOrDefault() is { Type: "Delete" } item && item.Name == e.FullPath[(e.FullPath.LastIndexOf('\\') + 1)..] && item.FullName != e.FullPath)
				{
					_ = App.FilesChangedList.Remove(item);
					App.FilesChangedList.Add(new FilesChanged(e.FullPath, "Move", "Old Path: " + item.PartialPath));
				}
				else App.FilesChangedList.Add(new FilesChanged(e.FullPath, "Create"));
			});
		}

		private new static void Renamed(object sender, RenamedEventArgs e)
		{
			_ = App.Window.DispatcherQueue.TryEnqueue
			(
				() => App.FilesChangedList.Add(new FilesChanged(e.FullPath, "Rename", "Old Name: " + e.OldName))
			);
		}

		private new static void Deleted(object sender, FileSystemEventArgs e)
		{
			_ = App.Window.DispatcherQueue.TryEnqueue
			(
				() => App.FilesChangedList.Add(new FilesChanged(e.FullPath, "Delete"))
			);
		}
	}
}
