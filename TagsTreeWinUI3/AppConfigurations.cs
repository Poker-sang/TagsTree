using TagsTreeWinUI3.Services.ExtensionMethods;
using Windows.Storage;

namespace TagsTreeWinUI3
{
	public class AppConfigurations
	{

		public bool Theme { get; set; }
		public string LibraryPath { get; set; }
		public bool PathTagsEnabled { get; set; }
		private bool _filesObserverEnabled;
		public bool FilesObserverEnabled
		{
			get => _filesObserverEnabled;
			set => _filesObserverEnabled = App.FilesObserver.EnableRaisingEvents = App.Window.SetFilesObserverEnable = value;
		}

		private static ApplicationDataContainer _configurationContainer = null!;

		private const string ConfigurationContainerKey = "Config";
		public static StorageFolder AppLocalFolder { get; set; } = null!;

		private AppConfigurations(bool theme, string libraryPath, bool pathTagsEnabled, bool filesObserverEnabled)
		{
			Theme = theme;
			LibraryPath = libraryPath;
			PathTagsEnabled = pathTagsEnabled;
			_filesObserverEnabled = filesObserverEnabled;
		}

		public static void Initialize()
		{
			AppLocalFolder = ApplicationData.Current.LocalFolder;
			if (!ApplicationData.Current.RoamingSettings.Containers.ContainsKey(ConfigurationContainerKey))
				ApplicationData.Current.RoamingSettings.CreateContainer(ConfigurationContainerKey, ApplicationDataCreateDisposition.Always);

			_configurationContainer = ApplicationData.Current.RoamingSettings.Containers[ConfigurationContainerKey];
		}


		public static AppConfigurations? LoadConfiguration()
		{
			try
			{
				return new AppConfigurations(
					_configurationContainer.Values[nameof(Theme)].CastThrow<bool>(),
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

		public static AppConfigurations GetDefault() => new(false, "", true, false);

		public static void SaveConfiguration(AppConfigurations appConfigurations)
		{
			_configurationContainer.Values[nameof(Theme)] = appConfigurations.Theme;
			_configurationContainer.Values[nameof(LibraryPath)] = appConfigurations.LibraryPath;
			_configurationContainer.Values[nameof(PathTagsEnabled)] = appConfigurations.PathTagsEnabled;
			_configurationContainer.Values[nameof(FilesObserverEnabled)] = appConfigurations.FilesObserverEnabled;
		}
	}
}
