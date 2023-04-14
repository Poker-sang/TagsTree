#define FIRST_TIME
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using TagsTree.Algorithm;
using TagsTree.Models;
using TagsTree.Services;
using TagsTree.Services.ExtensionMethods;
using TagsTree.Views;
using TagsTree.Views.ViewModels;
using Windows.Storage;
using WinUI3Utilities.Attributes;

namespace TagsTree;

[AppContext<AppConfig>]
public static partial class AppContext
{
    public static AppConfig AppConfig { get; private set; } = null!;

    public static string AppLocalFolder { get; private set; } = null!;

    public static FilesObserver FilesObserver { get; private set; } = null!;

    public static ObservableCollection<FileChanged> FilesChangedList => FilesObserverPage.Vm.FilesChangedList;

    public static SettingsViewModel SettingViewModel { get; } = new();

    public static void Initialize()
    {
        AppLocalFolder = ApplicationData.Current.LocalFolder.Path;
        InitializeConfigurationContainer();
        AppConfig =
#if FIRST_TIME
                   LoadConfiguration() ??
#endif
            new AppConfig();
        FilesObserver = new();
    }

    public static void SetDefaultAppConfig() => AppConfig = new();

    public static async Task FilesObserverChanged() => await FilesObserver.FilesObserverChanged(AppConfig.LibraryPath);

    public static async Task ExceptionHandler(string exception)
    {
        //switch (await ShowContentDialog.Warning(
        //            new TextBlock
        //            {
        //                TextWrapping = TextWrapping.Wrap,
        //                Inlines =
        //                {
        //                    AppLocalFolder.GetHyperlink("软件设置"),
        //                    new Run { Text = $"里，{exception}和{RelationsName}存储数据数不同" }
        //                }
        //            }))
        if (await ShowContentDialog.Warning(
                $"路径「{AppLocalFolder}」下，{exception}和{RelationsName}存储数据数不同",
                $"删除关系文件{RelationsName}并重新生成", "关闭软件并打开目录"))
        {
            File.Delete(RelationsPath);
            Relations.Reload();
        }
        else
        {
            AppLocalFolder.Open();
            Application.Current.Exit();
        }
    }

    public static string FilesChangedPath => AppLocalFolder + "\\" + FilesChangedName;

    public static string TagsPath => AppLocalFolder + "\\" + TagsName;

    private static string FilesPath => AppLocalFolder + "\\" + FilesName;

    private static string RelationsPath => AppLocalFolder + "\\" + RelationsName;

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
