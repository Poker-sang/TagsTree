// #define FIRST_TIME

using Microsoft.UI.Xaml;
using WinUI3Utilities;

namespace TagsTree;

public partial class App : Application
{
    public const string AppName = nameof(TagsTree);

    public App()
    {
        InitializeComponent();
        SettingsValueConverter.Context = ConfigSerializeContext.Default;
        AppContext.Initialize();
    }

    public static MainWindow MainWindow { get; private set; } = null!;

    /// <param name="args">Details about the launch request and process.</param>
    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        MainWindow = new();
        MainWindow.Initialize(new() { Size = WindowHelper.EstimatedWindowSize() });
        MainWindow.Activate();
    }
}
