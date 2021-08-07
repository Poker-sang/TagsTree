using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using TagsTree.ViewModels;

namespace TagsTree.Views
{
	/// <summary>
	/// FileEditTags.xaml 的交互逻辑
	/// </summary>
	public partial class FileEditTags : Window
	{
		public FileEditTags(Window owner, FileViewModel fileViewModel)
		{
			Owner = owner;
			InitializeComponent();
			((FileEditTagsViewModel)DataContext).FileViewModel = fileViewModel;
			Services.FileEditTagsService.Load(this);
			MouseLeftButtonDown += (_, _) => DragMove();
			_ = Tags.SetBinding(ItemsControl.ItemsSourceProperty, new Binding(".") { Mode = BindingMode.TwoWay, Source = ((FileEditTagsViewModel)DataContext).Xdp });
			Tags.SelectedItemChanged += FileEditTagsViewModel.TvSelectItemChanged;
		}
	}
}
