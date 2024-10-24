using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using TagsTree.Algorithm;
using TagsTree.Views.ViewModels;

namespace TagsTree.Models;

/// <summary>
/// Column标签，Row是文件，键分别是文件和标签的Id
/// </summary>
/// <remarks>
/// <see cref="FileModel.Id"/>记录节省文件空间
/// </remarks>
public partial class RelationsDataTable : TableDictionary<int, int>
{
    public bool this[TagViewModel tag, FileModel fileModel]
    {
        get => base[tag.Id, fileModel.Id];
        set => base[tag.Id, fileModel.Id] = value;
    }

    private Dictionary<int, int> Tags => Columns;

    private Dictionary<int, int> Files => Rows;

    public int TagsCount => Tags.Count;

    public int FilesCount => Files.Count;

    public IEnumerable<TagViewModel> GetTags(int fileId) => Tags.Where(pair => base[pair.Key, fileId]).Select(pair => AppContext.Tags.TagsDictionary[pair.Key]);

    [GeneratedRegex("(.)", RegexOptions.IgnoreCase)]
    private static partial Regex FuzzyRegex();

    public static ObservableCollection<FileViewModel> FuzzySearchName(string input, IEnumerable<FileViewModel> range)
    {   // 大小写不敏感
        // 完整包含搜索内容
        var precise = new List<FileViewModel>();
        // 有序并全部包含所有字符
        var fuzzy = new List<FileViewModel>();
        // 包含任意一个字符，并按包含数排序
        var part = new List<(int Count, FileViewModel FileViewModel)>();
        var fuzzyRegex = new Regex(FuzzyRegex().Replace(input, ".+$1"));
        var partRegex = new Regex($"[{input}]", RegexOptions.IgnoreCase);
        foreach (var fileViewModel in range)
            if (fileViewModel.Name.Contains(input, StringComparison.OrdinalIgnoreCase))
                precise.Add(fileViewModel);
            else if (fuzzyRegex.IsMatch(fileViewModel.Name))
                fuzzy.Add(fileViewModel);
            else if (partRegex.Matches(fileViewModel.Name) is { Count: not 0 } matches)
                part.Add((matches.Count, fileViewModel));

        precise.AddRange(fuzzy);
        part.Sort((x, y) => x.Count.CompareTo(y.Count));
        precise.AddRange(part.Select(item => item.FileViewModel));
        return [..precise];
    }

    public static IEnumerable<FileModel> GetFileModels(ICollection<PathTagModel>? tags = null)
    {
        IEnumerable<FileModel> filesRange = AppContext.IdFile.Values;
        if (tags is null or { Count: 0 })
            return filesRange;
        var individualTags = tags.ToHashSet();
        return individualTags.Aggregate(filesRange, (current, pathTagModel) => GetFileModels(pathTagModel, current));
    }

    private static IEnumerable<FileModel> GetFileModels(PathTagModel pathTagModel, IEnumerable<FileModel> filesRange)
    {
        if (pathTagModel is TagViewModel tagViewModel)
        {
            return filesRange.Where(fileModel => fileModel.HasTag(tagViewModel) is not false);
        }

        // 唯一需要判断是否能使用路径作为标签的地方
        return AppContext.AppConfig.PathTagsEnabled ? filesRange.Where(fileModel => fileModel.PathContains(pathTagModel)) : [];
    }

    public void NewTag(TagViewModel tagViewModel) => AddColumn(tagViewModel.Id);

    public void NewFile(FileModel fileModel) => AddRow(fileModel.Id);

    public void DeleteTag(TagViewModel tagViewModel) => RemoveColumn(tagViewModel.Id);

    public void DeleteFile(FileModel fileModel) => RemoveRow(fileModel.Id);

    public void Reload()
    {
        Table.Clear();
        Columns.Clear();
        Rows.Clear();
        foreach (var fileIds in AppContext.IdFile.Keys)
            Files[fileIds] = FilesCount;
        foreach (var tagIds in AppContext.Tags.TagsDictionary.Keys1.Skip(1))
        {
            Tags[tagIds] = TagsCount;
            Table.Add([]);
            for (var i = 0; i < AppContext.IdFile.Keys.Count; ++i)
                this[tagIds].Add(false);
        }
    }

    public new void Deserialize(string path)
    {
        try
        {
            base.Deserialize(path);
        }
        catch (Exception)
        {
            Reload();
        }
    }
}
