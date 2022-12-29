using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static TagsTree.SourceGenerator.Utilities;

namespace TagsTree.SourceGenerator;

internal static partial class TypeWithAttributeDelegates
{
    public static string GenerateConstructor(INamedTypeSymbol typeSymbol, ImmutableArray<AttributeData> attributeList)
    {
        var name = typeSymbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
        var namespaces = new HashSet<string>();
        var usedTypes = new HashSet<ITypeSymbol>(SymbolEqualityComparer.Default);
        var defaultCtor = ConstructorDeclaration(name).WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)));
        var ctor = defaultCtor;
        defaultCtor = defaultCtor.AddBodyStatements();
        foreach (var member in typeSymbol.GetMembers().Where(member =>
                         member is { Kind: SymbolKind.Property } and not { Name: "EqualityContract" })
                     .Cast<IPropertySymbol>())
        {
            ctor = GetDeclaration(member, ctor);
            namespaces.UseNamespace(usedTypes, typeSymbol, member.Type);
        }

        var generatedType = GetDeclaration(name, typeSymbol, defaultCtor, ctor);
        var generatedNamespace = GetFileScopedNamespaceDeclaration(typeSymbol, generatedType);
        var compilationUnit = GetCompilationUnit(generatedNamespace, namespaces);
        return SyntaxTree(compilationUnit, encoding: Encoding.UTF8).GetText().ToString();
    }
}
