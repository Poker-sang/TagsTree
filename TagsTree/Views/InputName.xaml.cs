using System.Windows;

namespace TagsTree.Views
{
	/// <summary>
	/// InputName.xaml 的交互逻辑
	/// </summary>
	public partial class InputName : Window
	{
		public InputName(Window owner)
		{
			Owner = owner;
			InitializeComponent();
		}

		public string Message = "";

		private void BConfirm_OnClick(object sender, RoutedEventArgs e)
		{
			Message = TextBox.Text;
			DialogResult = true;
			Close();
		}
	}
}
