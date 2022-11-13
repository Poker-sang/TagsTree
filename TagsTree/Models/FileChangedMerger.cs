using System;
using System.Collections.Generic;

namespace TagsTree.Models;

public class FileChangedMerger
{
    private List<FileChanged> Create { get; } = new();

    private List<FileChanged> Move { get; } = new();

    private List<FileChanged> Rename { get; } = new();

    private List<FileChanged> Delete { get; } = new();

    private bool IsExisted { get; set; } = true;

    public string CurrentName =>
        // Rename在第一个，其他随意
        Rename.Count is not 0 ? Rename[^1].Name :
        Move.Count is not 0 ? Move[^1].Name :
        Create.Count is not 0 ? Create[^1].Name :
        Delete[^1].Name;

    public string OriginalName =>
        // Rename在第一个，其他随意
        Rename.Count is not 0 ? Rename[0].Remark :
        Move.Count is not 0 ? Move[0].Name :
        Create.Count is not 0 ? Create[0].Name :
        Delete[0].Name;

    public string CurrentPath =>
        // Move在第一个，其他随意
        Move.Count is not 0 ? Move[^1].Path :
        Rename.Count is not 0 ? Rename[^1].Path :
        Create.Count is not 0 ? Create[^1].Path :
        Delete[^1].Path;

    public string OriginalPath =>
        // Move在第一个，其他随意
        Move.Count is not 0 ? Move[0].Remark :
        Rename.Count is not 0 ? Rename[0].Path :
        Create.Count is not 0 ? Create[0].Path :
        Delete[0].Path;

    public string CurrentFullName => $"{CurrentPath}\\{CurrentName}";

    public string OriginalFullName => $"{OriginalPath}\\{OriginalName}";

    public FileChangedMerger(FileChanged fileChanged)
    {
        switch (fileChanged.Type)
        {
            case FileChanged.ChangedType.Create:
                Create.Add(fileChanged);
                break;
            case FileChanged.ChangedType.Move:
                Move.Add(fileChanged);
                break;
            case FileChanged.ChangedType.Rename:
                Rename.Add(fileChanged);
                break;
            case FileChanged.ChangedType.Delete:
                Delete.Add(fileChanged);
                IsExisted = false;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(fileChanged));
        }
    }

    public bool CanMerge(FileChanged fileChanged)
    {
        if (fileChanged.OldFullName != CurrentFullName)
            return false;
        // 同或逻辑
        if (fileChanged is { Type: FileChanged.ChangedType.Create } == IsExisted)
            throw new("逻辑出错，可能是遗漏监听文件所致");
        switch (fileChanged.Type)
        {
            case FileChanged.ChangedType.Create:
                Create.Add(fileChanged);
                IsExisted = true;
                return true;
            case FileChanged.ChangedType.Move:
                Move.Add(fileChanged);
                return true;
            case FileChanged.ChangedType.Rename:
                Rename.Add(fileChanged);
                return true;
            case FileChanged.ChangedType.Delete:
                Delete.Add(fileChanged);
                IsExisted = false;
                return true;
            default:
                throw new ArgumentOutOfRangeException(nameof(fileChanged));
        }
    }

    public MergeResult GetMergeResult() => IsExisted
        ? Create.Count == Delete.Count
            ? OriginalFullName == CurrentFullName
                ? MergeResult.Nothing
                : OriginalName == CurrentName && OriginalPath != CurrentPath
                    ? MergeResult.Move
                    : OriginalName != CurrentName && OriginalPath == CurrentPath
                        ? MergeResult.Rename
                        : MergeResult.MoveRename
            : MergeResult.Create
        : Create.Count == Delete.Count
            ? MergeResult.Nothing
            : MergeResult.Delete;

    public enum MergeResult
    {
        /// <summary>
        /// 创建后删除或没有任何改变，没有留下记录
        /// </summary>
        Nothing,
        /// <summary>
        /// 只有路径改变
        /// </summary>
        Move,
        /// <summary>
        /// 只有名称改变
        /// </summary>
        Rename,
        /// <summary>
        /// 路径和名称改变
        /// </summary>
        MoveRename,
        /// <summary>
        /// 创建文件，<see cref="CurrentFullName"/>
        /// </summary>
        Create,
        /// <summary>
        /// 删除文件，<see cref="OriginalFullName"/>
        /// </summary>
        Delete
    }
}
