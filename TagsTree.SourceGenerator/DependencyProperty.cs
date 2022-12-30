using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static TagsTree.SourceGenerator.Utilities;

namespace TagsTree.SourceGenerator;

internal static partial class TypeWithAttributeDelegates
{
    public static string? DependencyProperty(INamedTypeSymbol typeSymbol, ImmutableArray<AttributeData> attributeList)
    {
        var members = new List<MemberDeclarationSyntax>();
        var namespaces = new HashSet<string> { "Microsoft.UI.Xaml" };
        var usedTypes = new HashSet<ITypeSymbol>(SymbolEqualityComparer.Default);

        foreach (var attribute in attributeList)
        {
            if (attribute.AttributeClass is not ({ IsGenericType: true } and { TypeArguments.IsDefaultOrEmpty: false }))
                return null;
            var type = attribute.AttributeClass.TypeArguments[0];

            if (attribute.ConstructorArguments.Length < 2 || attribute.ConstructorArguments[0].Value is not string propertyName || attribute.ConstructorArguments[1].Value is not string propertyChanged)
                continue;

            var isSetterPublic = true;
            var defaultValue = "DependencyProperty.UnsetValue";
            var isNullable = false;

            foreach (var namedArgument in attribute.NamedArguments)
                if (namedArgument.Value.Value is { } value)
                    switch (namedArgument.Key)
                    {
                        case "IsSetterPublic":
                            isSetterPublic = (bool)value;
                            break;
                        case "DefaultValue":
                            defaultValue = (string)value;
                            break;
                        case "IsNullable":
                            isNullable = (bool)value;
                            break;
                    }

            var fieldName = propertyName + "Property";

            namespaces.UseNamespace(usedTypes, typeSymbol, type);
            var defaultValueExpression = ParseExpression(defaultValue);
            var metadataCreation = GetObjectCreationExpression(defaultValueExpression);
            if (propertyChanged is not "")
                metadataCreation = GetMetadataCreation(metadataCreation, propertyChanged);

            var registration = GetRegistration(propertyName, type, typeSymbol, metadataCreation);
            var staticFieldDeclaration = GetStaticFieldDeclaration(fieldName, registration);
            var getter = GetGetter(fieldName, isNullable, type);
            var setter = GetSetter(fieldName, isSetterPublic);
            var propertyDeclaration = GetPropertyDeclaration(propertyName, isNullable, type, getter, setter);

            members.Add(staticFieldDeclaration);
            members.Add(propertyDeclaration);
        }

        if (members.Count > 0)
        {
            var generatedClass = GetClassDeclaration(typeSymbol, members);
            var generatedNamespace = GetFileScopedNamespaceDeclaration(typeSymbol, generatedClass);
            var compilationUnit = GetCompilationUnit(generatedNamespace, namespaces);
            return SyntaxTree(compilationUnit, encoding: Encoding.UTF8).GetText().ToString();
        }

        return null;
    }
}
