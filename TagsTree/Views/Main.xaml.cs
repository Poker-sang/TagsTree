using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;
using TagsTree.Views.Controls;
using Vm = TagsTree.ViewModels.MainViewModel;

namespace TagsTree.Views
{
	/// <summary>
	/// Interaction logic for Main.xaml
	/// </summary>
	public partial class Main : Window
	{
		public Main()
		{
			InitializeComponent();
			Services.MainService.Load(this);
			if (!Vm.CheckConfig())
			{
				Close();
				return;
			}
			Services.MainService.LoadFileProperties();

			MouseLeftButtonDown += Vm.MainMouseLeftButtonDown;
			((Style)Resources["DgRowStyle"]).Setters.Add(new EventSetter(MouseDoubleClickEvent, Vm.DgItemMouseDoubleClick));
			TbSearch.BeforeQuerySubmitted = (_, _) => Search();
			TbSearch.ResultChanged += Vm.ResultChanged;
			FileProperties.FileRemoved += Vm.FileRemoved;
			TbFuzzySearch.TextChanged += Vm.TextChanged;
			TbFuzzySearch.QuerySubmitted += Vm.QuerySubmitted;
		}

		private bool _isSearched;
		private async void Search()
		{
			if (_isSearched) return;
			_isSearched = true;
			TbBanner.BeginAnimation(MarginProperty, new ThicknessAnimation
			{
				From = new Thickness(0, 0, 0, 0),
				To = new Thickness(0, -350, 0, 0),
				Duration = TimeSpan.FromMilliseconds(1000)
			});
			TbSearch.BeginAnimation(MarginProperty, new ThicknessAnimation
			{
				From = new Thickness(0, 300, 0, 0),
				To = new Thickness(0, 50, 0, 0),
				Duration = TimeSpan.FromMilliseconds(1000)
			});
			await Task.Delay(1000);
			TbFuzzySearch.BeginAnimation(OpacityProperty, new DoubleAnimation
			{
				From = 0,
				To = 1,
				Duration = TimeSpan.FromMilliseconds(1000)
			});
			DgResult.BeginAnimation(OpacityProperty, new DoubleAnimation
			{
				From = 0,
				To = 1,
				Duration = TimeSpan.FromMilliseconds(1000)
			});
			TbFuzzySearch.IsHitTestVisible = true;
			DgResult.IsHitTestVisible = true;
		}

		private void TagsManager_Click(object sender, RoutedEventArgs e) => _ = new TagsManager(this).ShowDialog();
		private void FileAdder_OnClick(object sender, RoutedEventArgs e) => _ = new FileImporter(this).ShowDialog();
		private void TagEditFiles_OnClick(object sender, RoutedEventArgs e) => _ = new TagEditFiles(this).ShowDialog();
		private void ChangeConfig_Click(object sender, RoutedEventArgs e)
		{
			if (!App.MessageBoxX.Warning("更改设置后需要重启软件，请确保已保存")) return;
			if (new NewConfig(this).ShowDialog() == true) Close();
		}
	}
}