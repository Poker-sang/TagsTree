using System.IO;
using System.Linq;
using TagsTree.ViewModels;
using TagsTree.Views;
using static TagsTree.Properties.Settings;

namespace TagsTree.Services
{
	public static class FilePropertiesService
	{
		private static FileProperties ContentDialog;

		public static void Load(FileProperties contentDialog) => ContentDialog = contentDialog;

		#region 命令

		public static void OpenBClick(object? parameter) => App.FileX.Open(((FilePropertiesViewModel)parameter!).FileModel.FullName);
		public static void OpenExplorerBClick(object? parameter) => App.FileX.Open(((FilePropertiesViewModel)parameter!).FileModel.Path);
		public static void EditTagsBClick(object? parameter)
		{

		}
		public static void RemoveBClick(object? parameter)
		{
			if (!App.MessageBoxX.Warning("是否从软件移除该文件？")) return;
			var value = ((FilePropertiesViewModel)parameter!).FileModel;
			if (!App.IdFile.Contains(value)) return;
			_ = App.IdFile.Remove(value);
			App.Relations.Rows.Remove(App.Relations.RowAt(value));
			App.Relations.RefreshRowsDict();
			App.SaveFiles();
		}
		public static void RenameBClick(object? parameter)
		{
			var dialog = new InputName(MainService.Win, @"不能包含\/:*?""<>|和除空格外的空白字符", App.FileX.GetInvalidNameChars);
			if (dialog.ShowDialog() == false) return;
			if (((FilePropertiesViewModel)parameter!).FileModel.Name == dialog.Message)
			{
				App.MessageBoxX.Error("新文件名与原文件名一致！");
				return;
			}
			var newFullName = ((FilePropertiesViewModel)parameter!).FileModel.Path + @"\" + dialog.Message;
			if (((FilePropertiesViewModel)parameter!).FileModel.IsFolder ? Directory.Exists(newFullName) : File.Exists(newFullName)) 
			{
				App.MessageBoxX.Error("新文件名与文件夹中其他文件同名！");
				return;
			}
			new FileInfo(((FilePropertiesViewModel)parameter!).FileModel.FullName).MoveTo(newFullName);
		}
		public static void MoveBClick(object? parameter)
		{
			var dialog = new InputName(MainService.Win, @"不能包含\/:*?""<>|和除空格外的空白字符（只需填写「文件路径」后的路径）", App.FileX.GetInvalidPathChars);
			if (dialog.ShowDialog() == false) return;
			var newFullPath = Default.LibraryPath + @"\" + dialog.Message;
			if (((FilePropertiesViewModel)parameter!).FileModel.Path == newFullPath)
			{
				App.MessageBoxX.Error("新目录与原目录一致！");
				return;
			}
			if (newFullPath.Contains(((FilePropertiesViewModel)parameter!).FileModel.Path))
			{
				App.MessageBoxX.Error("不能将其移动到原目录下！");
				return;
			}
			if (!Directory.Exists(newFullPath))
			{
				App.MessageBoxX.Error("新目录不存在！");
				return;
			}
			var newFullName = newFullPath + @"\" + ((FilePropertiesViewModel)parameter!).FileModel.Name;
			if (((FilePropertiesViewModel)parameter!).FileModel.IsFolder ? Directory.Exists(newFullName) : File.Exists(newFullName))
			{
				App.MessageBoxX.Error("新文件名与文件夹中其他文件同名！");
				return;
			}
			new FileInfo(((FilePropertiesViewModel) parameter!).FileModel.FullName).MoveTo(newFullName);
		}
		public static void DeleteBClick(object? parameter)
		{
			if (!App.MessageBoxX.Warning("是否删除该文件？")) return;
			File.Delete(((FilePropertiesViewModel)parameter!).FileModel.FullName);
		}

		#endregion
	}
}
