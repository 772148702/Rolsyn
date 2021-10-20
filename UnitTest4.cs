using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;


namespace Rolsyn
{
    public class EmtpyStatementRemoval : CSharpSyntaxRewriter
    {
        public override SyntaxNode VisitEmptyStatement(EmptyStatementSyntax node)
        {
            //Simply remove all Empty Statements
            return null;
        }
    }

    public partial class Tests
    {
        [Test]
        public static void Test5()
        {
            var tree = CSharpSyntaxTree.ParseText(@"
            public class Sample
            {
               public void Foo()
               {
                  Console.WriteLine();
                  ;
                }
            }");
            var rewriter = new EmtpyStatementRemoval();
            var result = rewriter.Visit(tree.GetRoot());
            Console.WriteLine(result.ToFullString());
            
        }
    }

}