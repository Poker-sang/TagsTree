// #define DISABLE_XAML_GENERATED_BREAK_ON_UNHANDLED_EXCEPTION
// #define DISABLE_XAML_GENERATED_BINDING_DEBUG_OUTPUT
// #define FIRST_TIME
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using TagsTree.Algorithm;
using TagsTree.Models;
using TagsTree.Services;
using TagsTree.Services.ExtensionMethods;
using TagsTree.Views;
using WinUI3Utilities;

namespace TagsTree;

public partial class App : Application
{
    public static AppConfig AppConfig { get; private set; } = null!;
    public static FilesObserver FilesObserver { get; private set; } = null!;
    public static NavigationView RootNavigationView { get; set; } = null!;
    public static Frame RootFrame { get; set; } = null!;
    public static ObservableCollection<FileChanged> FilesChangedList => FilesObserverPage.Vm.FilesChangedList;
    public static bool ConfigSet { get; set; }

    public static void GotoPage<T>(object? parameter = null) where T : Page => GotoPage(typeof(T), parameter);

    public static void GotoPage(Type page, object? parameter = null)
    {
        _ = RootFrame.Navigate(page, parameter);
        RootNavigationView.IsBackEnabled = RootFrame.CanGoBack;
        GC.Collect();
    }

    public App()
    {
        InitializeComponent();
        CurrentContext.App = this;
        CurrentContext.Title = nameof(TagsTree);
        AppContext.Initialize();
        FilesObserver = new();
        if (AppContext.LoadConfiguration() is not { } appConfigurations
#if FIRST_TIME
        || true
#endif
           )
        {
            AppConfig = new AppConfig();
            ConfigSet = false;
        }
        else
        {
            AppConfig = appConfigurations;
            ConfigSet = true;
        }
    }

    /// <param name="args">Details about the launch request and process.</param>
    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        CurrentContext.Window = new MainWindow();
        AppHelper.Initialize(AppHelper.PredetermineEstimatedWindowSize());
    }

    public static async Task<bool> ChangeFilesObserver() => await FilesObserver.Change(AppConfig.LibraryPath);

    public static async Task ExceptionHandler(string exception)
    {
        switch (await ShowMessageDialog.Warning(
                    $"路径「{AppContext.AppLocalFolder}」下，{exception}和{RelationsName}存储数据数不同",
                    $"删除关系文件{RelationsName}并重新生成", "关闭软件并打开目录"))
        {
            case true:
                File.Delete(RelationsPath);
                Relations.Reload();
                break;
            case false:
                AppContext.AppLocalFolder.Open();
                Current.Exit();
                break;
        }
    }

    public static string FilesChangedPath => AppContext.AppLocalFolder + "\\" + FilesChangedName;
    public static string TagsPath => AppContext.AppLocalFolder + "\\" + TagsName;
    private static string FilesPath => AppContext.AppLocalFolder + "\\" + FilesName;
    private static string RelationsPath => AppContext.AppLocalFolder + "\\" + RelationsName;
    public const string FilesChangedName = "FileChanged.json";
    public const string TagsName = "TagsTree.json";
    private const string FilesName = "Files.json";
    private const string RelationsName = "Relations.csv";

    /// <summary>
    /// 保存标签
    /// </summary>
    public static void SaveTags() => Tags.Serialize(TagsPath);

    /// <summary>
    /// 保存文件
    /// </summary>
    public static void SaveFiles() => IdFile.Serialize(FilesPath);

    /// <summary>
    /// 保存关系
    /// </summary>
    public static void SaveRelations() => Relations.Serialize(RelationsPath);

    /// <summary>
    /// 所有标签
    /// </summary>
    public static TagsTreeDictionary Tags { get; set; } = new();

    /// <summary>
    /// 所有文件
    /// </summary>
    public static BidirectionalDictionary<int, FileModel> IdFile { get; } = new();

    /// <summary>
    /// 所有关系
    /// </summary>
    public static RelationsDataTable Relations { get; } = new();

    /// <summary>
    /// 重新加载新的配置文件
    /// </summary>
    public static string? LoadConfig()
    {
        // 文件监视
        FilesObserverPage.Vm = new(FileChanged.Deserialize(FilesChangedPath));

        // 标签
        Tags.DeserializeTree(TagsPath);
        Tags.LoadDictionary();

        // 文件
        IdFile.Deserialize(FilesPath);

        // 关系
        Relations.Deserialize(RelationsPath);

        // 如果本来是空，则按照标签和文件生成关系
        if (Relations.TagsCount is 0 && Relations.FilesCount is 0)
            Relations.Reload();
        else
        {
            // 检查
            // 第一个是空标签减去
            if (Tags.TagsDictionary.Count != Relations.TagsCount + 1)
                return TagsName;
            if (IdFile.Count != Relations.FilesCount)
                return FilesName;
        }

        return null;
    }
}
