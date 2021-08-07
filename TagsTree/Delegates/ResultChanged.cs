using System.Collections.Generic;
using TagsTree.ViewModels;
using TagsTree.Views.Controls;

namespace TagsTree.Delegates
{
	public delegate void ResultChangedEventHandler(TagSuggestBox sender, ResultChangedEventArgs e);
	public class ResultChangedEventArgs
	{
		public ResultChangedEventArgs(IEnumerable<FileViewModel> newResult) => NewResult = newResult;
		public IEnumerable<FileViewModel> NewResult { get; }
	}
}
