using System;
using System.ServiceProcess;
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

            if (!Environment.UserInteractive)
            {
                // Startup as service.
                
                ServiceBase.Run(new Service());
            }
            else
            {
                // Startup as application

                using (var k = new Kernel(processors, new [] { @"H:\Csharp\github\Tail.NET\test\" }))
                {
                    Console.ReadLine();
                }
            }
        }
    }
}
