namespace TagsTree.Interfaces;

/// <summary>
/// 可用于xaml的typeof()
/// </summary>
public interface ITypeGetter
{
    public static abstract System.Type TypeGetter { get; }
}