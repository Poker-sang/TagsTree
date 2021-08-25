using TagsTreeWpf.ViewModels;
using TagsTreeWpf.Views.Controls;

namespace TagsTreeWpf.Delegates
{
	public delegate void FileRemovedEventHandler(FileProperties sender, FileRemovedEventArgs e);
	public class FileRemovedEventArgs
	{
		public FileRemovedEventArgs(FileViewModel removedItem) => RemovedItem = removedItem;
		public FileViewModel RemovedItem { get; }
	}
}
