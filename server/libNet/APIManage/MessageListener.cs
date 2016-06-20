using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
namespace libNet.APIManage
{
    public delegate Result MessageHandler(SessionBase session, object[] clientparas);
    public class MessageListener
    {
        private static Dictionary<int, MessageHandler> _messageApilist;
        public static MessageListener Instance;
        static MessageListener()
        {
            Instance = new MessageListener();
        }

        public MessageListener()
        {
            _messageApilist = new Dictionary<int, MessageHandler>();

        }

        /// <summary>
        /// 注册一个事件.
        /// </summary>
        /// <param name="msgtype"></param>
        /// <param name="handler"></param>
        public void RegisterReceiveEventHandle(int msgtype, MessageHandler handler)
        {
            if (_messageApilist.ContainsKey(msgtype))
                _messageApilist[msgtype] += handler;
            else
            {
                MessageHandler oldHander;
                if (!_messageApilist.TryGetValue(msgtype, out oldHander))
                {
                    _messageApilist.Add(msgtype, handler);
                } 
            }
        }


        /// <summary>
        /// 移除一个监听.
        /// </summary>
        /// <param name="msgtype"></param>
        /// <param name="handler"></param>
        public void RemoveReceiveEventHandle(int msgtype, MessageHandler handler)
        {
            if (!_messageApilist.ContainsKey(msgtype))
                return;
            _messageApilist[msgtype] -= handler;
        }


        /// <summary>
        /// 调用函数.
        /// </summary>
        /// <param name="session"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public Result Call(SessionBase session, APIMessage msg)
        {
            Result result = null;
            MessageHandler handler;
            _messageApilist.TryGetValue(msg.Type, out handler);
            if (handler == null)
                return result;
            result = handler(session, msg.Parameters);
            return result;
        }

    }
}
