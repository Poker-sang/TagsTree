using TagsTree.ViewModels;

namespace TagsTree.Services.ExtensionMethods;

public static class FileViewModelHelper
{
    public static void AddNewAndSave(this FileViewModel fileViewModel)
    {
        fileViewModel.FileModel.AddNew();
        AppContext.SaveFiles();
        AppContext.SaveRelations();
    }

    public static void RemoveAndSave(this FileViewModel fileViewModel)
    {
        fileViewModel.FileModel.Remove();
        AppContext.SaveFiles();
        AppContext.SaveRelations();
    }
    
    public static void MoveOrRenameAndSave(this FileViewModel fileViewModel, string newFullName)
    {
        fileViewModel.FileModel.MoveOrRename(newFullName);
        AppContext.SaveFiles();
    }
}
