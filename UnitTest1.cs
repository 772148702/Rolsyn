using System;
using System.Linq;
using NUnit.Framework;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Rolsyn
{
    public partial  class Tests
    {
        [SetUp]
        public void Setup()
        {
     
        }

        [Test]
        public void Test1()
        {
            var tree = CSharpSyntaxTree.ParseText(@"
    public class MyClass
    {
        public void MyMethod()
        {
        }
    }");

            var syntaxRoot = tree.GetRoot();
            var MyClass = syntaxRoot.DescendantNodes().OfType<ClassDeclarationSyntax>().First();
            var MyMethod = syntaxRoot.DescendantNodes().OfType<MethodDeclarationSyntax>().First();

            Console.WriteLine(MyClass.Identifier.ToString());
            Console.WriteLine(MyMethod.Identifier.ToString());
            Assert.Pass();
        }
        
        [Test]
        public void Test2()
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
            }
        ");
            var syntexRoot = tree.GetRoot();
            var MyMethod = syntexRoot.DescendantNodes().OfType<MethodDeclarationSyntax>()
                .Where(n => n.ParameterList.Parameters.Any()).First();
            var containType = MyMethod.Ancestors().OfType<TypeDeclarationSyntax>().First();
            Console.WriteLine(containType.Identifier.ToString());
            Console.WriteLine(MyMethod.ToString());

        }
    }
}