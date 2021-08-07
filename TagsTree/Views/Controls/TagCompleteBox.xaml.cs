using System.Windows.Controls;
using Vm = TagsTree.ViewModels.Controls.TagCompleteBoxViewModel;

namespace TagsTree.Views.Controls
{
	/// <summary>
	/// TagCompleteBox.xaml 的交互逻辑
	/// </summary>
	public partial class TagCompleteBox : UserControl
	{
		public TagCompleteBox()
		{
			InitializeComponent();
			Services.Controls.TagCompleteBoxService.Load(this);
			AutoSuggestBox.LostFocus += Vm.PathComplement;
			AutoSuggestBox.TextChanged += Vm.PathChanged;
			AutoSuggestBox.SuggestionChosen += Vm.SuggestionChosen;
		}
	}
}
