using System;
using System.Collections.Generic;
using LogMonitor.Processors;

namespace LogMonitor
{
    public static class Program
    {
        public static void Main()
        {
            IProcessor[] processors = new IProcessor[]
            {
                new PowerShellProcessor(@"C:\develop\github\LogMonitor\source\LogMonitor.Core\Processors\CallCountProcessor.ps1", @"\.log$"),
                new PowerShellProcessor(@"C:\develop\github\LogMonitor\source\LogMonitor.Core\Processors\TimeTakenProcessor.ps1", @"\.log$"),
                new PowerShellProcessor(@"C:\develop\github\LogMonitor\source\LogMonitor.Core\Processors\HttpStatusProcessor.ps1", @"\.log$")
            };

            string path = @"C:\develop\github\LogMonitor\test";

            using (var k = new Kernel(
                processors,
                new[] { path },
                new Dictionary<string, IPreProcessor> { { path, new W3CProcessor() } }))
            {
                Console.ReadLine();
            }
        }
    }
}
