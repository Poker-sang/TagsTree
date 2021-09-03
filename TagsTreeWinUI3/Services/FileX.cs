﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using TagsTreeWinUI3.Services.ExtensionMethods;

namespace TagsTreeWinUI3.Services
{
	public static class FileX
	{
		public static string GetInvalidNameChars => @"\\/:*?""<>|" + new string(Path.GetInvalidPathChars());
		public static string GetInvalidPathChars => @"\/:*?""<>|" + new string(Path.GetInvalidPathChars());
		public enum InvalidMode
		{
			Name = 0,
			Path = 1
		}

		public static string GetName(this string fullName) => fullName[(fullName.LastIndexOf('\\') + 1)..];
		public static string GetPath(this string fullName) => fullName.LastIndexOf('\\') is -1 ? "" : fullName[..fullName.LastIndexOf('\\')];

		public static string CountSize(FileInfo file) => " " + file.Length switch
		{
			< 1 << 10 => file.Length.ToString("F2") + "Byte",
			< 1 << 20 => ((double)file.Length / (1 << 10)).ToString("F2") + "KB",
			< 1 << 30 => ((double)file.Length / (1 << 20)).ToString("F2") + "MB",
			_ => ((double)file.Length / (1 << 30)).ToString("F2") + "GB"
		};


		public static async Task<StorageFolder> GetStorageFolder()
		{
			var folderPicker = new FolderPicker().InitializeWithWindow();
			folderPicker.FileTypeFilter.Add("."); //不加会崩溃
			return await folderPicker.PickSingleFolderAsync();
		}
		public static async Task<StorageFile> GetStorageFile()
		{
			var fileOpenPicker = new FileOpenPicker().InitializeWithWindow();
			fileOpenPicker.FileTypeFilter.Add(".");
			return await fileOpenPicker.PickSingleFileAsync();
		}
		public static async Task<IReadOnlyList<StorageFile>> GetStorageFiles()
		{
			var fileOpenPicker = new FileOpenPicker().InitializeWithWindow();
			fileOpenPicker.FileTypeFilter.Add(".");
			return await fileOpenPicker.PickMultipleFilesAsync();
		}
	}
}
