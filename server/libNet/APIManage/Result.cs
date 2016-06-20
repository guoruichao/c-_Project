using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

/// <summary>
/// 结果集.
/// </summary>


namespace libNet.APIManage
{

    public class Result
    {
        private static object _resultlock = new object();
        private static Queue<Result> _pools;
        private Dictionary<string, object> _dict;
        private List<string> _alertmsg;
        private List<string> _errormsg;
        private bool _issuccess;

        static Result()
        {
            _pools = new Queue<Result>();
            for (int i = 0; i < 100; i++)
            {
                _pools.Enqueue(new Result());
            }
        }

        public Result()
        {
            _dict = new Dictionary<string, object>();
            _alertmsg = new List<string>();
            _errormsg = new List<string>();
        }


        /// <summary>
        /// 增加一个要传递到客户端的数据对象.
        /// </summary>
        /// <param name="k"></param>
        /// <param name="v"></param>
        public Result Add(string k, object v)
        {
            k = k.ToLower();
            if (k == "ret" || k == "alertmsg" || k == "errormsg")
                throw new Exception("ret,alertmsg,errormsg 为预定义键!");
            if (v == null)
                return this;
            if (_dict.ContainsKey(k))
                _dict[k] = v;
            else
                _dict.Add(k, v);
            return this;
        }

        /// <summary>
        /// 函数的执行结果.
        /// </summary>
        public bool Success { get { return _issuccess; } }

        /// <summary>
        /// 是否反馈.
        /// </summary>
        public bool FeedBack { get; set; }


        /// <summary>
        /// 警告提示客户端.
        /// </summary>
        /// <param name="msg"></param>
        public Result Alert(string msg)
        {
            _alertmsg.Add(msg);
            _issuccess = false;
            return this;
        }

        /// <summary>
        /// 错误提示客户端.
        /// </summary>
        /// <param name="msg"></param>
        public Result Error(string msg)
        {
            _errormsg.Add(msg);
            _issuccess = false;
            return this;
        }

        /// <summary>
        /// 方法执行结果.
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        public Result SetSuccess(bool r)
        {
            _issuccess = r;
            return this;
        }

        /// <summary>
        /// 合并结果集.
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public Result Merger(Result target)
        {
            foreach (var key in target._dict.Keys)
            {
                if (_dict.ContainsKey(key))
                    _dict[key] = target._dict[key];
                else
                    _dict.Add(key, target._dict[key]);
            }
            _issuccess = target._issuccess;
            _alertmsg.AddRange(target._alertmsg);
            _errormsg.AddRange(target._errormsg);
            return this;
        }

        /// <summary>
        /// 打包结果集.
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, object> Package(int msgtype = 0)
        {
            if (msgtype > 0)
            {
                if (!_dict.ContainsKey("msgtype"))
                    _dict.Add("msgtype", msgtype);
                else
                    _dict["msgtype"] = msgtype;
            }

            if (!_dict.ContainsKey("ret"))
                _dict.Add("ret", _issuccess);
            if (!_dict.ContainsKey("alertmsg") && _alertmsg.Count > 0)
                _dict.Add("alertmsg", _alertmsg);
            if (!_dict.ContainsKey("errormsg") && _errormsg.Count > 0)
                _dict.Add("errormsg", _errormsg);
            return _dict;
        }


        /// <summary>
        /// 从结果集的池中获得一个结果.
        /// </summary>
        /// <returns></returns>
        public static Result GetOne()
        {
            Result result = _pools.Dequeue();
            if (result == null)
            {
                result = new Result();
            }
            return result;
        }

        public void Dispose()
        {
            _dict.Clear();
            _alertmsg.Clear();
            _errormsg.Clear();
            _issuccess = false;
            _pools.Enqueue(this);
        }
    } 

}