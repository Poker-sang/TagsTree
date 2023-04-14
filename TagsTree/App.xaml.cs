// #define DISABLE_XAML_GENERATED_BREAK_ON_UNHANDLED_EXCEPTION
// #define DISABLE_XAML_GENERATED_BINDING_DEBUG_OUTPUT
// #define FIRST_TIME

using Microsoft.UI.Xaml;
using WinUI3Utilities;

namespace TagsTree;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        CurrentContext.Title = nameof(TagsTree);
        AppContext.Initialize();
    }

    /// <param name="args">Details about the launch request and process.</param>
    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        _ = new MainWindow();

        AppHelper.Initialize(new()
        {
            Size = WindowHelper.EstimatedWindowSize()
        });
    }
}
