﻿using System;

namespace TagsTree.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public sealed class DependencyPropertyAttribute : Attribute
{
    public DependencyPropertyAttribute(string name, Type type)
    {
        Name = name;
        Type = type;
    }

    public string Name { get; }
    public Type Type { get; }
    public bool IsSetterPublic { get; init; } = true;
    public bool IsNullable { get; init; } = true;
    public string DefaultValue { get; init; } = "null";
    public bool InstanceChangedCallback { get; init; }
}