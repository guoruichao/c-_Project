using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using libCommon;
//using Smarten.Utility;
namespace libNet.APIManage
{
    /// <summary>
    /// API管理类.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class APIManage<T>
    {
        private Dictionary<int, APIMethod> _dictmethod;

        public static APIManage<T> Instance;


        public APIManage()
        {
            if (_dictmethod != null) return;
            _dictmethod = new Dictionary<int, APIMethod>();
            Type type = typeof(T);
            MethodInfo[] methods = type.GetMethods();
            foreach (var m in methods)
            {
                APIMethod api = m.GetCustomAttributes(typeof(APIMethod), false).Cast<APIMethod>().FirstOrDefault();
                if (api == null) continue;
                if (_dictmethod.ContainsKey(api.Name))
                    throw new Exception("API Name[" + api.Name + "] Is Not Unique!\r\nConflict:" + m.Name + "\t\t" + _dictmethod[api.Name].Name);
                api.FastMethod = FastInvoke.GetMethodInvoker(m);
                api.Method = m;
                _dictmethod.Add(api.Name, api);
            }
        }

        static APIManage()
        {
            Instance = new APIManage<T>();

        }

        /// <summary>
        /// 根据消息类型以及消息参数调用服务器逻辑方法,此方法控制消息是否反馈.
        /// </summary>
        /// <typeparam name="TSession"></typeparam>
        /// <param name="message"></param>
        /// <param name="session"></param>
        /// <returns></returns>
        public Result Handle<TSession>(APIMessage message, TSession session) where TSession : SessionBase/*, new()*/
        {
            if (message == null) return null;

            if (!_dictmethod.ContainsKey(message.Type))
            {
                CommTrace.Debug("[" + DateTime.Now + "][" + session.SessionId + "]没有发现消息类型!   " + message.Type, session.SessionId);
                return null;
            }

            APIMethod apimethod = _dictmethod[message.Type];
            //CommTrace.Debug("协议："+message.Type.ToString());
            if (!session.CanVisit(apimethod.Name, apimethod.VisitTimeSpan))
            {
                CommTrace.Debug("[" + DateTime.Now + "][" + session.SessionId + "]Warning:API[" + apimethod.Method.Name + "] visit limit!", session.SessionId);
                return null;
            }

            object[] parameters = null;
            if (!apimethod.AgentMessage)
            {
                if (message.Parameters == null)
                {
                    parameters = new object[1];
                    parameters[0] = session;
                }
                else
                {
                    parameters = new object[message.Parameters.Length + 1];
                    Array.Copy(message.Parameters, parameters, message.Parameters.Length);
                    parameters[message.Parameters.Length] = session;
                }
            }
            else
                parameters = message.Parameters;

            string pa = string.Empty;
            if (ServerInfomation.OpenApiInfo)
            {
                foreach (var item in parameters)
                {
                    if (pa.Length > 0)
                        pa += ",";
                    pa += item.ToString();
                }
            }

            Result result = null;
            int exectime = Environment.TickCount;
            try
            {
                result = apimethod.FastMethod(null, parameters) as Result;
                exectime = Environment.TickCount - exectime;
            }
            catch (Exception ex)
            {
                CommTrace.Error("[" + DateTime.Now + "][" + session.SessionId + "][" + exectime + "ms]Error:API[" + apimethod.Method.Name + "]! Parameters:" + pa, ex, session.SessionId);
                return null;
            }
            if (result == null)
            {
                return result;
            }
            result.FeedBack = apimethod.Feedback;
            if (!result.Success)
            {
                CommTrace.Debug("[" + DateTime.Now + "][" + session.SessionId + "][" + exectime + "ms]Warning:API[" + apimethod.Method.Name + "]! Parameters:" + pa, session.SessionId);
            }
            if (ServerInfomation.OpenApiInfo)
            {
                if (result.Success)
                    CommTrace.Debug("[" + DateTime.Now + "][" + session.SessionId + "][" + exectime + "ms]Trace:API[" + apimethod.Method.Name + "]! Parameters:" + pa, session.SessionId);
              //  CommTrace.Debug(/*ReflectionHelper.Dump(result.Package())*/, session.SessionId);
            }
            return result;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            foreach (var key in _dictmethod.Keys)
            {
                string parameters = string.Empty;
                foreach (var item in _dictmethod[key].Method.GetParameters())
                {
                    if (parameters.Length > 0)
                        parameters += ",";
                    parameters += item.ParameterType.Name + " " + item.Name;
                }
                sb.AppendLine("[T]:" + key + " [N]:" + _dictmethod[key].Method.Name + "(" + parameters + ")");
            }
            return sb.ToString();

        }

        public string ToString(string msgtype)
        {
            StringBuilder sb = new StringBuilder();
            int type = 0;
            int.TryParse(msgtype, out type);
            if (type > 0)
            {
                string parameters = string.Empty;
                if (_dictmethod.ContainsKey(type))
                {
                    foreach (var item in _dictmethod[type].Method.GetParameters())
                    {
                        if (parameters.Length > 0)
                            parameters += ",";
                        parameters += item.ParameterType.Name + " " + item.Name;
                    }
                    sb.AppendLine("[T]:" + msgtype + " [N]:" + _dictmethod[type].Method.Name + "(" + parameters + ")");
                }
                else
                {
                    sb.AppendLine("查无此api.");
                }
            }
            else
            {
                sb.AppendLine("消息编号格式不正确.");
            }
            return sb.ToString();
        }
    }
}
