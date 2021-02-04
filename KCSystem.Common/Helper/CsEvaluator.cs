using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace KCSystem.Common.Helper
{
   public class CsEvaluator
    {
        /// <summary>CSharp 表达式求值</summary>
        /// <param name="expression">CSharp 表达式。如：2.5, DateTime.Now</param>
        public object Eval(string expression)
        {
            // 代码
            var text = string.Format(@"
                using System;
                public class Calculator
                {{
                    public static object Evaluate() {{ return {0}; }}
                }}", expression);

            // 编译生成程序集
            var tree = SyntaxFactory.ParseSyntaxTree(text);
            var compilation = CSharpCompilation.Create(
                "calc.dll",
                new[] { tree },
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary),
                references: new[] {
                    MetadataReference.CreateFromFile(typeof(object).Assembly.Location)
                });
            Assembly compiledAssembly;
            using (var stream = new MemoryStream())
            {
                var compileResult = compilation.Emit(stream);
                compiledAssembly = Assembly.Load(stream.GetBuffer());
            }

            // 用反射执行方法
            var calculatorClass = compiledAssembly.GetType("Calculator");
            var evaluateMethod = calculatorClass.GetMethod("Evaluate");
            return evaluateMethod.Invoke(null, null);


        }
    }
}
