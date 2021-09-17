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
            App.Relations.NewFile(newFileModel);
            App.IdFile[newFileModel.Id] = newFileModel;
        }

        public static void RemoveAndSave(this FileViewModel fileViewModel)
        {
            Remove(fileViewModel);
            App.SaveFiles();
            App.SaveRelations();
        }
        public static void Remove(this FileViewModel fileViewModel)
        {
            var fileModel = fileViewModel.GetFileModel();
            _ = App.IdFile.Remove(fileModel);
            App.Relations.DeleteFile(fileModel);
        }

        public static void MoveOrRenameAndSave(this FileViewModel fileViewModel, string newFullName)
        {
            MoveOrRename(fileViewModel, newFullName);
            App.SaveFiles();
        }
        public static void MoveOrRename(this FileViewModel fileViewModel, string newFullName)
        {
            fileViewModel.Reload(newFullName);
        }
    }
}