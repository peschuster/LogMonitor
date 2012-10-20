using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.CSharp;

namespace LogMonitor.Processors
{
    public class GenericCsProcessor<T> : IProcessor<T> where T : Line
    {
        public IEnumerable<Metric> ParseLine(string filenName, T lines)
        {
            yield break;
        }

        public bool IsMatch(string fileName)
        {
            // TODO: Implement this method
            throw new NotImplementedException();
        }

        static IEnumerable<IProcessor> LoadProcessors(string code)
        {
            Assembly compiledAssembly = Compile(new[] { code });

            return compiledAssembly.GetModules()
                .First()
                .GetTypes()
                .Where(t => typeof(IProcessor).IsAssignableFrom(t))
                .Select(t => Activator.CreateInstance(t))
                .Cast<IProcessor>();
        }

        static Assembly Compile(string[] code)
        {
            string[] references = { "System.dll", "LogMonitor.dll" };

            var settings = new CompilerParameters
            {
                GenerateInMemory = true,
                TreatWarningsAsErrors = false,
                GenerateExecutable = false,
                CompilerOptions = "/optimize",
            };

            settings.ReferencedAssemblies.AddRange(references);

            var provider = new CSharpCodeProvider();
            
            CompilerResults compile = provider.CompileAssemblyFromSource(settings, code);

            if (compile.Errors.HasErrors)
            {
                var text = new StringBuilder("Compile error: ");
                
                foreach (CompilerError errors in compile.Errors)
                {
                    text.AppendLine(errors.ToString());
                }

                throw new ArgumentException(text.ToString());
            }

            return compile.CompiledAssembly;
        }
    }
}
