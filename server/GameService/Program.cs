using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GameService
{
    class Program
    {
        static void Main(string[] args)
        {
            if (!ServerManager.GetInstance().Create())
            {
                return;
            }
            while (true)
            {
                if (!ServerManager.GetInstance().Update())
                {
                    break;
                }
                Thread.Sleep(1);
            }
        }
    }
}
