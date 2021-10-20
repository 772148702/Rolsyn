using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;

namespace Rolsyn
{
    public partial class Tests
    {
        [Test]
        public static void Test7()
        {
            var tree = CSharpSyntaxTree.ParseText(@"
            public class Sample
            {
               public void Foo()
               {
                    int MyMethod() {return 0;}           
                }
            }");

            var Mscorlib = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
            var comiplation = CSharpCompilation.Create("MyCompilation",
                syntaxTrees: new[] {tree}, references: new[] {Mscorlib});
            var  model = comiplation.GetSemanticModel(tree);
        }

   
        
        [Test]
        public static void Test8()
        {
            var tree = CSharpSyntaxTree.ParseText(@"
                 public class MyClass {
                            int Method1() {return 0;}

                            void Method2()
                            {
                                int x = Method1();
                            }
                        }
             ");
            var Mscorlib = PortableExecutableReference.CreateFromFile(typeof(object).Assembly.Location);
            var compilation = CSharpCompilation.Create("MyCompilation",
                syntaxTrees: new[] { tree }, references: new[] { Mscorlib });
            var model = compilation.GetSemanticModel(tree);
            var methodSyntax = tree.GetRoot().DescendantNodes().OfType<MethodDeclarationSyntax>().First();
            var methodSymbol = model.GetDeclaredSymbol(methodSyntax);
            
            Console.WriteLine(methodSymbol.ToString());
            Console.WriteLine(methodSymbol.ContainingSymbol);
            Console.WriteLine(methodSymbol.IsAbstract);

            var invocationSyntax = tree.GetRoot().DescendantNodes().OfType<InvocationExpressionSyntax>().First();
            var invokedSymbol = model.GetSymbolInfo(invocationSyntax).Symbol;
            
            Console.WriteLine(invokedSymbol.ToString());
            Console.WriteLine(invokedSymbol.ContainingSymbol);
            Console.WriteLine(invokedSymbol.IsAbstract);
            
            
            Console.WriteLine(invokedSymbol.Equals(methodSymbol));

        }

        [Test]
        public static void Test9()
        { 
            var tree = CSharpSyntaxTree.ParseText(@"
            public class Sample
            {
               public void Foo()
               {
                    int[] outerArray = new int[10] { 0, 1, 2, 3, 4, 0, 1, 2, 3, 4};
                    for (int index = 0; index < 10; index++)
                    {
                         int[] innerArray = new int[10] { 0, 1, 2, 3, 4, 0, 1, 2, 3, 4 };
                         index = index + 2;
                         outerArray[index – 1] = 5;
                    }
               }
            }");
            var Mscorlib = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
 
            var compilation = CSharpCompilation.Create("MyCompilation",
                syntaxTrees: new[] { tree }, references: new[] { Mscorlib });
            var model = compilation.GetSemanticModel(tree);
 
            var forStatement = tree.GetRoot().DescendantNodes().OfType<ForStatementSyntax>().Single();
            DataFlowAnalysis result = model.AnalyzeDataFlow(forStatement);
            foreach (var item in     result.DataFlowsIn)
            {
                Console.WriteLine(item.Language);
            }
        }

        [Test]
        public static void Test10()
        {
            var tree = CSharpSyntaxTree.ParseText(@"
                class C
                {
                    void M()
                    {
                        for (int i = 0; i < 10; i++)
                        {
                            if (i == 3)
                                continue;
                            if (i == 8)
                                break;
                        }
                    }
                }
            ");
            var Mscorlib = PortableExecutableReference.CreateFromFile(typeof(object).Assembly.Location);
            var compilation = CSharpCompilation.Create("MyCompilation",
                syntaxTrees: new[] { tree }, references: new[] { Mscorlib });
            var model = compilation.GetSemanticModel(tree);
            
            var firstFor = tree.GetRoot().DescendantNodes().OfType<ForStatementSyntax>().Single();
            ControlFlowAnalysis result = model.AnalyzeControlFlow(firstFor.Statement);
            
            Console.WriteLine(result.Succeeded);            //True
            Console.WriteLine(result.ExitPoints.Count());    //2 – continue, and break
        }

        [Test]
        public static void Test11()
        {
            var tree = CSharpSyntaxTree.ParseText(@"
            class C
            {
                void M(int x)
                {
                    L1: ; // 1
                    if (x == 0) goto L1;    //firstIf
                    if (x == 1) goto L2;
                    if (x == 3) goto L3;
                    L3: ;                   //label3
                    L2: ; // 2
                    if(x == 4) goto L3;
                }
            }
            ");

            var Mscorlib = PortableExecutableReference.CreateFromFile(typeof(object).Assembly.Location);
            var compilation = CSharpCompilation.Create("MyCompilation",
                syntaxTrees: new[] { tree }, references: new[] { Mscorlib });
            var model = compilation.GetSemanticModel(tree);

//Choose first and last statements
            var firstIf = tree.GetRoot().DescendantNodes().OfType<IfStatementSyntax>().First();
            var label3 = tree.GetRoot().DescendantNodes().OfType<LabeledStatementSyntax>().Skip(1).Take(1).Single();

            ControlFlowAnalysis result = model.AnalyzeControlFlow(firstIf, label3);
            Console.WriteLine(result.EntryPoints);      //1 – Label 3 is a candidate entry point within these statements
            Console.WriteLine(result.ExitPoints);       //2 – goto L1 and goto L2 and candidate exit points
        }

        [Test]
        public static void Test12()
        {
            var tree = CSharpSyntaxTree.ParseText(@"
                class C
                {
                    void M(int x)
                    {
                        return;
                        if(x == 0)                                  //-+     Start is unreachable
                            System.Console.WriteLine(""Hello"");    // |
                        L1:                                            //-+    End is unreachable
                    }
                }
            ");
            
            var Mscorlib =PortableExecutableReference.CreateFromFile(typeof(object).Assembly.Location);
            var compilation = CSharpCompilation.Create("MyCompilation",
                syntaxTrees: new[] { tree }, references: new[] { Mscorlib });
            var model = compilation.GetSemanticModel(tree);

            //Choose first and last statements
            var firstIf = tree.GetRoot().DescendantNodes().OfType<IfStatementSyntax>().Single();
            var label1 = tree.GetRoot().DescendantNodes().OfType<LabeledStatementSyntax>().Single();

            ControlFlowAnalysis result = model.AnalyzeControlFlow(firstIf, label1);
            Console.WriteLine(result.StartPointIsReachable);    //False
            Console.WriteLine(result.EndPointIsReachable);      //False
        }
    }
}