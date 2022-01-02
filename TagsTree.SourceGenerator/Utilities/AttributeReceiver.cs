using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace TagsTree.SourceGenerator.Utilities;

internal class AttributeReceiver : ISyntaxContextReceiver
{
    private readonly string _attributeName;
    private readonly List<TypeDeclarationSyntax> _candidateTypes = new();

    private INamedTypeSymbol? _attributeSymbol;

    public AttributeReceiver(string attributeName)
    {
        _attributeName = attributeName;
    }

    public IReadOnlyList<TypeDeclarationSyntax> CandidateTypes => _candidateTypes;

    public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
    {
        _attributeSymbol ??= context.SemanticModel.Compilation.GetTypeByMetadataName(_attributeName);

        if (_attributeSymbol is null) return;

        if (context.Node is TypeDeclarationSyntax typeDeclaration && typeDeclaration.AttributeLists
                .SelectMany(l => l.Attributes, (_, attribute) => context.SemanticModel.GetSymbolInfo(attribute))
                .Any(symbolInfo =>
                    SymbolEqualityComparer.Default.Equals(symbolInfo.Symbol?.ContainingType, _attributeSymbol)))
            _candidateTypes.Add(typeDeclaration);
    }
}