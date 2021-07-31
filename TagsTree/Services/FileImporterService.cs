using Microsoft.WindowsAPICodePack.Dialogs;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using TagsTree.Models;
using TagsTree.ViewModels;
using TagsTree.Views;
using static System.Windows.Application;
using static TagsTree.Properties.Settings;

namespace TagsTree.Services
{
	public static class FileImporterService
	{
		private static FileImporterViewModel Vm;
		private static FileImporter Win;

		public static void Load(FileImporter window)
		{
			Win = window;
			Vm = (FileImporterViewModel) window.DataContext;
		}

		public static async void Import(object? parameter)
		{
			Vm.Importing = true;
			var dialog = new CommonOpenFileDialog
			{
				Multiselect = true,
				EnsurePathExists = true,
				IsFolderPicker = true,
				InitialDirectory = Default.LibraryPath
			};
			var dictionary = new Dictionary<string, bool>();
			await Task.Run(() =>
			{
				foreach (var fileModel in Vm.FileModels)
					dictionary[fileModel.UniqueName] = true;
			});
			switch ((string)parameter!)
			{
				case "Select_Files":
					dialog.IsFolderPicker = false;
					dialog.Title = "选择你需要引入的文件";
					if (dialog.ShowDialog(Win) == CommonFileDialogResult.Ok)
						await Task.Run(() =>
						{
							foreach (var fileName in dialog.FileNames)
							{
								if (!FileModel.ValidPath(fileName))
									continue;
								var index = fileName.LastIndexOf('\\');
								if (!dictionary.ContainsKey(fileName))
									Current.Dispatcher.Invoke(() => Vm.FileModels.Add(new FileModel(fileName[(index + 1)..], fileName[..index], false)));
							}
						});
					break;
				case "Select_Folders":
					dialog.Title = "选择你需要引入的文件夹";
					if (dialog.ShowDialog(Win) == CommonFileDialogResult.Ok)
						await Task.Run(() =>
						{
							foreach (var directoryName in dialog.FileNames)
							{
								if (!FileModel.ValidPath(directoryName) || directoryName == Default.LibraryPath)
									continue;
								var index = directoryName.LastIndexOf('\\');
								if (!dictionary.ContainsKey(directoryName))
									Current.Dispatcher.Invoke(() => Vm.FileModels.Add(new FileModel(directoryName[(index + 1)..], directoryName[..index], true)));
							}
						});
					break;
				case "Path_Files":
					dialog.Title = "选择你需要引入的文件所在的文件夹";
					if (dialog.ShowDialog(Win) == CommonFileDialogResult.Ok)
						await Task.Run(() =>
						{
							foreach (var directoryName in dialog.FileNames)
							{
								if (!FileModel.ValidPath(directoryName))
									continue;
								foreach (var fileInfo in new DirectoryInfo(directoryName).GetFiles())
									if (!dictionary.ContainsKey(fileInfo.FullName + false))
										Current.Dispatcher.Invoke(() => Vm.FileModels.Add(new FileModel(fileInfo.Name, directoryName, false)));
							}
						});
					break;
				case "Path_Folders":
					dialog.Title = "选择你需要引入的文件夹所在的文件夹";
					if (dialog.ShowDialog(Win) == CommonFileDialogResult.Ok)
						await Task.Run(() =>
						{
							foreach (var directoryName in dialog.FileNames)
							{
								if (!directoryName.Contains(Default.LibraryPath))
									continue;
								foreach (var directoryInfo in new DirectoryInfo(directoryName).GetDirectories())
									if (!dictionary.ContainsKey(directoryInfo.FullName + true))
										Current.Dispatcher.Invoke(() => Vm.FileModels.Add(new FileModel(directoryInfo.Name, directoryName, true)));
							}
						});
					break;
				case "Path_Both":
					dialog.Title = "选择你需要引入的文件和文件夹所在的文件夹";
					if (dialog.ShowDialog(Win) == CommonFileDialogResult.Ok)
						await Task.Run(() =>
						{
							foreach (var directoryName in dialog.FileNames)
							{
								if (!FileModel.ValidPath(directoryName))
									continue;
								foreach (var fileInfo in new DirectoryInfo(directoryName).GetFiles())
									if (!dictionary.ContainsKey(fileInfo.FullName + false))
										Current.Dispatcher.Invoke(() => Vm.FileModels.Add(new FileModel(fileInfo.Name, directoryName, false)));
								foreach (var directoryInfo in new DirectoryInfo(directoryName).GetDirectories())
									if (!dictionary.ContainsKey(directoryInfo.FullName + true))
										Current.Dispatcher.Invoke(() => Vm.FileModels.Add(new FileModel(directoryInfo.Name, directoryName, true)));
							}
						});
					break;
				case "All":
					dialog.Title = "选择你需要引入的文件所在的根文件夹";
					if (dialog.ShowDialog(Win) == CommonFileDialogResult.Ok)
						await Task.Run(() =>
						{
							void RecursiveReadFiles(string folderName)
							{
								foreach (var fileInfo in new DirectoryInfo(folderName).GetFiles())
									if (!dictionary.ContainsKey(fileInfo.FullName + false))
										Current.Dispatcher.Invoke(() => Vm.FileModels.Add(new FileModel(fileInfo.Name, folderName, false)));
								foreach (var directoryInfo in new DirectoryInfo(folderName).GetDirectories())
									RecursiveReadFiles(directoryInfo.FullName);
							}
							foreach (var directoryName in dialog.FileNames)
							{
								if (!FileModel.ValidPath(directoryName))
									continue;
								RecursiveReadFiles(directoryName);
							}
						});
					break;
			}
			Vm.Importing = false;
		}

		public static void DeleteBClick(object? parameter) => Vm.FileModels.Clear();
		public static async void SaveBClick(object? parameter)
		{
			var border = new Border { Background = new SolidColorBrush(Color.FromArgb(0x55, 0x88, 0x88, 0x88)) };
			var progressBar = new ModernWpf.Controls.ProgressBar { Width = 300, Height = 20 };
			progressBar.ValueChanged += (_, _) => border.Child.UpdateLayout();
			border.Child = progressBar;
			_ = ((Grid)parameter!).Children.Add(border);
			progressBar.Value = 1;

			var duplicated = 0;
			await Task.Run(() =>
			{
				var dictionary = new Dictionary<string, bool>();
				foreach (var fileModel in App.IdToFile.Values)
					dictionary[fileModel.UniqueName] = true;
				_ = Current.Dispatcher.Invoke(() => progressBar.Value = 2);
				var unit = 97.0 / Vm.FileModels.Count;
				foreach (var fileModel in Vm.FileModels)
				{
					if (!dictionary.ContainsKey(fileModel.UniqueName))
					{
						App.Relations.NewRow(fileModel);
						App.IdToFile[fileModel.Id] = fileModel;
					}
					else duplicated++;
					_ = Current.Dispatcher.Invoke(() => progressBar.Value += unit);
				}
			});
			App.SaveFiles();
			App.SaveRelations();
			progressBar.Value = 100;
			var former = Vm.FileModels.Count;
			Vm.FileModels.Clear();
			((Grid)parameter!).Children.Remove(border);
			App.InformationMessageBox($"共导入 {former} 个文件，其中成功导入 {former - duplicated} 个，有 {duplicated} 个因重复未导入");
		}
	}
}