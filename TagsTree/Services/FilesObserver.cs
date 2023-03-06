using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TagsTree.Models;
using TagsTree.Services.ExtensionMethods;
using WinUI3Utilities;

namespace TagsTree.Services;

public class FilesObserver : FileSystemWatcher
{
    public FilesObserver()
    {
        IncludeSubdirectories = true;
        NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName;
        base.Created += Created;
        base.Renamed += Renamed;
        base.Deleted += Deleted;
    }

    public async Task FilesObserverChanged(string path)
    {
        // 路径是否存在
        if (!Directory.Exists(AppContext.AppConfig.LibraryPath))
        {
            await ShowContentDialog.Information(true, $"路径「{AppContext.AppConfig.LibraryPath}」不存在，无法开启文件监视，请在设置修改正确路径后保存");
            AppContext.FilesObserver.EnableRaisingEvents = false;
        }
        // 不能是错误路径
        Path = path;
        AppContext.FilesObserver.EnableRaisingEvents = AppContext.AppConfig.FilesObserverEnabled;
    }

    private static new void Created(object sender, FileSystemEventArgs e)
        => _ = CurrentContext.Window.DispatcherQueue.TryEnqueue(() =>
        {
            if (AppContext.FilesChangedList.LastOrDefault() is { Type: FileChanged.ChangedType.Delete } item && item.Name == e.FullPath.GetName() && item.FullName != e.FullPath)
            {
                _ = AppContext.FilesChangedList.Remove(item);
                AppContext.FilesChangedList.Add(new(e.FullPath, FileChanged.ChangedType.Move, item.Path));
            }
            else
                AppContext.FilesChangedList.Add(new(e.FullPath, FileChanged.ChangedType.Create));
        });

    private static new void Renamed(object sender, RenamedEventArgs e) => _ = CurrentContext.Window.DispatcherQueue.TryEnqueue
        (
            () => AppContext.FilesChangedList.Add(new(e.FullPath, FileChanged.ChangedType.Rename, e.OldFullPath.GetName()))
        );

    private static new void Deleted(object sender, FileSystemEventArgs e) => _ = CurrentContext.Window.DispatcherQueue.TryEnqueue
        (
            () => AppContext.FilesChangedList.Add(new(e.FullPath, FileChanged.ChangedType.Delete))
        );
}
