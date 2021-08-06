using System.IO;
using TagsTree.ViewModels;
using TagsTree.Views;
using static TagsTree.Properties.Settings;
using Vm = TagsTree.ViewModels.FilePropertiesViewModel;

namespace TagsTree.Services
{
	public static class FilePropertiesService
	{
		private static FileProperties ContentDialog;

		public static void Load(FileProperties contentDialog) => ContentDialog = contentDialog;

		#region 命令

		public static void OpenBClick(object? parameter) => App.FileX.Open(((Vm)parameter!).FileViewModel.FullName);
		public static void OpenExplorerBClick(object? parameter) => App.FileX.Open(((Vm)parameter!).FileViewModel.Path);
		public static void EditTagsBClick(object? parameter)
		{

		}
		public static void RemoveBClick(object? parameter)
		{
			if (!App.MessageBoxX.Warning("是否从软件移除该文件？")) return;
			Remove(((Vm)parameter!).FileViewModel);
		}
		public static void RenameBClick(object? parameter)
		{
			var dialog = new InputName(MainService.Win, @"不能包含\/:*?""<>|和除空格外的空白字符", App.FileX.GetInvalidNameChars, ((Vm)parameter!).FileViewModel.Name);
			if (dialog.ShowDialog() == false) return;
			if (((Vm)parameter).FileViewModel.Name == dialog.Message)
			{
				App.MessageBoxX.Error("新文件名与原文件名一致！");
				return;
			}
			var newFullName = ((Vm)parameter).FileViewModel.Path + @"\" + dialog.Message;
			if (((Vm)parameter).FileViewModel.IsFolder ? Directory.Exists(newFullName) : File.Exists(newFullName))
			{
				App.MessageBoxX.Error("新文件名与文件夹中其他文件同名！");
				return;
			}
			new FileInfo(((Vm)parameter).FileViewModel.FullName).MoveTo(newFullName);
			((Vm)parameter).FileViewModel.Reload(newFullName);
			((Vm)parameter).Load(((Vm)parameter).FileViewModel);
		}
		public static void MoveBClick(object? parameter)
		{
			var dialog = new InputName(MainService.Win, @"不能包含\/:*?""<>|和除空格外的空白字符（只需填写「文件路径」后的路径）", App.FileX.GetInvalidPathChars);
			if (dialog.ShowDialog() == false) return;
			var newFullPath = Default.LibraryPath + @"\" + dialog.Message;
			if (((Vm)parameter!).FileViewModel.Path == newFullPath)
			{
				App.MessageBoxX.Error("新目录与原目录一致！");
				return;
			}
			if (newFullPath.Contains(((Vm)parameter).FileViewModel.Path))
			{
				App.MessageBoxX.Error("不能将其移动到原目录下！");
				return;
			}
			if (!Directory.Exists(newFullPath))
			{
				App.MessageBoxX.Error("新目录不存在！");
				return;
			}
			var newFullName = newFullPath + @"\" + ((Vm)parameter).FileViewModel.Name;
			if (((Vm)parameter).FileViewModel.IsFolder ? Directory.Exists(newFullName) : File.Exists(newFullName))
			{
				App.MessageBoxX.Error("新文件名与文件夹中其他文件同名！");
				return;
			}
			new FileInfo(((Vm)parameter).FileViewModel.FullName).MoveTo(newFullName);
			((Vm)parameter).FileViewModel.Reload(newFullName);
			((Vm)parameter).Load(((Vm)parameter).FileViewModel);
		}
		public static void DeleteBClick(object? parameter)
		{
			if (!App.MessageBoxX.Warning("是否删除该文件？")) return;
			File.Delete(((Vm)parameter!).FileViewModel.FullName);
			Remove(((Vm)parameter).FileViewModel);
		}

		#endregion

		#region 操作

		private static void Remove(FileViewModel fileModel)
		{
			ContentDialog.Hide();
			if (!App.IdFile.Contains(fileModel.GetFileModel)) return;
			_ = App.IdFile.Remove(fileModel.GetFileModel);
			App.Relations.Rows.Remove(App.Relations.RowAt(fileModel.GetFileModel));
			App.Relations.RefreshRowsDict();
			App.SaveFiles();
		}

		#endregion
	}
}
