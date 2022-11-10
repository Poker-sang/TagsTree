using System;

namespace TagsTree.Attributes;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class LoadSaveConfigurationAttribute<T> : Attribute
{
    public LoadSaveConfigurationAttribute(string containerName) => ContainerName = containerName;
    public string ContainerName { get; }
    public string CastMethod { get; set; } = "null!";
}
