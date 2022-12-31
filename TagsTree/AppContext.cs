using Windows.Storage;
using WinUI3Utilities.Attributes;

namespace TagsTree;

[AppContext<AppConfig>]
public static partial class AppContext
{
    public static string AppLocalFolder { get; private set; } = null!;

    public static void Initialize()
    {
        AppLocalFolder = ApplicationData.Current.LocalFolder.Path;
        InitializeConfigurationContainer();
    }
}
