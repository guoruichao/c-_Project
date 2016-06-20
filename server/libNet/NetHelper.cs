using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace libNet
{
    public class NetHelper
    {

        public static long TransIpAddressToNumber(string ip)
        {
            if (string .IsNullOrEmpty(ip))
                return 0;
            //取出IP地址去掉‘.’后的string数组
            string[] Ip_List = ip.Split(".".ToCharArray());
            string X_Ip = "";
            foreach (string p_ip in Ip_List)
            {
                X_Ip += Convert.ToInt16(p_ip).ToString("x");
            }

            long N_Ip = long.Parse(X_Ip, System.Globalization.NumberStyles.HexNumber);
            return N_Ip;
        }
    }
}
