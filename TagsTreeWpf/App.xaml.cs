using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using TagsTreeWpf.Models;
using TagsTreeWpf.Services;
using TagsTreeWpf.ViewModels;
using TagsTreeWpf.Views;
using static TagsTreeWpf.Properties.Settings;

namespace TagsTreeWpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static string TagsPath => Default.ConfigPath + @"\TagsTree.json";
        public static string FilesPath => Default.ConfigPath + @"\Files.json";
        public static string RelationsPath => Default.ConfigPath + @"\Relations.xml";



        /// <summary>
        /// 主窗口
        /// </summary>
        public static Main Win;


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
        public static bool? LoadConfig()
        {
            //总体设置
            if (!Directory.Exists(Default.ConfigPath))
            {
                Default.IsSet = false;
                if (MessageBoxX.Warning($"路径「{Default.ConfigPath}」不存在", "修改设置", "关闭软件"))
                    return false;
                return null;
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
            if (Tags.TagsDictionary.Count != Relations.Columns.Count) //TagsDictionary第一个是总根标签，Relations第一列是文件Id 
            {
                if (MessageBoxX.Warning($"路径「{Default.ConfigPath}」下，TagsTree.xml和Relations.xml存储的标签数不同", "删除关系文件Relations.xml并关闭软件", "直接关闭软件"))
                    File.Delete(RelationsPath);
                return null;
            }
            if (IdFile.Count != Relations.Rows.Count)
            {
                if (MessageBoxX.Warning($"「路径{Default.ConfigPath}」下，Files.json和Relations.xml存储的文件数不同", "删除关系文件Relations.xml并关闭软件", "直接关闭软件"))
                    File.Delete(RelationsPath);
                return null;
            }
            return true;
        }
    }
}