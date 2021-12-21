using TagsTree.Attributes;
using Windows.Storage;

namespace TagsTree;

[LoadSaveConfiguration(typeof(AppConfiguration), nameof(_configurationContainer), CastMethod = "TagsTree.Services.ExtensionMethods.Misc.CastThrow")]
public static partial class AppContext
{

    private static ApplicationDataContainer _configurationContainer = null!;

    private const string ConfigurationContainerKey = "Config";
    public static string AppLocalFolder { get; private set; } = null!;


    public static void Initialize()
    {
        AppLocalFolder = ApplicationData.Current.LocalFolder.Path;
        if (!ApplicationData.Current.RoamingSettings.Containers.ContainsKey(ConfigurationContainerKey))
            _ = ApplicationData.Current.RoamingSettings.CreateContainer(ConfigurationContainerKey, ApplicationDataCreateDisposition.Always);

        _configurationContainer = ApplicationData.Current.RoamingSettings.Containers[ConfigurationContainerKey];
    }

    public static AppConfiguration GetDefault() => new(0, "", true, false);
}