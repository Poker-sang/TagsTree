using System.Text.RegularExpressions;
using System.Windows;

namespace TagsTree.Views
{
	/// <summary>
	/// InputName.xaml 的交互逻辑
	/// </summary>
	public partial class InputName : Window
	{
		public InputName(Window owner, string hintMessage, string invalidRegex, string text = "")
		{
			Owner = owner;
			_invalidRegex = invalidRegex;
			_hintMessage = hintMessage is "" ? invalidRegex : hintMessage;
			InitializeComponent();
			AsBox.PlaceholderText = _hintMessage;
			if (text is "") return;
			AsBox.Text = text;
		}

		public string Message = "";
		private readonly string _invalidRegex;
		private readonly string _hintMessage;

		private void BConfirm_OnClick(object sender, RoutedEventArgs e)
		{
			if (!new Regex($@"^[^{_invalidRegex}]+$").IsMatch(AsBox.Text))
			{
				App.MessageBoxX.Error(_hintMessage);
				return;
			}
			Message = AsBox.Text;
			DialogResult = true;
			Close();
		}
	}
}
