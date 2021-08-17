using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace TagsTree.Views
{
	/// <summary>
	/// InputName.xaml 的交互逻辑
	/// </summary>
	public partial class InputName : Window
	{
		public InputName(Window owner, App.FileX.InvalidMode mode, string text = "")
		{
			Owner = owner;
			InitializeComponent();
			switch (mode)
			{
				case App.FileX.InvalidMode.Name:
					AsBox.PlaceholderText = @"不能包含\/:*?""<>|和除空格外的空白字符";
					_invalidRegex = App.FileX.GetInvalidNameChars;
					break;
				case App.FileX.InvalidMode.Path:
					AsBox.PlaceholderText = @"不能包含/:*?""<>|和除空格外的空白字符";
					_invalidRegex = App.FileX.GetInvalidPathChars;
					break;
				default: throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
			}
			AsBox.Text = text;
			_ = Dispatcher.BeginInvoke(DispatcherPriority.Background, (Action)(() => Keyboard.Focus(AsBox)));
		}

		public string Message = "";
		private readonly string _invalidRegex;

		private void BConfirm_OnClick(object sender, RoutedEventArgs e)
		{
			if (!new Regex($@"^[^{_invalidRegex}]+$").IsMatch(AsBox.Text))
			{
				App.MessageBoxX.Error(AsBox.PlaceholderText);
				return;
			}
			Message = AsBox.Text;
			DialogResult = true;
			Close();
		}
	}
}
