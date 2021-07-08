using System.Windows;
using System.Windows.Controls.Primitives;
using ModernWpf.Controls;

namespace TagsTree
{
	/// <summary>
	/// FileAdder.xaml 的交互逻辑
	/// </summary>
	public partial class FileAdder : Window
	{
		public FileAdder()
		{
			InitializeComponent();

			//var a = new CommonOpenFileDialog()
			//{
			//	EnsureFileExists = true,
			//	EnsurePathExists = true,
			//	Multiselect = true,
			//	IsFolderPicker = true
			//}; if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
			//a.ShowDialog();
				//foreach (var VARIABLE in a.FileNames)
				//{
				//	var c = a.FileNames.Count();
				//	var b = VARIABLE;
				//}

			}

		private void ButtonBase_OnClick(object sender, RoutedEventArgs e) =>
			((ToggleButton)sender).Content = (ToggleButton)sender switch
			{
				{ IsChecked: false } => new FontIcon { Glyph = "\uE7C3" },
				{ IsChecked: true } => new FontIcon { Glyph = "\uE8B7" },
				_ => ((ToggleButton)sender).Content
			};
	}
}
