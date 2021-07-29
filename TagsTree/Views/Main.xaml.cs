using ModernWpf.Controls;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace TagsTree.Views
{
	/// <summary>
	/// Interaction logic for Main.xaml
	/// </summary>
	public partial class Main : Window
	{
		public Main()
		{
			var vm = Services.MainService.Load(this);
			if (!vm.CheckConfig())
			{
				Close();
				return;
			}
			DataContext = vm;
			InitializeComponent();
			MouseLeftButtonDown += (sender, e) =>
			{
				if (e.GetPosition((Main)sender).X is < 215 or > 1065)
					Cd.Hide();
			};

			Cd.ShowAsync(ContentDialogPlacement.Popup);
			((Style)Resources["DgRowStyle"]).Setters.Add(new EventSetter(MouseDoubleClickEvent, vm.DgItemMouseDoubleClick));
			TbInput.SuggestionChosen += vm.SuggestionChosen;
			TbInput.TextChanged += vm.TextChanged;
			TbInput.QuerySubmitted += TbInput_OnQuerySubmitted + vm.QuerySubmitted;
		}


		private static async void Search(IAnimatable textBlock, IAnimatable textBox, UIElement dataGrid)
		{
			textBlock.BeginAnimation(MarginProperty, new ThicknessAnimation
			{
				From = new Thickness(0, 0, 0, 0),
				To = new Thickness(0, -350, 0, 0),
				Duration = TimeSpan.FromMilliseconds(1000)
			});
			textBox.BeginAnimation(MarginProperty, new ThicknessAnimation
			{
				From = new Thickness(0, 300, 0, 0),
				To = new Thickness(0, 80, 0, 0),
				Duration = TimeSpan.FromMilliseconds(1000)
			});
			await Task.Delay(1000);
			dataGrid.BeginAnimation(OpacityProperty, new DoubleAnimation
			{
				From = 0,
				To = 1,
				Duration = TimeSpan.FromMilliseconds(1000)
			});
			dataGrid.IsHitTestVisible = true;
		}

		private bool _isSearched;
		private void TbInput_OnQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
		{
			if (_isSearched) return;
			Search(TbBanner, TbInput, DgResult);
			_isSearched = true;
		}


		private void ChangeConfig_Click(object sender, RoutedEventArgs e) => _ = new NewConfig(this).ShowDialog();
		private void TagsManager_Click(object sender, RoutedEventArgs e) => _ = new TagsManager(this).ShowDialog();
		private void FileAdder_OnClick(object sender, RoutedEventArgs e) => _ = new FileImporter(this).ShowDialog();

		private void UIElement_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			Cd.Hide();
		}
	}
}