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
			var vm = Services.FilePropertiesService.Load(this, file);
			DataContext = vm;
			InitializeComponent();
		}
	}
}
