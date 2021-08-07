using TagsTree.ViewModels;
using TagsTree.Views.Controls;

namespace TagsTree.Delegates
{
	public delegate void FileRemovedEventHandler(FileProperties sender, FileRemovedEventArgs e);
	public class FileRemovedEventArgs
	{
		public FileRemovedEventArgs(FileViewModel removedItem) => RemovedItem = removedItem;
		public FileViewModel RemovedItem { get; }
	}
}
