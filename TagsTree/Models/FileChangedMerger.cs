using System;
using System.Collections.Generic;
using System.Linq;

namespace TagsTree.Models;

public class FileChangedMerger
{
    private List<FileChanged> Create { get; } = new();
    private List<FileChanged> Move { get; } = new();
    private List<FileChanged> Rename { get; } = new();
    private List<FileChanged> Delete { get; } = new();
    private bool IsExisted { get; set; } = true;

    public string CurrentName =>
        Rename.Count is not 0 ? Rename[^1].Name : //Rename在第一个，其他随意
        Move.Count is not 0 ? Move[^1].Name :
        Create.Count is not 0 ? Create[^1].Name :
        Delete[^1].Name;

    public string OriginalName =>
        Rename.Count is not 0 ? Rename[0].Remark : //Rename在第一个，其他随意
        Move.Count is not 0 ? Move[0].Name :
        Create.Count is not 0 ? Create[0].Name :
        Delete[0].Name;

    public string CurrentPath =>
        Move.Count is not 0 ? Move[^1].Path : //Move在第一个，其他随意
        Rename.Count is not 0 ? Rename[^1].Path :
        Create.Count is not 0 ? Create[^1].Path :
        Delete[^1].Path;

    public string OriginalPath =>
        Move.Count is not 0 ? Move[0].Remark : //Move在第一个，其他随意
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
            default: break;
        }
    }

    public bool CanMerge(FileChanged fileChanged)
    {
        if (fileChanged.OldFullName != CurrentFullName)
            return false;
        if (fileChanged is { Type: FileChanged.ChangedType.Create } == IsExisted) //同或门
            throw new Exception("逻辑出错，可能是遗漏监听文件所致");
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
            default: throw new ArgumentOutOfRangeException();
        }
    }

    public MergeResult GetMergeResult()
    {
        if (IsExisted)
            if (Create.Count == Delete.Count)
                if (OriginalFullName == CurrentFullName)
                    return MergeResult.Nothing;
                else if (OriginalName == CurrentName && OriginalPath != CurrentPath)
                    return MergeResult.Move;
                else if (OriginalName != CurrentName && OriginalPath == CurrentPath)
                    return MergeResult.Rename;
                else return MergeResult.MoveRename;
            else return MergeResult.Create;
        else return Create.Count == Delete.Count ? MergeResult.Nothing : MergeResult.Delete;
    }

    public enum MergeResult
    {
        Nothing = 0, //创建后删除或没有任何改变，没有留下记录
        Move = 1, //只有路径改变
        Rename = 2, //只有名称改变
        MoveRename = 3, //路径和名称改变
        Create = 4, //创建文件，CurrentFullName
        Delete = 5 //删除文件，OriginalFullName
    }
}