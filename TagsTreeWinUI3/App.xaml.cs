//#define DISABLE_XAML_GENERATED_BREAK_ON_UNHANDLED_EXCEPTION
//#define DISABLE_XAML_GENERATED_BINDING_DEBUG_OUTPUT
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.WinUI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using TagsTreeWinUI3.Models;
using TagsTreeWinUI3.Services;
using TagsTreeWinUI3.Services.ExtensionMethods;
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
        public static MainWindow Window { get; private set; } = null!;
        public static AppConfigurations AppConfigurations { get; private set; } = null!;
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
            IconX.Initialize();
            FilesObserver = new FilesObserver();
            AppConfigurations.Initialize();
            if (AppConfigurations.LoadConfiguration() is { } appConfigurations)
            {
                AppConfigurations = appConfigurations;
                ConfigSet = true;
            }
            else
            {
                AppConfigurations = AppConfigurations.GetDefault();
                ConfigSet = false;
            }

            RequestedTheme = AppConfigurations.Theme switch
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

        public static async Task<bool> ChangeFilesObserver() => await FilesObserver.Change(AppConfigurations.LibraryPath);

        public static async Task ExceptionHandler(string exception)
        {
            switch (await MessageDialogX.Warning($"路径「{AppConfigurations.AppLocalFolder}」下，{exception}和Relations.json存储的文件数不同", "删除关系文件Relations.json并重新生成", "关闭软件并打开目录"))
            {
                case true:
                    File.Delete(RelationsPath);
                    Relations.Reload();
                    break;
                case false:
                    AppConfigurations.AppLocalFolder.Open();
                    Current.Exit();
                    break;
            }
        }


        public static string FilesChangedPath => AppConfigurations.AppLocalFolder + @"\FileChanged.json";
        private static string TagsPath => AppConfigurations.AppLocalFolder + @"\TagsTree.json";
        private static string FilesPath => AppConfigurations.AppLocalFolder + @"\Files.json";
        private static string RelationsPath => AppConfigurations.AppLocalFolder + @"\Relations.json";

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
        public static void SaveRelations() => Relations.Serialize(RelationsPath);


        /// <summary>
        /// 所有标签
        /// </summary>
        public static TreeDictionary Tags { get; private set; } = new();

        /// <summary>
        /// 所有文件
        /// </summary>
        public static BidirectionalDictionary<int, FileModel> IdFile { get; private set; } = new();

        /// <summary>
        /// 所有关系
        /// </summary>
        public static RelationsDataTable Relations { get; private set; } = new();

        /// <summary>
        /// 重新加载新的配置文件
        /// </summary>
        public static string? LoadConfig()
        {
            //文件监视
            FilesObserverPage.Vm = new FilesObserverViewModel(FileChanged.Deserialize(FilesChangedPath));

            //标签
            var tempTags = new TreeDictionary();
            tempTags.DeserializeTree(TagsPath);
            tempTags.LoadDictionary();

            //文件
            var tempIdFile = new BidirectionalDictionary<int, FileModel>();
            tempIdFile.Deserialize(FilesPath);

            //关系
            var tempRelations = new RelationsDataTable();
            tempRelations.Deserialize(RelationsPath); //异常在内部处理

            //如果本来是空，则按照标签和文件生成关系
            if (tempRelations.TagsCount is 0 && tempRelations.FilesCount is 0)
                tempRelations.Reload();
            else
            {
                //检查
                if (tempTags.TagsDictionary.Count != tempRelations.TagsCount + 1) //TagsDictionary第一个是总根标签，不算
                    return "TagsTree.json";
                if (tempIdFile.Count != tempRelations.FilesCount)
                    return "Files.json";
            }

            Tags = tempTags;
            IdFile = tempIdFile;
            Relations = tempRelations;
            return null;
        }

        #region 没用的错误捕捉

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
                await MessageDialogX.Information(true, e.ToString());
                ExitWithPushedNotification();
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
}
