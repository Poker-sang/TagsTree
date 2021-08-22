using System.Collections.Generic;
using TagsTree.Models;
using TagsTree.Views.Controls;

namespace TagsTree.Delegates
{
	public delegate void ResultChangedEventHandler(TagSearchBox sender, ResultChangedEventArgs e);
	public class ResultChangedEventArgs
	{
		public ResultChangedEventArgs(IEnumerable<FileModel> newResult) => NewResult = newResult;
		public IEnumerable<FileModel> NewResult { get; }
	}
}
