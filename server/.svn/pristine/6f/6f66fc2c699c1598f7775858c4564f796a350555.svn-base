using System;
using System.Collections.Generic;
using System.Linq;
using System.Text; 
namespace libCommon
{
    public class MyTimeOut
    {
        private long _nInterval;
        private long _nStartTime;
        public MyTimeOut(long nInterval)
        {
            Start(nInterval);
        }
        public bool Start(long nInterval)
        {
            _nStartTime = DelayEvent.GetCurUtcMSTime();
            _nInterval = nInterval;
            return true;
        }
        public bool IsTimeOut()
        {
            long nCurTime = DelayEvent.GetCurUtcMSTime();
            if (nCurTime - _nStartTime >= _nInterval)
            {
                _nStartTime = nCurTime;
                return true;
            }
            return false;
        }

    }
}
