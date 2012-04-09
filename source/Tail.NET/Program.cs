using System;
using Tail.Processors;

namespace Tail
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
