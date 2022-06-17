using TagsTree.Models;

namespace TagsTree.Services.ExtensionMethods;

public static class FileModelHelper
{
    public static void AddNewAndSave(this FileModel fileModel)
    {
        AddNew(fileModel);
        App.SaveFiles();
        App.SaveRelations();
    }
    public static void RemoveAndSave(this FileModel fileModel)
    {
        Remove(fileModel);
        App.SaveFiles();
        App.SaveRelations();
    }
    public static void MoveOrRenameAndSave(this FileModel fileModel, string newFullName)
    {
        MoveOrRename(fileModel, newFullName);
        App.SaveFiles();
    }

    public static void AddNew(this FileModel fileModel)
    {
        App.Relations.NewFile(fileModel);
        App.IdFile[fileModel.Id] = fileModel;
    }
    public static void Remove(this FileModel fileModel)
    {
        _ = App.IdFile.Remove(fileModel);
        App.Relations.DeleteFile(fileModel);
    }
    public static void MoveOrRename(this FileModel fileModel, string newFullName) => fileModel.Reload(newFullName);
}