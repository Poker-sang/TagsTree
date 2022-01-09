//#define DISABLE_XAML_GENERATED_BREAK_ON_UNHANDLED_EXCEPTION
//#define DISABLE_XAML_GENERATED_BINDING_DEBUG_OUTPUT
//#define FIRST_TIME
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.WinUI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using TagsTree.Algorithm;
using TagsTree.Models;
using TagsTree.Services;
using TagsTree.Services.ExtensionMethods;
using TagsTree.ViewModels;
using TagsTree.Views;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace TagsTree;

/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
public partial class App : Application
{
    public static MainWindow Window { get; private set; } = null!;
    public static AppConfiguration AppConfiguration { get; private set; } = null!;
    public static FilesObserver FilesObserver { get; private set; } = null!;
    public static NavigationView RootNavigationView { get; set; } = null!;
    public static Frame RootFrame { get; set; } = null!;
    public static ObservableCollection<FileChanged> FilesChangedList => FilesObserverPage.Vm.FilesChangedList;
    public static bool ConfigSet { get; set; }


    /// <summary>
    /// Initializes the singleton application object.  This is the first line of authored code
    /// executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App()
    {
        InitializeComponent();
        RegisterUnhandledExceptionHandler();
        IconsHelper.Initialize();
        FilesObserver = new FilesObserver();
        AppContext.Initialize();
        if (AppContext.LoadConfiguration() is { } appConfigurations
#if FIRST_TIME
                && false
#endif 
           )
        {
            AppConfiguration = appConfigurations;
            ConfigSet = true;
        }
        else
        {
            AppConfiguration = AppContext.GetDefault();
            ConfigSet = false;
        }

        RequestedTheme = AppConfiguration.Theme switch
        {
            1 => ApplicationTheme.Light,
            2 => ApplicationTheme.Dark,
            _ => RequestedTheme
        };
    }

    /// <summary>
    /// Invoked when the application is launched normally by the end user.  Other entry points
    /// will be used such as when the application is launched to open a specific file.
    /// </summary>
    /// <param name="args">Details about the launch request and process.</param>
    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        Window = new MainWindow { ExtendsContentIntoTitleBar = true };
        WindowHelper.SetWindowSize(800, 450);
        Window.Activate();
    }

    public static async Task<bool> ChangeFilesObserver() => await FilesObserver.Change(AppConfiguration.LibraryPath);

    public static async Task ExceptionHandler(string exception)
    {
        switch (await ShowMessageDialog.Warning($"路径「{AppContext.AppLocalFolder}」下，{exception}和Relations.csv存储数据数不同", "删除关系文件Relations.csv并重新生成", "关闭软件并打开目录"))
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


    public static string FilesChangedPath => AppContext.AppLocalFolder + @"\FileChanged.json";
    public static string TagsPath => AppContext.AppLocalFolder + @"\TagsTree.json";
    private static string FilesPath => AppContext.AppLocalFolder + @"\Files.json";
    private static string RelationsPath => AppContext.AppLocalFolder + @"\Relations.csv";

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
        //文件监视
        FilesObserverPage.Vm = new FilesObserverViewModel(FileChanged.Deserialize(FilesChangedPath));

        //标签
        Tags.DeserializeTree(TagsPath);
        Tags.LoadDictionary();

        //文件
        IdFile.Deserialize(FilesPath);

        //关系
        Relations.Deserialize(RelationsPath);

        //如果本来是空，则按照标签和文件生成关系
        if (Relations.TagsCount is 0 && Relations.FilesCount is 0)
            Relations.Reload();
        else
        {
            //检查
            if (Tags.TagsDictionary.Count != Relations.TagsCount + 1)//第一个是空标签减去
                return "TagsTree.json";
            if (IdFile.Count != Relations.FilesCount)
                return "Files.json";
        }

        return null;
    }

    #region 错误捕捉

    private void RegisterUnhandledExceptionHandler()
    {
        UnhandledException += async (_, args) =>
        {
            args.Handled = true;
            await Window.DispatcherQueue.EnqueueAsync(async () => await UncaughtExceptionHandler(args.Exception));
        };

        TaskScheduler.UnobservedTaskException += async (_, args) =>
        {
            args.SetObserved();
            await Window.DispatcherQueue.EnqueueAsync(async () => await UncaughtExceptionHandler(args.Exception));
        };

        AppDomain.CurrentDomain.UnhandledException += async (_, args) =>
        {
            if (args.ExceptionObject is Exception e)
                await Window.DispatcherQueue.EnqueueAsync(async () => await UncaughtExceptionHandler(e));
            else ExitWithPushedNotification();
        };

        DebugSettings.BindingFailed += (sender, args) =>
        {
            Debug.WriteLine(args.Message);
        };

#if DEBUG
        static Task UncaughtExceptionHandler(Exception e)
        {
            if (Debugger.IsAttached)
                Debugger.Break();
            return Task.CompletedTask;
        }
#elif RELEASE
        static async Task UncaughtExceptionHandler(Exception e)
        {
            await ShowMessageDialog.Information(true, e.ToString());
            //ExitWithPushedNotification();
        }
#endif
    }
    private static void ExitWithPushedNotification()
    {
        _ = WeakReferenceMessenger.Default.Send(new ApplicationExitingMessage());
        Current.Exit();
    }
    public record ApplicationExitingMessage;

    #endregion
}