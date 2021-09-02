using System;
using TagsTreeWinUI3.Models;
using TagsTreeWinUI3.ViewModels;

namespace TagsTreeWinUI3.Services.ExtensionMethods
{
	public static class FileViewModelHelper
	{
		public static void AddNewAndSave(this FileViewModel fileViewModel)
		{
			AddNew(fileViewModel);
			App.SaveFiles();
			App.SaveRelations();
		}
		public static void AddNew(this FileViewModel fileViewModel)
		{
			var newFileModel = fileViewModel.NewFileModel();
			App.Relations.NewRow(newFileModel);
			App.IdFile[newFileModel.Id] = newFileModel;
		}

		public static void RemoveAndSave(this FileModel fileModel)
		{
			Remove(fileModel);
			App.SaveFiles();
			App.SaveRelations();
		}
		public static void Remove(this FileModel fileModel)
		{
			if (!App.IdFile.Contains(fileModel))
				throw new NullReferenceException("文件列表中不存在：" + fileModel.FullName);
			_ = App.IdFile.Remove(fileModel);
			App.Relations.Rows.Remove(App.Relations.RowAt(fileModel));
		}

		public static void MoveOrRenameAndSave(this FileModel fileModel, string newFullName)
		{
			MoveOrRename(fileModel, newFullName);
			App.SaveFiles();
		}
		public static void MoveOrRename(this FileModel fileViewModel, string newFullName)
		{
			fileViewModel.Reload(newFullName);
		}
	}
}