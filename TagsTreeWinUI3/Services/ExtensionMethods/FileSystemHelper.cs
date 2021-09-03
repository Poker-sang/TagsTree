using System;
using System.Diagnostics;
using System.IO;
using Microsoft.VisualBasic.FileIO;
using TagsTreeWinUI3.ViewModels;

namespace TagsTreeWinUI3.Services.ExtensionMethods
{
	public static class FileSystemHelper
	{
		public static bool Exists(this string fullName) => File.Exists(fullName) || Directory.Exists(fullName);

		public static void Open(this FileViewModel fileViewModel)
		{
			try
			{
				var process = new Process { StartInfo = new ProcessStartInfo(fileViewModel.FullName) };
				process.StartInfo.UseShellExecute = true;
				process.Start();
			}
			catch (System.ComponentModel.Win32Exception)
			{
				MessageDialogX.Information(true, "找不到文件（夹），源文件可能已被更改");
			}
		}
		public static void OpenDirectory(this FileViewModel fileViewModel)
		{
			try
			{
				var process = new Process { StartInfo = new ProcessStartInfo(fileViewModel.Path) };
				process.StartInfo.UseShellExecute = true;
				process.Start();
			}
			catch (System.ComponentModel.Win32Exception)
			{
				MessageDialogX.Information(true, "找不到目录，源文件可能已被更改");
			}
		}

		public static void Move(this FileViewModel fileViewModel, string newFullName)
		{
			var fileModel = fileViewModel.GetFileModel();
			if (fileModel.IsFolder)
				FileSystem.MoveDirectory(fileModel.FullName, newFullName.GetPath(), UIOption.OnlyErrorDialogs);
			FileSystem.MoveFile(fileModel.FullName, newFullName.GetPath(), UIOption.OnlyErrorDialogs);
		}
		public static void Rename(this FileViewModel fileViewModel, string newFullName)
		{
			var fileModel = fileViewModel.GetFileModel();
			if (fileModel.IsFolder)
				FileSystem.RenameDirectory(fileModel.FullName, newFullName.GetName());
			FileSystem.RenameFile(fileModel.FullName, newFullName.GetName());
		}
		public static void Delete(this FileViewModel fileViewModel)
		{
			var fileModel = fileViewModel.GetFileModel();

			if (fileModel.IsFolder)
				FileSystem.DeleteDirectory(fileModel.FullName, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
			FileSystem.DeleteFile(fileModel.FullName, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
		}
	}
}