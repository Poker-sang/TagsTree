using System;

namespace TagsTree.Attributes;

/// <summary>
///     生成如下代码
///     <code>
/// public static readonly DependencyProperty Property = DependencyProperty.Register("Field", typeof(Type), typeof(TClass), new PropertyMetadata(DefaultValue, OnPropertyChanged));
/// public Type Field { get => (Type)GetValue(Property); set => SetValue(Property, value); }
/// </code>
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public sealed class DependencyPropertyAttribute : Attribute
{
    public DependencyPropertyAttribute(string name, Type type, string propertyChanged = "")
    {
        Name = name;
        Type = type;
        PropertyChanged = propertyChanged;
    }

    public string Name { get; }

    public Type Type { get; }

    public string PropertyChanged { get; }

    public bool IsSetterpublic { get; init; } = true;

    public bool IsNullable { get; init; } = true;

    public string DefaultValue { get; init; } = "DependencyProperty.UnsetValue";
}