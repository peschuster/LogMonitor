using System;
using System.ServiceProcess;

namespace LogMonitor
{
    public static class Program
    {
        public static void Main()
        {
            try
            {
                if (Environment.UserInteractive)
                {
                    using (var starter = new ConfigurationAppStarter())
                    {
                        using (starter.Start())
                        {
                            Console.ReadLine();
                        }
                    }
                }
                else
                {
                    ServiceBase.Run(new WindowsService());
                }
            }
            catch (SystemException exception)
            {
                var color = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;

                Console.WriteLine(exception.Message);
                Console.ForegroundColor = color;

                Console.WriteLine();
            }
        }
    }
}
