using ModernWpf.Controls;

namespace TagsTree.Views
{
	/// <summary>
	/// FileProperties.xaml 的交互逻辑
	/// </summary>
	public partial class FileProperties : ContentDialog
	{
		public FileProperties()
		{
			Services.FilePropertiesService.Load(this);
			InitializeComponent();
		}
	}
}
