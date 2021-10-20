using System.Linq;
using System.Reflection.Metadata;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;

namespace Rolsyn
{
    public partial class Tests
    {
        [Test]
        public static void Test13()
        {
       
            AdhocWorkspace workspace = new AdhocWorkspace();
            Project project = workspace.AddProject("SampleProject", LanguageNames.CSharp);
    

             //Attach a syntax annotation to the class declaration
            var syntaxAnnotation = new SyntaxAnnotation();
            var classDeclaration = SyntaxFactory.ClassDeclaration("MyClass")
                .WithAdditionalAnnotations(syntaxAnnotation);

            var compilationUnit = SyntaxFactory.CompilationUnit().AddMembers(classDeclaration);

            Microsoft.CodeAnalysis.Document document = project.AddDocument("SampleDocument.cs", compilationUnit);
            SemanticModel semanticModel = document.GetSemanticModelAsync().Result;

             //Use the annotation on our original node to find the new class declaration
            var changedClass = document.GetSyntaxRootAsync().Result.DescendantNodes().OfType<ClassDeclarationSyntax>()
                .Where(n => n.HasAnnotation(syntaxAnnotation)).Single();
            var symbol = ModelExtensions.GetDeclaredSymbol(semanticModel, changedClass);
        }

        // [Test]
        // public static void Test14()
        // {
        //     AdhocWorkspace workspace = new AdhocWorkspace();
        //     Project project = workspace.AddProject("Test", LanguageNames.CSharp);
        //
        //     string annotationKind = "SampleKind";
        //     var syntaxAnnotation = new SyntaxAnnotation(annotationKind);
        //     var classDeclaration = SyntaxFactory.ClassDeclaration("MyClass")
        //         .WithAdditionalAnnotations(syntaxAnnotation);
        //
        //     var compilationUnit = SyntaxFactory.CompilationUnit().AddMembers(classDeclaration);
        //
        //     Microsoft.CodeAnalysis.Document document = project.AddDocument("Test.cs", compilationUnit);
        //     SemanticModel semanticModel = await document.GetSemanticModelAsync();
        //     var newAnnotation = new SyntaxAnnotation("test");
        //
        //     //Just search for the Kind instead
        //     var root = await document.GetSyntaxRootAsync();
        //     var changedClass = root.GetAnnotatedNodes(annotationKind).Single();
        //
        //     var symbol = semanticModel.GetDeclaredSymbol(changedClass);
        // }
        
    }
}