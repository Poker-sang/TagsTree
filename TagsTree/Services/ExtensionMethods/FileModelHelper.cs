using TagsTree.Models;

namespace TagsTree.Services.ExtensionMethods;

public static class FileModelHelper
{
    public static void AddNew(this FileModel fileModel)
    {
        AppContext.Relations.NewFile(fileModel);
        AppContext.IdFile[fileModel.Id] = fileModel;
    }

    public static void Remove(this FileModel fileModel)
    {
        _ = AppContext.IdFile.Remove(fileModel);
        AppContext.Relations.DeleteFile(fileModel);
    }

    public static void MoveOrRename(this FileModel fileModel, string newFullName) => fileModel.Reload(newFullName);
}
