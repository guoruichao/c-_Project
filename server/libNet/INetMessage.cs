using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace libNet
{
    public interface INetMessage
    {
        int MsgType { get; set; }
    }
}
