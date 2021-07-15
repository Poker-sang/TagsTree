using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Windows.Foundation.Metadata;
using Microsoft.WindowsAPICodePack.Dialogs;
using SQLite;
using TagsTree.Models;
using TagsTree.ViewModels;
using TagsTree.Views;
using static TagsTree.Properties.Settings;

namespace TagsTree.Services
{
	public static class FileImporterService
	{
		private static readonly FileImporterViewModel Vm = new();
		private static FileImporter Win;

		public static FileImporterViewModel Load(FileImporter window)
		{
			Win = window;
			return Vm;
		}

		public static void Import(object? parameter)
		{
			var dialog = new CommonOpenFileDialog
			{
				Multiselect = true,
				EnsurePathExists = true,
				IsFolderPicker = true,
				InitialDirectory = Default.LibraryPath
			};
			var wrongPath = false;
			switch (parameter!)
			{
				case "Select_Files":
					dialog.IsFolderPicker = false;
					dialog.Title = "选择你需要引入的文件";
					if (dialog.ShowDialog(Win) == CommonFileDialogResult.Ok)
						foreach (var fileName in dialog.FileNames)
						{
							if (!fileName.Contains(Default.LibraryPath))
							{
								wrongPath = true;
								continue;
							}
							var index = fileName.LastIndexOf('\\');
							Vm.FileModels.Add(new FileModel(fileName[(index + 1)..], fileName[..index], false));
						}
					break;
				case "Select_Folders":
					dialog.Title = "选择你需要引入的文件夹";
					if (dialog.ShowDialog(Win) == CommonFileDialogResult.Ok)
						foreach (var directoryName in dialog.FileNames)
						{
							if (!directoryName.Contains(Default.LibraryPath) || directoryName == Default.LibraryPath)
							{
								wrongPath = true;
								continue;
							}
							var index = directoryName.LastIndexOf('\\');
							Vm.FileModels.Add(new FileModel(directoryName[(index + 1)..], directoryName[..index], true));
						}
					break;
				case "Path_Files":
					dialog.Title = "选择你需要引入的文件所在的文件夹";
					if (dialog.ShowDialog(Win) == CommonFileDialogResult.Ok)
						foreach (var directoryName in dialog.FileNames)
						{
							if (!directoryName.Contains(Default.LibraryPath))
							{
								wrongPath = true;
								continue;
							}
							foreach (var fileInfo in new DirectoryInfo(directoryName).GetFiles())
								Vm.FileModels.Add(new FileModel(fileInfo.Name, directoryName, false));
						}
					break;
				case "Path_Folders":
					dialog.Title = "选择你需要引入的文件夹所在的文件夹";
					if (dialog.ShowDialog(Win) == CommonFileDialogResult.Ok)
						foreach (var directoryName in dialog.FileNames)
						{
							if (!directoryName.Contains(Default.LibraryPath))
							{
								wrongPath = true;
								continue;
							}
							foreach (var directoryInfo in new DirectoryInfo(directoryName).GetDirectories())
								Vm.FileModels.Add(new FileModel(directoryInfo.Name, directoryName, true));
						}
					break;
				case "Path_Both":
					dialog.Title = "选择你需要引入的文件和文件夹所在的文件夹";
					if (dialog.ShowDialog(Win) == CommonFileDialogResult.Ok)
						foreach (var directoryName in dialog.FileNames)
						{
							if (!directoryName.Contains(Default.LibraryPath))
							{
								wrongPath = true;
								continue;
							}
							foreach (var fileInfo in new DirectoryInfo(directoryName).GetFiles())
								Vm.FileModels.Add(new FileModel(fileInfo.Name, directoryName, false));
							foreach (var directoryInfo in new DirectoryInfo(directoryName).GetDirectories())
								Vm.FileModels.Add(new FileModel(directoryInfo.Name, directoryName, true));
						}
					break;
				case "All":
					dialog.Title = "选择你需要引入的文件所在的根文件夹";
					if (dialog.ShowDialog(Win) == CommonFileDialogResult.Ok)
					{
						static void RecursiveReadFiles(string folderName)
						{
							foreach (var fileInfo in new DirectoryInfo(folderName).GetFiles())
								Vm.FileModels.Add(new FileModel(fileInfo.Name, folderName, false));
							foreach (var directoryInfo in new DirectoryInfo(folderName).GetDirectories())
								RecursiveReadFiles(directoryInfo.FullName);
						}
						foreach (var directoryName in dialog.FileNames)
						{
							if (!directoryName.Contains(Default.LibraryPath))
							{
								wrongPath = true;
								continue;
							}
							RecursiveReadFiles(directoryName);
						}
					}
					break;
			}
			if (wrongPath)
				App.ErrorMessageBox("只允许导入文件路径下的文件或文件夹，不符合的已被剔除");
		}

		public static void DeleteBClick(object? parameter) => Vm.FileModels.Clear();
		public static void SaveBClick(object? parameter)
		{
			var db = new SQLiteConnection(Default.ConfigPath + @"\Files.db");
			_ = db.CreateTable<FileModel>();
			var former = Vm.FileModels.Count;
			foreach (var dbFileModel in db.Table<FileModel>())
				for (var i = 0; i < Vm.FileModels.Count; i++)
					if (Vm.FileModels[i].Name == dbFileModel.Name && Vm.FileModels[i].Path == dbFileModel.Path)
						Vm.FileModels.RemoveAt(i);
			var add = db.InsertAll(Vm.FileModels);
			_ = MessageBox.Show($"共导入 {former} 个文件，其中成功导入 {add} 个，有 {former - add} 个因重复未导入", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
			Vm.FileModels.Clear();
		}
	}
}
