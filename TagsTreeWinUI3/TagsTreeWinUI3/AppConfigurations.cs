using TagsTreeWinUI3.Services.ExtensionMethods;
using Windows.Storage;

namespace TagsTreeWinUI3
{
	public class AppConfigurations
	{

		public bool Theme { get; set; }
		public string LibraryPath { get; set; }
		public string ProxyPath { get; set; }
		public bool PathTags { get; set; }

		private static ApplicationDataContainer _configurationContainer = null!;

		private const string ConfigurationContainerKey = "Config";
		public static StorageFolder AppLocalFolder { get; set; } = null!;

		public AppConfigurations(bool theme, string libraryPath, string proxyPath, bool pathTags)
		{
			Theme = theme;
			LibraryPath = libraryPath;
			ProxyPath = proxyPath;
			PathTags = pathTags;
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
					_configurationContainer.Values[nameof(ProxyPath)].CastThrow<string>(),
					_configurationContainer.Values[nameof(PathTags)].CastThrow<bool>()
					);
			}
			catch
			{
				return null;
			}
		}

		public static AppConfigurations GetDefault() => new(false, "", "", true);

		public static void SaveConfiguration(AppConfigurations appConfigurations)
		{
			_configurationContainer.Values[nameof(Theme)] = appConfigurations.Theme;
			_configurationContainer.Values[nameof(LibraryPath)] = appConfigurations.LibraryPath;
			_configurationContainer.Values[nameof(ProxyPath)] = appConfigurations.ProxyPath;
			_configurationContainer.Values[nameof(PathTags)] = appConfigurations.PathTags;
		}
	}
}
