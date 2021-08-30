using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TagsTreeWinUI3.Services.ExtensionMethods;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Pickers;

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
			folderPicker.FileTypeFilter.Add(".");
			return await folderPicker.PickSingleFolderAsync();
		}
		public static async Task<IReadOnlyList<StorageFile>> GetStorageFiles() => await new FileOpenPicker().InitializeWithWindow().PickMultipleFilesAsync();

		public static async Task<StorageFile> GetStorageFile() => await new FileOpenPicker().InitializeWithWindow().PickSingleFileAsync();
	}

}
