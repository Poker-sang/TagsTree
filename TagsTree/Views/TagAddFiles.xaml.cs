using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using TagsTree.ViewModels;

namespace TagsTree.Views
{
	/// <summary>
	/// TagAddFiles.xaml 的交互逻辑
	/// </summary>
	public partial class TagAddFiles : Window
	{
		public TagAddFiles(Window owner)
		{
			Owner = owner;
			InitializeComponent();
			Services.TagAddFilesService.Load(this);
			MouseLeftButtonDown += (_, _) => DragMove();
			_ = Tags.SetBinding(ItemsControl.ItemsSourceProperty, new Binding(".") { Mode = BindingMode.TwoWay, Source = ((TagAddFilesViewModel)DataContext).Xdp });

			Tags.SelectedItemChanged += TagAddFilesViewModel.TvSelectItemChanged;
			Tags.MouseDoubleClick += (_, _) =>
			{
				BConfirmClick();
				TagAddFilesViewModel.ConfirmBClick();
			};
			BConfirm.Click += (_, _) =>
			{
				BConfirmClick();
				TagAddFilesViewModel.ConfirmBClick();
			};
		}

		public bool Mode = false;

		private void BConfirmClick()
		{
			if (!Mode) //Mode变为true由ConfirmBClick()完成
			{





			}
			else Close();
		}

	}
}
