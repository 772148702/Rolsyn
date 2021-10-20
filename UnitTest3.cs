using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.VisualBasic;
using NUnit.Framework;

namespace Rolsyn
{

    public class DeepWalker : CSharpSyntaxWalker
    {
        private static int Tabs = 0;

        public DeepWalker() : base(SyntaxWalkerDepth.Token)
        {
            
        }

        public override void Visit(SyntaxNode node)
        {
            Tabs++;
            var indents = new String('\t',Tabs);
            Console.WriteLine(indents+node.Kind());
            base.Visit(node);
            Tabs--;
        }

        public override void VisitToken(SyntaxToken token)
        {
          var indents = new String('\t',Tabs);
          Console.WriteLine(indents+token);
          base.VisitToken(token);
        }
    }
    
    public partial  class Tests
    {
        [Test]
        public static void Test4()
        {
       //      var tree = CSharpSyntaxTree.ParseText(@"
       //  public class MyClass
       //  {
       //      public void MyMethod()
       //      {
       //      }
       //      public void MyMethod(int n)
       //      {
       //      }
       // ");
       var tree = CSharpSyntaxTree.ParseText(@"
    public class MyClass
    {
        public void MyMethod()
        {
        }
    }
    public class MyOtherClass
    {
        public void MyMethod(int n)
        {
        }
    }
   ");
            var walker = new DeepWalker();
            walker.Visit(tree.GetRoot());
        }
        
    }
}