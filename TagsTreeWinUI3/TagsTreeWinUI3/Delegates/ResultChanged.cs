using System.Collections.Generic;
using TagsTreeWinUI3.Models;
using TagsTreeWinUI3.Views.Controls;

namespace TagsTreeWinUI3.Delegates
{
	public delegate void ResultChangedEventHandler(TagSearchBox sender, ResultChangedEventArgs e);
	public class ResultChangedEventArgs
	{
		public ResultChangedEventArgs(IEnumerable<FileModel> newResult) => NewResult = newResult;
		public IEnumerable<FileModel> NewResult { get; }
	}
}
