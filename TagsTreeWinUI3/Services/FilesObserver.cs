using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TagsTreeWinUI3.Models;
using TagsTreeWinUI3.Services.ExtensionMethods;

namespace TagsTreeWinUI3.Services
{
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
            if (!Directory.Exists(App.AppConfigurations.LibraryPath))
            {
                await MessageDialogX.Information(true, $"路径「{App.AppConfigurations.LibraryPath}」不存在，无法开启文件监视，请在设置修改正确路径后保存");
                return App.FilesObserver.EnableRaisingEvents = false;
            }

            Path = path; //不能是错误路径
            return App.FilesObserver.EnableRaisingEvents = App.AppConfigurations.FilesObserverEnabled;
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
}
