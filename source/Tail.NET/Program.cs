using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tail
{
    public static class Program
    {
        public static void Main()
        {
            using (var k = new Kernel(@"H:\Csharp\github\Tail.NET\test\"))
            {
                Console.ReadLine();

                GC.KeepAlive(k);
            }
        }
    }
}
