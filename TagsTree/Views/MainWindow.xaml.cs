using System.Text.RegularExpressions;
using System.Windows;
using TagsTree.Services;
using static TagsTree.Properties.Settings;

namespace TagsTree.Views
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			while (true)
			{
				if (Default.IsSet) //如果之前有储存过用户配置，则判断是否符合
				{
					var legalPath = new Regex(@"^[a-zA-Z]:\\[^\/\:\*\?\""\<\>\|\,]+$");
					if (legalPath.IsMatch(Default.ConfigPath) && legalPath.IsMatch(Default.LibraryPath))
					{
						if (TagsTreeStatic.LoadConfig(Default.ConfigPath))
							break;
					}
					else
					{
						App.ErrorMessageBox("配置文件损坏！请重新输入");
						Default.IsSet = false;
					}
				}
				else if (new NewConfig(this).ShowDialog() == false)
				{
					Close();
					return;
				}
			}
			InitializeComponent();
		}

		private void ChangeConfig_Click(object sender, RoutedEventArgs e) => _ = new NewConfig(this).ShowDialog();

		private void TagsManager_Click(object sender, RoutedEventArgs e) => _ = new TagsManager(this).ShowDialog();

		private void FileAdder_OnClick(object sender, RoutedEventArgs e) => _ = new FileImporter(this).ShowDialog();
	}
}