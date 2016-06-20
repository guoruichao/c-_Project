using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks; 

namespace ClientTest
{  
    class Program
    {  
        static List<ClientUser> cListPool=new List<ClientUser>();
        static void Main(string[] args)
        { 
            ClientConfig.GetInstance().Create();
            for (int i = 0; i <3; i++)//创建10个角色
            {
                ClientUser user = new ClientUser(10000+i);
                if (!user.Start())
                {
                    continue;
                }
                //ThreadPool.QueueUserWorkItem(UserItem, user);
                cListPool.Add(user);
            }
            Thread thread = new Thread(DoRun);
             thread.Start();
             //Console.Read();
            

        }
        static void UserItem(object param)
        {
            ClientUser user = param as ClientUser;
            while (true)
            {     
                user.Run();
                Thread.Sleep(1);
            }
        }
        static void DoRun()
        {
            while (true)
            {
                for (int i = 0; i < cListPool.Count; i++)
                {
                    ClientUser user = cListPool[i];
                    user.Run();
                }
                Thread.Sleep(1);
            }
        }
    }
}
