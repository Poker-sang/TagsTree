using ModernWpf.Controls;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;
using TagsTree.ViewModels;

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
			if (!((MainViewModel)DataContext).CheckConfig())
			{
				Close();
				return;
			}
			Services.MainService.LoadFileProperties();

			MouseLeftButtonDown += ((MainViewModel)DataContext).MainMouseLeftButtonDown;
			((Style)Resources["DgRowStyle"]).Setters.Add(new EventSetter(MouseDoubleClickEvent, ((MainViewModel)DataContext).DgItemMouseDoubleClick));
			TbInput.SuggestionChosen += ((MainViewModel)DataContext).SuggestionChosen;
			TbInput.TextChanged += ((MainViewModel)DataContext).TextChanged;
			TbInput.QuerySubmitted += TbInput_OnQuerySubmitted + ((MainViewModel)DataContext).QuerySubmitted;
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
	}
}