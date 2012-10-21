using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LogMonitor.Processors;

namespace LogMonitor
{
    public static class Program
    {
        public static void Main()
        {
            string basePath = Path.GetDirectoryName(typeof(Program).Assembly.Location);

            PowershellProcessor.CheckScript(Path.Combine(basePath, @"Scripts\CallCountProcessor.ps1"));

            IProcessor[] processors = new IProcessor[]
            {
                new PowershellProcessor(Path.Combine(basePath, @"Scripts\CallCountProcessor.ps1"), @"\.log$"),
                new PowershellProcessor(Path.Combine(basePath, @"Scripts\TimeTakenProcessor.ps1"), @"\.log$"),
                new PowershellProcessor(Path.Combine(basePath, @"Scripts\HttpStatusProcessor.ps1"), @"\.log$"),
            };

            string path = @"C:\develop\github\LogMonitor\test";

            using (new Kernel(
                processors,
                new[] { path },
                new Dictionary<string, IPreProcessor> { { path, new W3CProcessor() } }))
            {
                Console.ReadLine();
            }

            foreach (IDisposable disposable in processors.OfType<IDisposable>())
            {
                disposable.Dispose();
            }
        }
    }
}
