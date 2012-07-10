using System;
using LogMonitor.Processors;
using LogMonitor.Output;

namespace LogMonitor
{
    public static class Program
    {
        public static void Main()
        {
            IProcessor[] processors = new IProcessor[]
            {
                new ConsoleWriter(),
            };

            using (var k = new Kernel(processors, new [] { @"H:\Csharp\github\Tail.NET\test\" }))
            {
                Console.ReadLine();
            }
        }
    }
}
