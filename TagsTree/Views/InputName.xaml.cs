using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static System.EventArgs;

namespace TagsTree
{
	/// <summary>
	/// InputName.xaml 的交互逻辑
	/// </summary>
	public partial class InputName : Window
	{
		public InputName()
		{
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
