using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using static TagsTree.SourceGenerator.Utilities;

namespace TagsTree.SourceGenerator;

internal static partial class TypeWithAttributeDelegates
{
    public static string? AppContext(INamedTypeSymbol typeSymbol, ImmutableArray<AttributeData> attributeList)
    {
        var attribute = attributeList[0];

        if (attribute.AttributeClass is not ({ IsGenericType: true } and { TypeArguments.IsDefaultOrEmpty: false }))
            return null;
        var type = attribute.AttributeClass.TypeArguments[0];

        var staticClassName = "static WinUI3Utilities.Misc";
        var methodName = "Cast";

        string? configKey = null;

        foreach (var namedArgument in attribute.NamedArguments)
            if (namedArgument.Value.Value is { } value)
                switch (namedArgument.Key)
                {
                    case "ConfigKey":
                        configKey = (string)value;
                        break;
                    case "CastMethod":
                        var castMethodFullName = (string)value;
                        var dotPosition = castMethodFullName.LastIndexOf('.');
                        if (dotPosition is -1)
                            throw new InvalidDataException("\"CastMethod\" must contain the full name.");
                        staticClassName = "static " + castMethodFullName.Substring(0, dotPosition);
                        methodName = castMethodFullName.Substring(dotPosition + 1);
                        break;
                }

        configKey ??= "Configuration";

        var name = typeSymbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
        var namespaces = new HashSet<string> { "Windows.Storage" };
        // methodName方法所用namespace
        _ = namespaces.Add(staticClassName);
        var usedTypes = new HashSet<ITypeSymbol>(SymbolEqualityComparer.Default);
        /*-----Body Begin-----*/
        var classBegin = @$"
namespace {typeSymbol.ContainingNamespace.ToDisplayString()};

public static partial class {name}
{{
    private static ApplicationDataContainer _configurationContainer = null!;

    private const string ConfigurationContainerKey = ""{configKey}"";

    public static string AppLocalFolder {{ get; private set; }} = null!;

    public static void Initialize()
    {{
        AppLocalFolder = ApplicationData.Current.LocalFolder.Path;
        if (!ApplicationData.Current.RoamingSettings.Containers.ContainsKey(ConfigurationContainerKey))
            _ = ApplicationData.Current.RoamingSettings.CreateContainer(ConfigurationContainerKey, ApplicationDataCreateDisposition.Always);

        _configurationContainer = ApplicationData.Current.RoamingSettings.Containers[ConfigurationContainerKey];
    }}";
        var loadConfigurationBegin = $@"    public static {type.Name}? LoadConfiguration()
    {{
        try
        {{
            return new {type.Name}(";
        /*-----Splitter-----*/
        var loadConfigurationContent = new StringBuilder();
        /*-----Splitter-----*/
        var loadConfigurationEndAndSaveConfigurationBegin = $@"           );
        }}
        catch
        {{
            return null;
        }}
    }}

    public static void SaveConfiguration({type.Name}? configuration)
    {{
        if (configuration is {{ }} appConfiguration)
        {{";
        /*-----Splitter-----*/
        var saveConfigurationContent = new StringBuilder();
        /*-----Splitter-----*/
        const string saveConfigurationEndAndClassEnd = $@"        }}
    }}
}}";
        /*-----Body End-----*/
        foreach (var member in type.GetMembers().Where(member =>
                         member is { Kind: SymbolKind.Property } and not { Name: "EqualityContract" })
                     .Cast<IPropertySymbol>())
        {
            _ = loadConfigurationContent.AppendLine(LoadRecord(member.Name, member.Type.Name, type.Name, methodName));
            _ = saveConfigurationContent.AppendLine(SaveRecord(member.Name, member.Type, type.Name, methodName));
            namespaces.UseNamespace(usedTypes, typeSymbol, member.Type);
        }

        // 去除" \r\n"
        _ = loadConfigurationContent.Remove(loadConfigurationContent.Length - 3, 3);

        return namespaces.GenerateFileHeader()
            .AppendLine(classBegin)
            .AppendLine(loadConfigurationBegin)
            .AppendLine(loadConfigurationContent.ToString())
            .AppendLine(loadConfigurationEndAndSaveConfigurationBegin)
            // saveConfigurationContent 后已有空行
            .Append(saveConfigurationContent)
            .AppendLine(saveConfigurationEndAndClassEnd)
            .ToString();
    }

    private static string LoadRecord(string name, string type, string typeName, string? methodName) => methodName is null
            ? $"{Spacing(4)}({type})_configurationContainer.Values[nameof({typeName}.{name})],"
            : $"{Spacing(4)}_configurationContainer.Values[nameof({typeName}.{name})].{methodName}<{type}>(),";

    private static readonly HashSet<string> _primitiveTypes = new()
    {
        nameof(SByte),
        nameof(Byte),
        nameof(Int16),
        nameof(UInt16),
        nameof(Int32),
        nameof(UInt32),
        nameof(Int64),
        nameof(UInt64),
        nameof(Single),
        nameof(Double),
        nameof(Boolean),
        nameof(Char),
        nameof(DateTime),
        nameof(TimeSpan),
        nameof(Guid),
        nameof(DateTimeOffset)
    };

    private static string SaveRecord(string name, ITypeSymbol type, string typeName, string? methodName)
    {
        var body = $"_configurationContainer.Values[nameof({typeName}.{name})] = appConfiguration.{name}";
        return !_primitiveTypes.Contains(type.Name)
            ? type switch
            {
                { Name: nameof(String) } => $"{Spacing(3)}{body} ?? string.Empty;",
                { TypeKind: TypeKind.Enum } => methodName is null
                    ? $"{Spacing(3)}(int)({body});"
                    : $"{Spacing(3)}{body}.{methodName}<int>();",
                _ => throw new InvalidCastException("Only primitive and Enum types are supported.")
            }
            : $"{Spacing(3)}{body};";
    }
}
