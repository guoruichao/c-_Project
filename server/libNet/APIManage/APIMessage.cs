using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// API消息.
/// </summary>

namespace libNet.APIManage
{
    public class APIMessage
    {
        public int Type { get; set; }
        public object[] Parameters { get; set; }
    }  
}