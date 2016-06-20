using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using libCommon;
namespace ClientTest
{
    class ClientConfig : SingleInst<ClientConfig>
    {
        public string szIPAddr = "";
        public int nPort = 0;
        //public long lnAccountID = 11124;
        public bool Create()
        {
            string value = ConfigurationManager.AppSettings.Get("szUserIP");
            if (String.IsNullOrEmpty(value))
            {
                CliTrace.Log("没有设置服务器Ip");
                return false;
            }
            CliTrace.Log("ip= "+value);
            szIPAddr = value;

            value = ConfigurationManager.AppSettings.Get("nUserPort");
            if (string.IsNullOrEmpty(value))
            {
                CliTrace.Log("没有设置服务器端口");
                return false;
            }
            CliTrace.Log("port= "+value);
            nPort = Convert.ToInt32(value);

//             value = ConfigurationManager.AppSettings.Get("lnAccountID");
//             if (string.IsNullOrEmpty(value))
//             {
//                 CliTrace.Log("没有设置登陆ID");
//                 return false;
//             }
//             CliTrace.Log("lnAccountID= " + value);
//             lnAccountID = Convert.ToInt32(value); 
            //CliTrace.Log("lnAccountID= " + lnAccountID);
            return true;
        }
    }
}
