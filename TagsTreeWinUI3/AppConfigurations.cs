using TagsTreeWinUI3.Services.ExtensionMethods;
using Windows.Storage;

namespace TagsTreeWinUI3
{
    public record AppConfigurations
    {
        public int Theme { get; set; }
        public string LibraryPath { get; set; }
        public bool PathTagsEnabled { get; set; }
        public bool FilesObserverEnabled { get; set; }

        private static ApplicationDataContainer _configurationContainer = null!;

        private const string ConfigurationContainerKey = "Config";
        public static string AppLocalFolder { get; private set; } = null!;

        private AppConfigurations(int theme, string libraryPath, bool pathTagsEnabled, bool filesObserverEnabled)
        {
            Theme = theme;
            LibraryPath = libraryPath;
            PathTagsEnabled = pathTagsEnabled;
            FilesObserverEnabled = filesObserverEnabled;
        }

        public static void Initialize()
        {
            AppLocalFolder = ApplicationData.Current.LocalFolder.Path;
            if (!ApplicationData.Current.RoamingSettings.Containers.ContainsKey(ConfigurationContainerKey))
                _ = ApplicationData.Current.RoamingSettings.CreateContainer(ConfigurationContainerKey, ApplicationDataCreateDisposition.Always);

            _configurationContainer = ApplicationData.Current.RoamingSettings.Containers[ConfigurationContainerKey];
        }


        public static AppConfigurations? LoadConfiguration()
        {
            try
            {
                return new AppConfigurations(
                    _configurationContainer.Values[nameof(Theme)].CastThrow<int>(),
                    _configurationContainer.Values[nameof(LibraryPath)].CastThrow<string>(),
                    _configurationContainer.Values[nameof(PathTagsEnabled)].CastThrow<bool>(),
                    _configurationContainer.Values[nameof(FilesObserverEnabled)].CastThrow<bool>()
                    );
            }
            catch
            {
                return null;
            }
        }

        public static AppConfigurations GetDefault() => new(0, "", true, false);

        public static void SaveConfiguration(AppConfigurations appConfigurations)
        {
            _configurationContainer.Values[nameof(Theme)] = appConfigurations.Theme;
            _configurationContainer.Values[nameof(LibraryPath)] = appConfigurations.LibraryPath;
            _configurationContainer.Values[nameof(PathTagsEnabled)] = appConfigurations.PathTagsEnabled;
            _configurationContainer.Values[nameof(FilesObserverEnabled)] = appConfigurations.FilesObserverEnabled;
        }
    }
}
