using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using libNet.Enum;
namespace libNet.APIManage
{
    /// <summary>
    /// API方法标记.
    /// 
    /// </summary>
    public class APIMethod : Attribute
    {
        private bool _feedback;
        public APIMethod()
        {
            _feedback = true;
        }
        /// <summary>
        /// 唯一标识.
        /// </summary>
        public int Name { get; set; }
        /// <summary>
        /// 访问间隔(毫秒).
        /// </summary>
        public int VisitTimeSpan { get; set; }
        /// <summary>
        /// 代理服务器消息.
        /// </summary>
        public bool AgentMessage { get; set; }
        /// <summary>
        /// 方法.
        /// </summary>
        internal FastInvokeHandler FastMethod { get; set; }
        internal MethodInfo Method { get; set; }
        /// <summary>
        ///反馈给客户端.
        ///此属性默认为True.
        ///true----将函数的执行结果以消息的形式反馈给客户端.
        ///false----函数的执行结果将不会反馈给客户端.
        /// </summary>
        public bool Feedback { get { return _feedback; } set { _feedback = value; } }


    }


}
