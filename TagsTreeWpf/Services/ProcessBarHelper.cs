using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace TagsTreeWpf.Services
{
	public class ProcessBarHelper
	{
		public ProcessBarHelper(Panel panel)
		{
			_border = new Border { Background = new SolidColorBrush(Color.FromArgb(0x55, 0x88, 0x88, 0x88)) };
			_progressBar = new ModernWpf.Controls.ProgressBar { Width = 300, Height = 20 };
			_progressBar.ValueChanged += (_, _) => _border.Child.UpdateLayout();
			_border.Child = _progressBar;
			_ = panel.Children.Add(_border);
			_panel = panel;
		}

		public void Dispose() => _panel.Children.Remove(_border);

		private readonly Panel _panel;
		private readonly Border _border;
		private readonly ModernWpf.Controls.ProgressBar _progressBar;

		public double ProcessValue
		{
			get => Application.Current.Dispatcher.Invoke(() => _progressBar.Value);
			set => _ = Application.Current.Dispatcher.Invoke(() => _progressBar.Value = value);
		}
	}
}