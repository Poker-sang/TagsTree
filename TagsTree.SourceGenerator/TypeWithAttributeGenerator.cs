using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace TagsTree.SourceGenerator;

[Generator]
public class TypeWithAttributeGenerator : IIncrementalGenerator
{
    /// <summary>
    /// 对拥有某attribute的type生成代码
    /// </summary>
    /// <param name="typeSymbol"></param>
    /// <param name="attributeList">该类的某种Attribute</param>
    /// <returns>生成的代码</returns>
    private delegate string? TypeWithAttribute(INamedTypeSymbol typeSymbol, ImmutableArray<AttributeData> attributeList);

    /// <summary>
    /// 需要生成的Attribute
    /// </summary>
    private static readonly Dictionary<string, TypeWithAttribute> _attributes = new()
    {
        { "WinUI3Utilities.Attributes.GenerateConstructorAttribute", TypeWithAttributeDelegates.GenerateConstructor },
        { "WinUI3Utilities.Attributes.AppContextAttribute`1", TypeWithAttributeDelegates.AppContext },
        { "WinUI3Utilities.Attributes.DependencyPropertyAttribute`1", TypeWithAttributeDelegates.DependencyProperty }
    };

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        foreach (var attribute in _attributes)
        {
            var generatorAttributes = context.SyntaxProvider.ForAttributeWithMetadataName(
                attribute.Key,
                (_, _) => true,
                (syntaxContext, _) => syntaxContext
            );
            context.RegisterSourceOutput(generatorAttributes, (spc, ga) =>
            {
                if (ga.TargetSymbol is not INamedTypeSymbol symbol)
                    return;
                if (attribute.Value(symbol, ga.Attributes) is { } source)
                    spc.AddSource(
                        // 不能重名
                        $"{symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat.WithGlobalNamespaceStyle(SymbolDisplayGlobalNamespaceStyle.Omitted))}_{attribute.Key}.g.cs",
                        source);
            });
        }
    }
}
