using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace libNet
{
    /// <summary>
    /// 序列化规则.
    /// </summary>
    public interface INetSerialize
    {
        /// <summary>
        /// 序列化.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        byte[] Serialize<T>(T data);
        /// <summary>
        /// 反序列化.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        T Deserialize<T>(byte[] data);
    }
}
