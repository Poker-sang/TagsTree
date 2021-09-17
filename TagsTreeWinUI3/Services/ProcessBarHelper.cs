using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Windows.UI;

namespace TagsTreeWinUI3.Services
{
    public class ProcessBarHelper
    {
        public ProcessBarHelper(Panel panel)
        {
            _border = new Border { Background = new SolidColorBrush(Color.FromArgb(0x55, 0x88, 0x88, 0x88)) };
            _progressBar = new ProgressBar { Width = 300, Height = 20 };
            _progressBar.ValueChanged += (_, _) => _border.Child.UpdateLayout();
            _border.Child = _progressBar;
            panel.Children.Add(_border);
            _panel = panel;
        }

        public void Dispose() => _panel.Children.Remove(_border);

        private readonly Panel _panel;
        private readonly Border _border;
        private readonly ProgressBar _progressBar;

        public double ProcessValue //TODO 不返回值
        {
            get
            {
                double temp = 0;
                _ = _progressBar.DispatcherQueue.TryEnqueue(() => temp = _progressBar.Value);
                return temp;
            }
            set => _ = _progressBar.DispatcherQueue.TryEnqueue(() => _progressBar.Value = value);
        }
    }
}