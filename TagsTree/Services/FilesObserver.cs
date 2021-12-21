using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TagsTree.Models;
using TagsTree.Services.ExtensionMethods;

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
        //路径是否存在
        if (!Directory.Exists(App.AppConfiguration.LibraryPath))
        {
            await ShowMessageDialog.Information(true, $"路径「{App.AppConfiguration.LibraryPath}」不存在，无法开启文件监视，请在设置修改正确路径后保存");
            return App.FilesObserver.EnableRaisingEvents = false;
        }

        Path = path; //不能是错误路径
        return App.FilesObserver.EnableRaisingEvents = App.AppConfiguration.FilesObserverEnabled;
    }

    private new static void Created(object sender, FileSystemEventArgs e)
    {
        _ = App.Window.DispatcherQueue.TryEnqueue(() =>
        {
            if (App.FilesChangedList.LastOrDefault() is { Type: FileChanged.ChangedType.Delete } item && item.Name == e.FullPath.GetName() && item.FullName != e.FullPath)
            {
                _ = App.FilesChangedList.Remove(item);
                App.FilesChangedList.Add(new FileChanged(e.FullPath, FileChanged.ChangedType.Move, item.Path));
            }
            else App.FilesChangedList.Add(new FileChanged(e.FullPath, FileChanged.ChangedType.Create));
        });
    }

    private new static void Renamed(object sender, RenamedEventArgs e)
    {
        _ = App.Window.DispatcherQueue.TryEnqueue
        (
            () => App.FilesChangedList.Add(new FileChanged(e.FullPath, FileChanged.ChangedType.Rename, e.OldFullPath.GetName()))
        );
    }

    private new static void Deleted(object sender, FileSystemEventArgs e)
    {
        _ = App.Window.DispatcherQueue.TryEnqueue
        (
            () => App.FilesChangedList.Add(new FileChanged(e.FullPath, FileChanged.ChangedType.Delete))
        );
    }
}