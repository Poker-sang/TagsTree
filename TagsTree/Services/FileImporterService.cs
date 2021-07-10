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
using TagsTree.Models;
using TagsTree.ViewModels;
using static TagsTree.Properties.Settings;

namespace TagsTree.Services
{
	public static class FileImporterService
	{
		public static readonly FileImporterViewModel Vm = new();
		public static Views.FileImporter? Win;

		public static void Import(object? parameter)
		{
			var dialog = new CommonOpenFileDialog
			{
				Multiselect = true,
				EnsurePathExists = true,
				IsFolderPicker = true,
				InitialDirectory = Default.LibraryPath
			};
			switch (parameter!)
			{
				case "Select_Files":
					dialog.IsFolderPicker = false;
					dialog.Title = "选择你需要引入的文件";
					if (dialog.ShowDialog(Win) == CommonFileDialogResult.Ok)
						foreach (var fileName in dialog.FileNames)
						{
							var index = fileName.LastIndexOf('\\');
							Vm.FileModels.Add(new FileModel(fileName[(index + 1)..], fileName[..index], false));
						}
					break;
				case "Select_Folders":
					dialog.Title = "选择你需要引入的文件夹";
					if (dialog.ShowDialog(Win) == CommonFileDialogResult.Ok)
						foreach (var directoryName in dialog.FileNames)
						{
							var index = directoryName.LastIndexOf('\\');
							Vm.FileModels.Add(new FileModel(directoryName[(index + 1)..], directoryName[..index], true));
						}
					break;
				case "Path_Files":
					dialog.Title = "选择你需要引入的文件所在的文件夹";
					if (dialog.ShowDialog(Win) == CommonFileDialogResult.Ok)
						foreach (var directoryName in dialog.FileNames)
							foreach (var fileInfo in new DirectoryInfo(directoryName).GetFiles())
								Vm.FileModels.Add(new FileModel(fileInfo.Name, directoryName, false));
					break;
				case "Path_Folders":
					dialog.Title = "选择你需要引入的文件夹所在的文件夹";
					if (dialog.ShowDialog(Win) == CommonFileDialogResult.Ok)
						foreach (var directoryName in dialog.FileNames)
							foreach (var directoryInfo in new DirectoryInfo(directoryName).GetDirectories())
								Vm.FileModels.Add(new FileModel(directoryInfo.Name, directoryName, true));
					break;
				case "Path_Both":
					dialog.Title = "选择你需要引入的文件和文件夹所在的文件夹";
					if (dialog.ShowDialog(Win) == CommonFileDialogResult.Ok)
						foreach (var directoryName in dialog.FileNames)
						{
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
							RecursiveReadFiles(directoryName);
					}
					break;
			}
		}

		public static void DeleteBClick(object? parameter) => Vm.FileModels.Clear();
		public static void SaveBClick(object? parameter)
		{
			throw new NotImplementedException();
		}
	}
}
