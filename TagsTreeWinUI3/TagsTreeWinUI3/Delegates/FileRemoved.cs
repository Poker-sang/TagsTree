using TagsTreeWinUI3.ViewModels;
using TagsTreeWinUI3.Views.Controls;

namespace TagsTreeWinUI3.Delegates
{
	public delegate void FileRemovedEventHandler(FileProperties sender, FileRemovedEventArgs e);
	public class FileRemovedEventArgs
	{
		public FileRemovedEventArgs(FileViewModel removedItem) => RemovedItem = removedItem;
		public FileViewModel RemovedItem { get; }
	}
}
