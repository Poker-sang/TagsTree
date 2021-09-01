using CommunityToolkit.WinUI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using TagsTreeWinUI3.Models;
using TagsTreeWinUI3.Services;
using TagsTreeWinUI3.Services.ExtensionMethods;
using TagsTreeWinUI3.ViewModels;
using TagsTreeWinUI3.Views;
using AppDomain = System.AppDomain;
using Exception = System.Exception;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace TagsTreeWinUI3
{
	/// <summary>
	/// Provides application-specific behavior to supplement the default Application class.
	/// </summary>
	public partial class App : Application
	{
		public static MainWindow Window { get; private set; } = null!;
		public static Frame RootFrame => Window.NavigateFrame;
		public static AppConfigurations AppConfigurations { get; private set; } = null!;
		public static FilesObserver FilesObserver { get; private set; } = null!;
		public static ObservableCollection<FilesChanged> FilesChangedList => FilesObserverPage.Vm.FilesChangedList;

		public static bool ConfigSet
		{
			get => _configSet;
			set
			{
				if (_configSet != value && value)
					LoadConfig();
				_configSet = value;
			}
		}

		/// <summary>
		/// Initializes the singleton application object.  This is the first line of authored code
		/// executed, and as such is the logical equivalent of main() or WinMain().
		/// </summary>
		public App()
		{
			InitializeComponent();
			IconX.Initialize();
			FilesObserver = new FilesObserver();
			AppConfigurations.Initialize();
			if (AppConfigurations.LoadConfiguration() is { } appConfigurations)
				if (!Directory.Exists(appConfigurations.ProxyPath))
					MessageDialogX.Information(true, "配置路径不存在！");
				else
				{
					AppConfigurations = appConfigurations;
					_configSet = true;
				}
			else
			{
				AppConfigurations = AppConfigurations.GetDefault();
				_configSet = false;
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
			if (ConfigSet)
			{
				LoadConfig();
				Window.ConfigModeUnlock();
			}
			Window.Activate();
			FilesObserver.Initialize(AppConfigurations.LibraryPath);
		}
		
		public static string FilesChangedPath => AppConfigurations.ProxyPath + @"\FilesChanged.json";
		private static string TagsPath => AppConfigurations.ProxyPath + @"\TagsTree.json";
		private static string FilesPath => AppConfigurations.ProxyPath + @"\Files.json";
		private static string RelationsPath => AppConfigurations.ProxyPath + @"\Relations.xml";

		/// <summary>
		/// 保存标签
		/// </summary>
		public static void SaveTags(ObservableCollection<TagViewModel> tags) => Serialization.Serialize(TagsPath, tags);

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

		private static bool _configSet;

		public static bool TryRemoveFileModel(FileViewModel fileViewModel)
		{
			var fileModel = fileViewModel.GetFileModel();
			if (!IdFile.Contains(fileModel)) return false;
			_ = IdFile.Remove(fileModel);
			Relations.Rows.Remove(Relations.RowAt(fileModel));
			Relations.RefreshRowsDict();
			SaveFiles();
			SaveRelations();
			return true;
		}

		///  <summary>
		///  重新加载新的配置文件
		///  </summary>
		///  <returns>true：已填写正确地址，进入软件；false：打开设置页面；null：关闭软件</returns>
		private static async void LoadConfig()
		{
			//文件监视
			FilesObserverPage.Vm = new FilesObserverViewModel(FilesChanged.Deserialize(FilesChangedPath));

			//标签
			Tags.LoadTree(TagsPath);
			Tags.LoadDictionary();

			//文件
			IdFile.Deserialize(FilesPath);

			//关系
			if (!File.Exists(RelationsPath))
				_ = File.Create(RelationsPath);
			Relations.Load(RelationsPath); //异常在内部处理

			//检查
			if (Tags.TagsDictionary.Count != Relations.Columns.Count) //TagsDictionary第一个是总根标签，Relations第一列是文件Id 
			{
				if (await MessageDialogX.Warning($"路径「{AppConfigurations.ProxyPath}」下，TagsTree.xml和Relations.xml存储的标签数不同", "删除关系文件Relations.xml并重新生成", "直接关闭软件"))
				{
					File.Delete(RelationsPath);
					Relations.Load(RelationsPath);
				}
				else
				{
					Window.Close();
					return;
				}
			}
			if (IdFile.Count != Relations.Rows.Count)
			{
				if (await MessageDialogX.Warning($"「路径{AppConfigurations.ProxyPath}」下，Files.json和Relations.xml存储的文件数不同", "删除关系文件Relations.xml并重新生成", "直接关闭软件"))
				{
					File.Delete(RelationsPath);
					Relations.Load(RelationsPath);
				}
				else
				{
					Window.Close();
					return;
				}
			}
		}

		private void RegisterUnhandledExceptionHandler()
		{
			UnhandledException += async (_, args) =>
			{
				args.Handled = true;
				await Window.DispatcherQueue.EnqueueAsync(() => UncaughtExceptionHandler(args.Exception));
			};

			TaskScheduler.UnobservedTaskException += async (_, args) =>
			{
				args.SetObserved();
				await Window.DispatcherQueue.EnqueueAsync(() => UncaughtExceptionHandler(args.Exception));
			};

			AppDomain.CurrentDomain.UnhandledException += async (_, args) =>
			{
				if (args.ExceptionObject is Exception e)
				{
					await Window.DispatcherQueue.EnqueueAsync(() => UncaughtExceptionHandler(e));
				}
				else
				{
				}
			};

			static void UncaughtExceptionHandler(Exception e)
			{
#if DEBUG
				Debugger.Break();
#endif
				MessageDialogX.Information(true, e.ToString());
			}
		}
	}
}
