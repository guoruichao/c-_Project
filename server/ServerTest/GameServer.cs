using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using libNet.Network.Interface;
using System.Threading;

namespace ServerTest
{
    class GameNet
    {
        public GameNet()
        {

        }

        private INet m_pNet;
    }
    public class GameServer : INetEvent
    {
        private INet m_pListenNet = null;
        public GameServer()
        {
            
        }

        public bool Create()
        {
            m_pListenNet = libNet.Network.MyNetwork.Listen("0.0.0.0:1111",this);
            if (m_pListenNet == null)
            {
                return false;
            }
            ThreadPool.QueueUserWorkItem(ClientProcesFunc, this);
            return true;
        }
        private void ClientProcesFunc(object state)
        {
            this.Update();

            ThreadPool.QueueUserWorkItem(ClientProcesFunc, this);
        }

        public void Update()
        {
            if (m_pListenNet != null)
            {
                m_pListenNet.Process();
            }
        }
#region INetEvent Interface 
        public bool OnAccept(INet net)
        {
            if (net == null)
                return false;


            return true;
        }
        override public void OnRecvice(INet net, byte[] data, short len)
        {

        }
        override public void OnClose()
        {

        }
#endregion
    }
}
