using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using static TagsTree.SourceGenerator.Utilities;

namespace TagsTree.SourceGenerator;

internal static partial class TypeWithAttributeDelegates
{
    public static string? LoadSaveConfiguration(TypeDeclarationSyntax typeDeclaration, INamedTypeSymbol typeSymbol, List<AttributeData> attributeList)
    {
        var attribute = attributeList[0];
        if (attribute.ConstructorArguments[0].Value is not INamedTypeSymbol type)
            return null;
        if (attribute.ConstructorArguments[1].Value is not string containerName)
            return null;

        string? staticClassName = null;
        string? methodName = null;

        if (attribute.NamedArguments[0].Key is "CastMethod" && attribute.NamedArguments[0].Value.Value is string castMethodFullName)
        {
            var dotPosition = castMethodFullName.LastIndexOf('.');
            if (dotPosition is -1)
                throw new InvalidDataException("\"CastMethod\" must contain the full name.");
            staticClassName = "static " + castMethodFullName.Substring(0, dotPosition);
            methodName = castMethodFullName.Substring(dotPosition + 1);
        }

        var name = typeSymbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
        var namespaces = new HashSet<string>();
        if (staticClassName is not null)
            _ = namespaces.Add(staticClassName); //methodName方法所用namespace
        var usedTypes = new HashSet<ITypeSymbol>(SymbolEqualityComparer.Default);
        /*-----Body Begin-----*/
        var stringBuilder = new StringBuilder().AppendLine("#nullable enable\n");
        /*-----Splitter-----*/
        var classBegin = @$"
namespace {typeSymbol.ContainingNamespace.ToDisplayString()};

partial class {name}
{{";
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
        const string saveConfigurationEndAndClassEnd = $@"      }}
    }}
}}";
        /*-----Body End-----*/
        foreach (var member in type.GetMembers().Where(member =>
                         member is { Kind: SymbolKind.Property } and not { Name: "EqualityContract" })
                     .Cast<IPropertySymbol>())
        {
            _ = loadConfigurationContent.AppendLine(LoadRecord(member.Name, member.Type.Name, type.Name, containerName,
                methodName));
            _ = saveConfigurationContent.AppendLine(SaveRecord(member.Name, member.Type, type.Name, containerName,
                methodName));
            namespaces.UseNamespace(usedTypes, typeSymbol, member.Type);
        }

        // 去除" \r\n"
        loadConfigurationContent = loadConfigurationContent.Remove(loadConfigurationContent.Length - 3, 3);

        foreach (var s in namespaces)
            _ = stringBuilder.AppendLine($"using {s};");
        _ = stringBuilder.AppendLine(classBegin)
            .AppendLine(loadConfigurationBegin)
            .AppendLine(loadConfigurationContent.ToString())
            .AppendLine(loadConfigurationEndAndSaveConfigurationBegin)
            // saveConfigurationContent 后已有空行
            .Append(saveConfigurationContent)
            .AppendLine(saveConfigurationEndAndClassEnd);
        return stringBuilder.ToString();
    }

    private static string LoadRecord(string name, string type, string typeName, string containerName, string? methodName) => methodName is null
            ? $"{Spacing(4)}({type}){containerName}.Values[nameof({typeName}.{name})],"
            : $"{Spacing(4)}{containerName}.Values[nameof({typeName}.{name})].{methodName}<{type}>(),";

    private static readonly HashSet<string> PrimitiveTypes = new()
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

    private static string SaveRecord(string name, ITypeSymbol type, string typeName, string containerName, string? methodName)
    {
        var body = $"{containerName}.Values[nameof({typeName}.{name})] = appConfiguration.{name}";
        return !PrimitiveTypes.Contains(type.Name)
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