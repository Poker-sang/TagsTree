using Microsoft.UI.Xaml;
using System.Collections.ObjectModel;
using System.IO;
using TagsTreeWinUI3.Models;
using TagsTreeWinUI3.Services;
using TagsTreeWinUI3.Services.ExtensionMethods;
using TagsTreeWinUI3.ViewModels;
using Windows.Storage;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace TagsTreeWinUI3
{
	/// <summary>
	/// Provides application-specific behavior to supplement the default Application class.
	/// </summary>
	public partial class App : Application
	{
		public static MainWindow Window = null!;
		public static AppConfigurations AppConfigurations { get; set; } = null!;
		public static bool ConfigSet { get; set; }

		/// <summary>
		/// Initializes the singleton application object.  This is the first line of authored code
		/// executed, and as such is the logical equivalent of main() or WinMain().
		/// </summary>
		public App()
		{
			InitializeComponent();
			AppConfigurations.Initialize();
			if (AppConfigurations.LoadConfiguration() is { } appConfigurations)
				if (!Directory.Exists(appConfigurations.ProxyPath))
					MessageDialogX.Information(true, "配置路径不存在！");
				else
				{
					AppConfigurations = appConfigurations;
					ConfigSet = true;
				}
			else
			{
				AppConfigurations = AppConfigurations.GetDefault();
				ConfigSet = false;
			}
			RequestedTheme = AppConfigurations.Theme ? ApplicationTheme.Dark : ApplicationTheme.Light;
		}

		/// <summary>
		/// Invoked when the application is launched normally by the end user.  Other entry points
		/// will be used such as when the application is launched to open a specific file.
		/// </summary>
		/// <param name="args">Details about the launch request and process.</param>
		protected override void OnLaunched(LaunchActivatedEventArgs args)
		{
			Window = new MainWindow { ExtendsContentIntoTitleBar = true };
			WindowHelper.SetWindowSize(1280, 720);
			Window.Activate();
		}

		public static string TagsPath => AppConfigurations.ProxyPath + @"\TagsTree.json";
		public static string FilesPath => AppConfigurations.ProxyPath + @"\Files.json";
		public static string RelationsPath => AppConfigurations.ProxyPath + @"\Relations.xml";

		/// <summary>
		/// 保存标签
		/// </summary>
		public static void SaveTags(ObservableCollection<TagModel> tags) => Serialization.Serialize(TagsPath, tags);

		/// <summary>
		/// 保存文件
		/// </summary>
		public static void SaveFiles() => IdFile.Serialize(FilesPath);

		/// <summary>
		/// 保存关系
		/// </summary>
		public static void SaveRelations() => Relations.Save(RelationsPath);


		/// <summary>
		/// 所有标签
		/// </summary>
		public static readonly TreeDictionary Tags = new();

		/// <summary>
		/// 所有标签
		/// </summary>
		public static readonly BidirectionalDictionary<int, FileModel> IdFile = new();

		/// <summary>
		/// 所有关系
		/// </summary>
		public static readonly RelationsDataTable Relations = new();

		public static bool TryRemoveFileModel(FileViewModel fileViewModel)
		{
			var fileModel = fileViewModel.GetFileModel;
			if (!IdFile.Contains(fileModel)) return false;
			_ = IdFile.Remove(fileModel);
			Relations.Rows.Remove(Relations.RowAt(fileModel));
			Relations.RefreshRowsDict();
			SaveFiles();
			SaveRelations();
			return true;
		}
	}
}
