using System.Windows;
using System.Windows.Controls.Primitives;
using ModernWpf.Controls;

namespace TagsTree.Views
{
	/// <summary>
	/// FileImporter.xaml 的交互逻辑
	/// </summary>
	public partial class FileImporter : Window
	{
		public FileImporter(Window owner)
		{
			Owner = owner;
			InitializeComponent();
			Services.FileImporterService.Win = this;
			var vm = Services.FileImporterService.Vm;
			MouseLeftButtonDown += (_, _) => DragMove();
			DataContext = vm;
		}
	}
}
