using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace libNet.APIManage
{

    public class ClientResult
    {
        private Dictionary<string, object> _msg;
        public ClientResult(Dictionary<string, object> msg)
        {
            _msg = msg;
        }

        public T GetValue<T>(string key)
        {
            if (_msg.ContainsKey(key))
                return (T)_msg[key];
            else
                return default(T);
        }

        public List<T> GetListT<T>(string key)
        {
            IList tmplist = _msg[key] as IList;
            List<T> list = new List<T>();
            foreach (var item in tmplist)
            {
                if (item == null) continue;
                list.Add((T)item);
            }
            return list;
        }

        public IList GetArray(string key)
        {
            if (_msg.ContainsKey(key))
                return _msg[key] as IList;
            return null;
        }
    }

}
