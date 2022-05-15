using System;
using Microsoft.UI.Xaml;

namespace TagsTree.Attributes;

/// <summary>
///     生成如下代码
///     <code>
/// <see langword="public static readonly"/> <see cref="DependencyProperty"/> Property = <see cref="DependencyProperty"/>.Register("Field", <see langword="typeof"/>(Type), <see langword="typeof"/>(TClass), <see langword="new"/> <see cref="PropertyMetadata"/>(DefaultValue, OnPropertyChanged));
/// <br/>
/// <see langword="public"/> <see cref="Type"/> Field { <see langword="get"/> => (<see cref="Type"/>)GetValue(Property); <see langword="set"/> => SetValue(Property, <see langword="value"/>); }
///     </code>
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public sealed class DependencyPropertyAttribute : Attribute
{
    public DependencyPropertyAttribute(string name, Type type, string propertyChanged = "")
    {
        Name = name;
        DpType = type;
        PropertyChanged = propertyChanged;
    }

    public string Name { get; }

    public Type DpType { get; }

    public string PropertyChanged { get; }

    public bool IsSetterpublic { get; init; } = true;

    public bool IsNullable { get; init; } = true;

    public string DefaultValue { get; init; } = "DependencyProperty.UnsetValue";
}