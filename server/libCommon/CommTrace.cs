using System;
using System.Collections.Generic;
using System.Linq;
using System.Text; 

namespace libCommon
{
    public class CommTrace
    {
        public delegate void MsgOutputFunc(string szMsg);

        public static MsgOutputFunc OnDebug;
        public static MsgOutputFunc OnLog;
        public static MsgOutputFunc OnError;
        public static void Debug(string szMsg)
        {
            if (OnDebug!= null)
                OnDebug(szMsg);
            else 
                Console.WriteLine(szMsg);
        }
        public static void Log(string szMsg)
        {
            if (OnLog != null)
                OnLog(szMsg);
            else
                Console.WriteLine(szMsg);
        }

        public static void Error(string szMsg)
        {
            if (OnError != null)
                OnError(szMsg);
            else
                Console.WriteLine(szMsg);
        }

        public static void Error(Exception err)
        {
            Error(err.Message);
        }

        public static void Error(Exception err, params object[] pars)
        {
            Error(err.Message);
        }

        public static void Debug(string szMsg,params object [] pars)
        {
            string szResMsg = string.Format(szMsg, pars);
            Debug(szResMsg);
        }

        public static void Error(string szMsg, params object[] pars)
        {
            string szResMsg = string.Format(szMsg, pars);
            Error(szResMsg);
        }

        public static void Log(string szMsg, params object[] pars)
        {
            string szResMsg = string.Format(szMsg, pars);
            Log(szResMsg);
        }
    }
}
