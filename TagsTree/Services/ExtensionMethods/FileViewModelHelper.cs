using TagsTree.ViewModels;

namespace TagsTree.Services.ExtensionMethods;

public static class FileViewModelHelper
{
    public static void AddNewAndSave(this FileViewModel fileViewModel)
    {
        fileViewModel.FileModel.AddNew();
        App.SaveFiles();
        App.SaveRelations();
    }
    public static void RemoveAndSave(this FileViewModel fileViewModel)
    {
        fileViewModel.FileModel.Remove();
        App.SaveFiles();
        App.SaveRelations();
    }
    public static void MoveOrRenameAndSave(this FileViewModel fileViewModel, string newFullName)
    {
        fileViewModel.FileModel.MoveOrRename(newFullName);
        App.SaveFiles();
    }
}
