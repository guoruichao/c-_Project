using System;
using System.Collections.Generic;
using System.Linq;
using System.Text; 

namespace libNet
{
    /// <summary>
    /// 传递的消息.
    /// </summary>
    public class Message
    {
        private static Queue<Message> _pool;
        private static object _lockmsg = new object();
        private bool _isdispose;
        static Message()
        {
            _pool = new Queue<Message>(1000);
            for (int i = 0; i < 1000; i++)
            {
                _pool.Enqueue(new Message());
            }
        }
        ~Message()
        {
            if (!_isdispose)
                Dispose();
        }
        /// <summary>
        /// 消息长度.
        /// </summary>
        public int Size { get; set; }
        /// <summary>
        /// 消息内容.
        /// </summary>
        public byte[] Body { get; set; }

        public void Dispose()
        {
            if (Body != null)
            {
                Array.Clear(Body, 0, Body.Length);
                Body = null;
            }
            Size = 0;
            lock (_lockmsg)
            {
                _pool.Enqueue(this);
            }
            _isdispose = true; 
        }

        public static Message GetOne()
        {
            Message msg = null;
            lock (_lockmsg)
            {
                if (_pool.Count > 0)
                    msg = _pool.Dequeue();
            }
            if (msg == null)
                msg = new Message();
            return msg;
        }
    }
}
