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

    public async Task<bool> Change(string path)
    {
        // 路径是否存在
        if (!Directory.Exists(App.AppConfig.LibraryPath))
        {
            await ShowMessageDialog.Information(true, $"路径「{App.AppConfig.LibraryPath}」不存在，无法开启文件监视，请在设置修改正确路径后保存");
            return App.FilesObserver.EnableRaisingEvents = false;
        }
        // 不能是错误路径
        Path = path;
        return App.FilesObserver.EnableRaisingEvents = App.AppConfig.FilesObserverEnabled;
    }

    private static new void Created(object sender, FileSystemEventArgs e)
        => _ = CurrentContext.Window.DispatcherQueue.TryEnqueue(() =>
        {
            if (App.FilesChangedList.LastOrDefault() is { Type: FileChanged.ChangedType.Delete } item && item.Name == e.FullPath.GetName() && item.FullName != e.FullPath)
            {
                _ = App.FilesChangedList.Remove(item);
                App.FilesChangedList.Add(new(e.FullPath, FileChanged.ChangedType.Move, item.Path));
            }
            else
                App.FilesChangedList.Add(new(e.FullPath, FileChanged.ChangedType.Create));
        });

    private static new void Renamed(object sender, RenamedEventArgs e) => _ = CurrentContext.Window.DispatcherQueue.TryEnqueue
        (
            () => App.FilesChangedList.Add(new(e.FullPath, FileChanged.ChangedType.Rename, e.OldFullPath.GetName()))
        );

    private static new void Deleted(object sender, FileSystemEventArgs e) => _ = CurrentContext.Window.DispatcherQueue.TryEnqueue
        (
            () => App.FilesChangedList.Add(new(e.FullPath, FileChanged.ChangedType.Delete))
        );
}
