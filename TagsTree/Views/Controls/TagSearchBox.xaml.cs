using System.Windows;
using ModernWpf;
using ModernWpf.Controls;
using System.Windows.Controls;
using TagsTree.Delegates;
using TagsTree.Services.Controls;
using TagsTree.ViewModels.Controls;

namespace TagsTree.Views.Controls
{
	/// <summary>
	/// TagSearchBox.xaml 的交互逻辑
	/// </summary>
	public partial class TagSearchBox : UserControl
	{

		public TagSearchBox()
		{
			InitializeComponent();
			TagSearchBoxService.Load(this);
			AutoSuggestBox.TextChanged += TagSearchBoxViewModel.TextChanged;
			AutoSuggestBox.QuerySubmitted += TagSearchBoxViewModel.QuerySubmitted;
			AutoSuggestBox.SuggestionChosen += TagSearchBoxViewModel.SuggestionChosen;
		}

		public TypedEventHandler<AutoSuggestBox, AutoSuggestBoxQuerySubmittedEventArgs> BeforeQuerySubmitted
		{
			set
			{
				AutoSuggestBox.QuerySubmitted -= TagSearchBoxViewModel.QuerySubmitted;
				AutoSuggestBox.QuerySubmitted += value;
				AutoSuggestBox.QuerySubmitted += TagSearchBoxViewModel.QuerySubmitted;
			}
		}

		public event ResultChangedEventHandler ResultChanged;

		public void OnResultChanged(ResultChangedEventArgs e) => ResultChanged.Invoke(this, e);
	}
}
