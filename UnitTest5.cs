using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;

namespace Rolsyn
{
    public class EmtpyStatementRemovalPro : CSharpSyntaxRewriter
    {
        public override SyntaxNode VisitEmptyStatement(EmptyStatementSyntax node)
        {
            //Construct an EmptyStatementSyntax with a missing semicolon
            return node.WithSemicolonToken(
                SyntaxFactory.MissingToken(SyntaxKind.SemicolonToken)
                    .WithLeadingTrivia(node.SemicolonToken.LeadingTrivia)
                    .WithTrailingTrivia(node.SemicolonToken.TrailingTrivia));
        }
    }
    
    public partial class Tests
    {
        [Test]
        public static void Test6()
        {
            var tree = CSharpSyntaxTree.ParseText(@"
            public class Sample
            {
               public void Foo()
               {
                  Console.WriteLine();
                  #region SomeRegion
                  //Some other code
                  #endregion
                  ;
                }
            }");
            
            
            var rewriter = new EmtpyStatementRemovalPro();
            var result = rewriter.Visit(tree.GetRoot());
            Console.WriteLine(result.ToFullString());
            
        }
    }
}