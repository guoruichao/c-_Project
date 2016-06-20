using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientTest
{
    class CliTrace
    {
        public static void Debug(string szMsg)
        {
            Console.WriteLine(szMsg);
        }
        public static void Log(string szMsg)
        {
            Console.WriteLine(szMsg);
        }

        public static void Error(string szMsg)
        {
            Console.WriteLine(szMsg);
        }
    }
}
