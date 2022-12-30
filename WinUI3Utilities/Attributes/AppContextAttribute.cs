using System;

namespace WinUI3Utilities.Attributes;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class AppContextAttribute<T> : Attribute
{
    /// <summary>
    /// ConfigurationContainerKey
    /// </summary>
    public string ConfigKey { get; init; } = "";

    public string CastMethod { get; init; } = "";
}
