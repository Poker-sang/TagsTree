using TagsTree.ViewModels;

namespace TagsTree.Services.ExtensionMethods;

public static class FileViewModelHelper
{
    public static void AddNewAndSave(this FileViewModel fileViewModel)
    {
        fileViewModel.GetFileModel().AddNew();
        App.SaveFiles();
        App.SaveRelations();
    }
    public static void RemoveAndSave(this FileViewModel fileViewModel)
    {
        fileViewModel.GetFileModel().Remove();
        App.SaveFiles();
        App.SaveRelations();
    }
    public static void MoveOrRenameAndSave(this FileViewModel fileViewModel, string newFullName)
    {
        fileViewModel.Reload(newFullName);
        fileViewModel.GetFileModel().MoveOrRename(newFullName);
        App.SaveFiles();
    }
}
