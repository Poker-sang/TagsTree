using System.Collections.Generic;
using TagsTreeWpf.Models;
using TagsTreeWpf.Views.Controls;

namespace TagsTreeWpf.Delegates
{
	public delegate void ResultChangedEventHandler(TagSearchBox sender, ResultChangedEventArgs e);
	public class ResultChangedEventArgs
	{
		public ResultChangedEventArgs(IEnumerable<FileModel> newResult) => NewResult = newResult;
		public IEnumerable<FileModel> NewResult { get; }
	}
}
