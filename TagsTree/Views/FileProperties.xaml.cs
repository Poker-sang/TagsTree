using ModernWpf.Controls;
using System.Windows.Media;
using TagsTree.Models;

namespace TagsTree.Views
{
	/// <summary>
	/// FileProperties.xaml 的交互逻辑
	/// </summary>
	public partial class FileProperties : ContentDialog
	{
		public FileProperties(FileModel file)
		{
			InitializeComponent();
			Icon.Source = GetIcon(file.FullName);
			Title = "属性：" + file.Name;
		}
		public static ImageSource GetIcon(string fileName)
		{
			return CIconOfPath.IconOfPath(fileName, true, true,false);
		}
	}
}
