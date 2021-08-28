using Microsoft.UI.Xaml;
using System.Collections.ObjectModel;
using System.IO;
using TagsTreeWinUI3.Models;
using TagsTreeWinUI3.Services;
using TagsTreeWinUI3.ViewModels;
using TagsTreeWinUI3.Views;

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
		public static AppConfigurations? AppConfigurations { get; set; }

		/// <summary>
		/// Initializes the singleton application object.  This is the first line of authored code
		/// executed, and as such is the logical equivalent of main() or WinMain().
		/// </summary>
		public App()
		{
			InitializeComponent();
			AppConfigurations.Initialize();
		}

		/// <summary>
		/// Invoked when the application is launched normally by the end user.  Other entry points
		/// will be used such as when the application is launched to open a specific file.
		/// </summary>
		/// <param name="args">Details about the launch request and process.</param>
		protected override void OnLaunched(LaunchActivatedEventArgs args)
		{
			Window = new MainWindow { ExtendsContentIntoTitleBar = true };
			Window.Activate();
			AppConfigurations = AppConfigurations.LoadConfiguration();
			LoadConfig();
			if (AppConfigurations is null) return;
			RequestedTheme = AppConfigurations.Theme switch
			{
				"Dark" => ApplicationTheme.Dark,
				"Light" => ApplicationTheme.Light,
				_ => RequestedTheme
			};
			Window.ConfigModeUnlock();
		}

		public static string TagsPath => AppConfigurations!.ProxyPath + @"\TagsTree.json";
		public static string FilesPath => AppConfigurations!.ProxyPath + @"\Files.json";
		public static string RelationsPath => AppConfigurations!.ProxyPath + @"\Relations.xml";

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

		///  <summary>
		///  重新加载新的配置文件
		///  </summary>
		///  <returns>true：已填写正确地址，进入软件；false：打开设置页面；null：关闭软件</returns>
		private static void LoadConfig()
		{
			//总体设置
			if (AppConfigurations is null || !Directory.Exists(AppConfigurations.ProxyPath))
			{
				AppConfigurations = null;
				return;
			}
			//标签
			Tags.LoadTree(TagsPath);
			Tags.LoadDictionary();

			//文件
			IdFile.Deserialize(FilesPath);

			//关系
			if (!File.Exists(RelationsPath))
				_ = File.Create(RelationsPath);
			Relations.Load(); //异常在内部处理

			//检查
			//if (Tags.TagsDictionary.Count != Relations.Columns.Count) //TagsDictionary第一个是总根标签，Relations第一列是文件Id 
			//{
			//	if (await Window.MessageDialogX.Warning($"路径「{AppConfigurations.ProxyPath}」下，TagsTree.xml和Relations.xml存储的标签数不同", "删除关系文件Relations.xml并关闭软件", "直接关闭软件"))
			//		File.Delete(RelationsPath);
			//	return null;
			//}
			//if (IdFile.Count != Relations.Rows.Count)
			//{
			//	if (await Window.MessageDialogX.Warning($"「路径{AppConfigurations.ProxyPath}」下，Files.json和Relations.xml存储的文件数不同", "删除关系文件Relations.xml并关闭软件", "直接关闭软件"))
			//		File.Delete(RelationsPath);
			//	return null;
			//}
			//return true;
		}

	}
}
