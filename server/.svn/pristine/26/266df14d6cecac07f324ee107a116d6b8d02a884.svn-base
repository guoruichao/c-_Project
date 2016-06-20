using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace libDB
{
    public interface IModel
    {
        /// <summary>
        /// 检查Model的某个字段是否被赋值.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        int Change(int index);
        /// <summary>
        /// 给Model所有的字段赋值.
        /// </summary>
        void Init();
        /// <summary>
        /// 判断Model是否为一个新Model.
        /// </summary>
        /// <returns></returns>
        bool IsNew();

    }
}
