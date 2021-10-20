using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;

namespace Rolsyn
{
    public class CustomWalker : CSharpSyntaxWalker
    {
        static int Tabs = 0;
        public override void Visit(SyntaxNode node)
        {
            Tabs++;
            var indents = new String('\t', Tabs);
            Console.WriteLine(indents + node.Kind());
            base.Visit(node);
            Tabs--;
        }
    }
    
    public partial  class Tests
    {
        [Test]
        public void Test3()
        {
            var tree = CSharpSyntaxTree.ParseText(@"
        public class MyClass
        {
            public void MyMethod()
            {
            }
            public void MyMethod(int n)
            {
            }
       ");
    
            var walker = new CustomWalker();
            walker.Visit(tree.GetRoot());

        }
    }
}