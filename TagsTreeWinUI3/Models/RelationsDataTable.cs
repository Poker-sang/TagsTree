using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using TagsTree.ViewModels;

namespace TagsTree.Models
{
    public class RelationsDataTable
    {
        /// <summary>
        /// 外层是标签，内层是文件，键分别是文件和标签的Id
        /// 数字记录节省文件空间
        /// </summary>
        private Dictionary<int, Dictionary<int, bool>> Table { get; set; } = new();
        public bool this[TagViewModel tag,FileModel fileModel]
        {
            get => Table[tag.Id][fileModel.Id];
            set => Table[tag.Id][fileModel.Id] = value;
        }
        public int TagsCount => Table.Count;
        public int FilesCount => Table is { Count: 0 } ? 0 : Table.Values.First().Count; 
        
        public IEnumerable<TagViewModel> GetTags(FileModel fileModel)
        {
            foreach (var (tagId,tagDict) in Table)
                if (tagDict[fileModel.Id])
                    yield return App.Tags.TagsDictionary[tagId];
        }

        public static ObservableCollection<FileViewModel> FuzzySearchName(string input, IEnumerable<FileViewModel> range)
        {   //大小写不敏感
            var precise = new List<FileViewModel>(); //完整包含搜索内容
            var fuzzy = new List<FileViewModel>(); //有序并全部包含所有字符
            var part = new List<KeyValuePair<int, FileViewModel>>(); //包含任意一个字符，并按包含数排序
            var fuzzyRegex = new Regex(Regex.Replace(input, "(.)", ".+$1", RegexOptions.IgnoreCase));
            var partRegex = new Regex($"[{input}]", RegexOptions.IgnoreCase);
            foreach (var fileViewModel in range)
            {
                if (fileViewModel.Name.Contains(input, StringComparison.OrdinalIgnoreCase))
                    precise.Add(fileViewModel);
                else if (fuzzyRegex.IsMatch(fileViewModel.Name))
                    fuzzy.Add(fileViewModel);
                else
                {
                    var matches = partRegex.Matches(fileViewModel.Name);
                    if (matches.Count is not 0)
                        part.Add(new KeyValuePair<int, FileViewModel>(matches.Count, fileViewModel));
                }
            }
            precise.AddRange(fuzzy);
            part.Sort((x, y) => x.Key.CompareTo(y.Key));
            precise.AddRange(part.Select(item => item.Value));
            var temp = new ObservableCollection<FileViewModel>();
            foreach (var fileModel in precise)
                temp.Add(fileModel);
            return temp;
        }
        public IEnumerable<FileModel> GetFileModels(List<PathTagModel>? tags = null)
        {
            IEnumerable<FileModel> filesRange = App.IdFile.Values;
            if (tags is null or { Count: 0 })
                return filesRange;
            var individualTags = new Dictionary<PathTagModel, bool>();
            foreach (var tag in tags.Where(tag => !individualTags.ContainsKey(tag)))
                individualTags[tag] = true;
            return individualTags.Keys.Aggregate(filesRange, (current, pathTagModel) => GetFileModels(pathTagModel, current));
        }
        private IEnumerable<FileModel> GetFileModels(PathTagModel pathTagModel, IEnumerable<FileModel> filesRange)
        {
            if (pathTagModel is TagViewModel tagViewModel)
            {
                filesRange = tagViewModel.SubTags.Aggregate(filesRange, (current, subTag) => GetFileModels(subTag, current));
                return filesRange.Where(fileModel => this[tagViewModel,fileModel]);
            }
            //唯一需要判断是否能使用路径作为标签的地方
            else if (App.AppConfigurations.PathTagsEnabled)
                return filesRange.Where(fileModel => fileModel.PathContains(pathTagModel));
            return Enumerable.Empty<FileModel>();
        }
        public void NewTag(TagViewModel tagViewModel)
        {
            Table[tagViewModel.Id] = new Dictionary<int, bool>();
            foreach (var fileIds in Table.First().Value.Keys)
                Table[tagViewModel.Id][fileIds] = false;
        }
        public void NewFile(FileModel fileModel)
        {
            foreach (var tagDict in Table.Values)
                tagDict[fileModel.Id] = false;
        }
        public void DeleteTag(TagViewModel tagViewModel)
        {
            Table.Remove(tagViewModel.Id);
        }
        public void DeleteFile(FileModel fileModel)
        {
            foreach (var tagDict in Table.Values)
                _ = tagDict.Remove(fileModel.Id);
        }

        public void Reload()
        {
            foreach (var tagIds in App.Tags.TagsDictionary.Keys1)
            {
                Table[tagIds] = new Dictionary<int, bool>();
                foreach (var fileIds in App.IdFile.Keys)
                    Table[tagIds][fileIds] = false;
            }
        }

        public void Deserialize(string path)
        {
            try
            {
                Table.Clear();
                var buffer = File.ReadAllText(path);
                var rows = buffer.Split(';');
                var fileIds = rows[0].Split(',').Select(int.Parse).ToArray();
                foreach (var row in rows.Skip(1))
                {
                    var columns = row.Split(',');
                    var tagId = int.Parse(columns[0]);
                    Table[tagId] = new Dictionary<int, bool>();
                    for (var i = 0; i < fileIds.Length; ++i)
                        Table[tagId][fileIds[i]] = columns[1][i] is '1';
                }
            }
            catch (Exception)
            {
                Reload();
            }
        }

        public void Serialize(string path)
        {
            var buffer = Table.First().Value.Keys.Aggregate("", (current, fileId) => current + (fileId + ","));
            buffer = buffer.Remove(buffer.Length - 1) + ";";
            foreach (var (tagId, tagDict) in Table)
                buffer = tagDict.Values.Aggregate(buffer + tagId + ",", (current, relation) => current + (relation ? 1 : 0)) + ";";
            buffer = buffer.Remove(buffer.Length - 1);
            File.WriteAllText(path, buffer, Encoding.UTF8);
        }
    }
}