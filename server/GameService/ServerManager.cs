using libCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameService
{
    public class ServerManager : SingleInst<ServerManager>
    {
        protected BaseServer m_pGameServer; 

        public bool Create()
        {
            m_pGameServer = GameServer.CreateNew();
            if (m_pGameServer == null)
            {
                return false;
            }


            if (!m_pGameServer.Create())
            {
                return false;
            }
             
            return true;
        }

        public bool Update()
        {
            if (m_pGameServer != null)
                if (!m_pGameServer.Update())
                    return false;
            DelayEvent.Update();
            return true;
        }

        public void Destroy()
        {

        }
    }
}
