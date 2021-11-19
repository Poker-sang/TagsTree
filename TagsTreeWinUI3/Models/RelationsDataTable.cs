using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using TagsTreeWinUI3.Services;
using TagsTreeWinUI3.ViewModels;

namespace TagsTreeWinUI3.Models
{
    public class RelationsDataTable
    {
        public Dictionary<int, Dictionary<int, bool>> Table { get; private set; } = new(); //外层是文件，内层是标签，键分别是文件和标签的Id
        public bool this[FileModel fileModel, TagViewModel tag]
        {
            get => Table[fileModel.Id][tag.Id];
            set => Table[fileModel.Id][tag.Id] = value;
        }
        public int FilesCount => Table.Count;
        public int TagsCount => Table is { Count: 0 } ? 0 : Table.First().Value.Count;

        public IEnumerable<TagViewModel> GetTags(FileModel fileModel) => Table[fileModel.Id].Where(pair => pair.Value).Select(pair => App.Tags.TagsDictionary[pair.Key]);

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
                return filesRange.Where(fileModel => Table[fileModel.Id][tagViewModel.Id]);
            }
            else if (App.AppConfigurations.PathTagsEnabled) //唯一需要判断是否能使用路径作为标签的地方
                return filesRange.Where(fileModel => fileModel.PathContains(pathTagModel));
            return Enumerable.Empty<FileModel>();
        }
        public void NewFile(FileModel fileModel)
        {
            Table[fileModel.Id] = new Dictionary<int, bool>();
            foreach (var tagIds in Table.First().Value.Keys)
                Table[fileModel.Id][tagIds] = false;
        }
        public void NewTag(TagViewModel tagViewModel)
        {
            foreach (var fileDict in Table.Values)
                fileDict[tagViewModel.Id] = false;
        }
        public void DeleteFile(FileModel fileModel) => Table.Remove(fileModel.Id);

        public void DeleteTag(TagViewModel tagViewModel)
        {
            foreach (var fileDict in Table.Values)
                _ = fileDict.Remove(tagViewModel.Id);
        }

        public void Reload()
        {
            foreach (var fileIds in App.IdFile.Keys)
            {
                Table[fileIds] = new Dictionary<int, bool>();
                foreach (var tagIds in App.Tags.TagsDictionary.Keys1)
                    Table[fileIds][tagIds] = false;
            }
        }

        public void Deserialize(string path)
        {
            Table.Clear();
            Table = Serialization.Deserialize<Dictionary<int, Dictionary<int, bool>>>(path);
        }

        public void Serialize(string path) => Serialization.Serialize(path, Table);
    }
}