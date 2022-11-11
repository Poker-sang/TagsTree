// #define DISABLE_XAML_GENERATED_BREAK_ON_UNHANDLED_EXCEPTION
// #define DISABLE_XAML_GENERATED_BINDING_DEBUG_OUTPUT
// #define FIRST_TIME
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
using TagsTree.Views;

namespace TagsTree;

public partial class App : Application
{
    public static AppConfiguration AppConfiguration { get; private set; } = null!;
    public static FilesObserver FilesObserver { get; private set; } = null!;
    public static NavigationView RootNavigationView { get; set; } = null!;
    public static Frame RootFrame { get; set; } = null!;
    public static ObservableCollection<FileChanged> FilesChangedList => FilesObserverPage.Vm.FilesChangedList;
    public static bool ConfigSet { get; set; }

    public App()
    {
        InitializeComponent();
        RegisterUnhandledExceptionHandler();
        FilesObserver = new();
        AppContext.Initialize();
#if !FIRST_TIME
        if (AppContext.LoadConfiguration() is not { } appConfigurations)
        {
            AppConfiguration = AppContext.GetDefault();
            ConfigSet = false;
        }
        else
#endif
        {
            AppConfiguration = appConfigurations;
            ConfigSet = true;
        }
    }

    /// <param name="args">Details about the launch request and process.</param>
    protected override void OnLaunched(LaunchActivatedEventArgs args) =>
        WindowHelper.Initialize().SetWindowSize(800, 450).Activate();

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
            await WindowHelper.Window.DispatcherQueue.EnqueueAsync(async () => await UncaughtExceptionHandler(args.Exception));
        };

        TaskScheduler.UnobservedTaskException += async (_, args) =>
        {
            args.SetObserved();
            await WindowHelper.Window.DispatcherQueue.EnqueueAsync(async () => await UncaughtExceptionHandler(args.Exception));
        };

        AppDomain.CurrentDomain.UnhandledException += async (_, args) =>
        {
            if (args.ExceptionObject is Exception e)
                await WindowHelper.Window.DispatcherQueue.EnqueueAsync(async () => await UncaughtExceptionHandler(e));
            else
                ExitWithPushedNotification();
        };

        DebugSettings.BindingFailed += (sender, args) => Debug.WriteLine(args.Message);

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
            // ExitWithPushedNotification();
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
