using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Animation;
using Vm = TagsTree.ViewModels.TagAddFilesViewModel;

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
			_ = Tags.SetBinding(ItemsControl.ItemsSourceProperty, new Binding(".") { Mode = BindingMode.TwoWay, Source = ((Vm)DataContext).Xdp });

			Tags.SelectedItemChanged += Vm.TvSelectItemChanged;
			TbInput.SuggestionChosen += Vm.SuggestionChosen;
			TbInput.TextChanged += Vm.TextChanged;
			TbInput.QuerySubmitted += Vm.QuerySubmitted;

			DgResult.RowStyle = new Style(typeof(DataGridRow), (Style)Application.Current.Resources["DefaultDataGridRowStyle"]);
			DgResult.RowStyle.Setters.Add(new EventSetter(MouseLeftButtonDownEvent, new MouseButtonEventHandler((_, _) => Vm.Selected(DgResult.CurrentItem))));
			DgResult.SelectionChanged += (_, _) => Vm.Selected(DgResult.CurrentItem);
		}


		public async void BConfirmClick()
		{
			Tags.BeginAnimation(OpacityProperty, new DoubleAnimation
			{
				From = 1,
				To = 0,
				Duration = TimeSpan.FromMilliseconds(500)
			});
			await Task.Delay(500);
			StackPanel.Children.Remove(Tags);
			BConfirm.Content = "保存";
			TbInput.BeginAnimation(OpacityProperty, new DoubleAnimation
			{
				From = 0,
				To = 1,
				Duration = TimeSpan.FromMilliseconds(500)
			});
			DgResult.BeginAnimation(OpacityProperty, new DoubleAnimation
			{
				From = 0,
				To = 1,
				Duration = TimeSpan.FromMilliseconds(500)
			});
			TbInput.IsHitTestVisible = true;
			DgResult.IsHitTestVisible = true;
		}

	}
}
