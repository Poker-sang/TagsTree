using System;
using System.Threading.Tasks;
using System.Windows.Controls;
using ModernWpf.Controls;
using TagsTree.Delegates;
using ModernWpf;
using TagsTree.Services.Controls;
using TagsTree.ViewModels.Controls;

namespace TagsTree.Views.Controls
{
	/// <summary>
	/// TagSuggestBox.xaml 的交互逻辑
	/// </summary>
	public partial class TagSuggestBox : UserControl
	{

		public TagSuggestBox()
		{
			InitializeComponent();
			TagSuggestBoxService.Load(this);
			AutoSuggestBox.TextChanged += TagSuggestBoxViewModel.TextChanged;
			AutoSuggestBox.QuerySubmitted += TagSuggestBoxViewModel.QuerySubmitted;
			AutoSuggestBox.SuggestionChosen += TagSuggestBoxViewModel.SuggestionChosen;
		}

		public TypedEventHandler<AutoSuggestBox, AutoSuggestBoxQuerySubmittedEventArgs> BeforeQuerySubmitted
		{
			set
			{
				AutoSuggestBox.QuerySubmitted -= TagSuggestBoxViewModel.QuerySubmitted;
				AutoSuggestBox.QuerySubmitted += value;
				AutoSuggestBox.QuerySubmitted += TagSuggestBoxViewModel.QuerySubmitted;
			}
		}

		public event ResultChangedEventHandler ResultChanged;
		public void OnResultChanged(ResultChangedEventArgs e) => ResultChanged.Invoke(this, e);
	}
}
