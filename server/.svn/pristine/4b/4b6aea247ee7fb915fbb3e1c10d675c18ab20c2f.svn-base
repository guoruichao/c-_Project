using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libNet.APIManage;
using libCommon;
namespace libNet
{
    /// <summary>
    /// 接受客户端消息的委托.
    /// </summary>
    /// <typeparam name="TNetMessage"></typeparam>
    /// <param name="message"></param>
    /// <param name="session"></param>
    /// <returns></returns>
    public delegate Result NetMessageHandler<TNetMessage>(TNetMessage message, SessionBase session) where TNetMessage : INetMessage;
    /// <summary>
    /// 消息监听者.
    /// </summary>
    public class NetMessageListener
    {
        Dictionary<int, FastInvokeHandler> _messagehandlerlist;

        public static NetMessageListener Instance;

        static NetMessageListener()
        {
            Instance = new NetMessageListener();
        }

        public NetMessageListener()
        {
            _messagehandlerlist = new Dictionary<int, FastInvokeHandler>();
        }


        /// <summary>
        /// 注册一个消息,消息的类型为INetMessage,当收到客户端消息时，被注册的方法会被调用.
        /// </summary>
        /// <typeparam name="TNetMessage"></typeparam>
        /// <param name="msgtype"></param>
        /// <param name="handler"></param>
        public void Register<TNetMessage>(int msgtype, NetMessageHandler<TNetMessage> handler) where TNetMessage : INetMessage
        {
            if (_messagehandlerlist.ContainsKey(msgtype))
            {
                CommTrace.Error("消息[" + msgtype + "]已经注册了", null);
                return;
            }
            _messagehandlerlist.Add(msgtype, FastInvoke.GetMethodInvoker(handler.Method));
        }

        public void Remove(int msgtype)
        {
            if (_messagehandlerlist.ContainsKey(msgtype))
            {
                _messagehandlerlist[msgtype] = null;
                _messagehandlerlist.Remove(msgtype);
            }
        }

        public Result Handle(INetMessage message, SessionBase session)
        {
            Result result = Result.GetOne();
            try
            {
                result = _messagehandlerlist[message.MsgType](_messagehandlerlist[message.MsgType].Target, new object[] { message, session }) as Result;
                if (result.Success)
                {
                    CommTrace.Error("消息[" + message.MsgType + "]执行成功!", null);
                }
                else
                {
                    CommTrace.Error("消息[" + message.MsgType + "]执行失败!"/* + ReflectionHelper.Dump(message)*/, null);
                }
            }
            catch (Exception ex)
            {
                CommTrace.Error("消息[" + message.MsgType + "]执行出错!", ex, session.SessionId);
            }
            return result;
        }
    }
}
