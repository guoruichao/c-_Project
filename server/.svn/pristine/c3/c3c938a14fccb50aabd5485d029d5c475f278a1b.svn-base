using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace libCommon
{
    public delegate void DelayCallback(object[] obj);
    public class DelayEvent
    {
        class SDCallbackInfo
        {
            public long nExecTime;
            public DelayCallback pCB;
            public object[] Param;
        }

        static DateTime sg_StartUsTime = DateTime.UtcNow;

        static private int CompareTo(SDCallbackInfo a, SDCallbackInfo b)
        {
            int result;
            try
            {
                if (a.nExecTime > b.nExecTime)
                {
                    result = 1;
                }
                else
                    result = -1;
                return result;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        static private List<SDCallbackInfo> _delayEventList = new List<SDCallbackInfo>();

        public static bool AddEvent(DelayCallback cb, long time = 0, object[] param = null)
        {
            if (time <= 0)
            {
                cb(param);
                return true;
            }
            long lnNow = DelayEvent.GetCurUtcMSTime();

            SDCallbackInfo TimeCB = new SDCallbackInfo();
            TimeCB.nExecTime = lnNow + time;
            TimeCB.pCB = cb;
            TimeCB.Param = param;
            InsertAndSort(ref _delayEventList, TimeCB);
            //             _delayEventList.Add(TimeCB);
            //             _delayEventList.Sort(CompareTo);
            return true;
        }
        private static void InsertAndSort(ref List<SDCallbackInfo> list, SDCallbackInfo newSD)
        {
            SDCallbackInfo[] arrSD = list.ToArray();
            if (arrSD.Length == 0)
            {
                list.Add(newSD); return;
            }
            for (int i = 0; i < arrSD.Length; i++)
            {
                if (newSD.nExecTime < arrSD[i].nExecTime)
                {
                    list.Insert(i, newSD);
                    break;
                }
                if (i == arrSD.Length - 1)//队列里的都比新进入的小（新的应该放到最后一个）
                {
                    list.Add(newSD);
                }
            }
        }

        public static void Update()
        {
            while (_delayEventList.Count > 0)
            {
                SDCallbackInfo sd = _delayEventList[0];
                if (sd == null)
                {
                    return;
                }
                long lnNow = DelayEvent.GetCurUtcMSTime();
                if (lnNow < sd.nExecTime)
                    return;

                _delayEventList.RemoveAt(0);
                if (sd.pCB != null)
                {
                    sd.pCB(sd.Param);
                }

            }

        }
        public static long ClockMS()
        {
            TimeSpan ts2 = DateTime.UtcNow - sg_StartUsTime;
            return (long)ts2.TotalMilliseconds;
        }
        public static long ClockS()
        {
            TimeSpan ts2 = DateTime.UtcNow - sg_StartUsTime;
            return (long)ts2.TotalSeconds;
        }
        public static long ClockUS()
        {
            TimeSpan ts2 = DateTime.UtcNow - sg_StartUsTime;
            return (long)ts2.TotalMilliseconds * 1000;
        }
        public static long GetCurUtcMSTime()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return (long)ts.TotalMilliseconds;
        }
        public static long GetCurUtcSTime()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return (long)ts.TotalSeconds;
        }
        public static long GetCurMSTime()
        {
            TimeSpan ts = DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return (long)ts.TotalMilliseconds;
        }
        public static long GetCurSTime()
        {
            TimeSpan ts = DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return (long)ts.TotalSeconds;
        }
    }
}
