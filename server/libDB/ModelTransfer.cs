using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using libDB;

namespace libDB
{
    /// <summary>
    /// 数据转换类.
    /// </summary>
    public class ModelTransfer
    {
        private static Dictionary<string, Dictionary<string, PropertyInfo>> _dictCache;
        static ModelTransfer()
        {
            _dictCache = new Dictionary<string, Dictionary<string, PropertyInfo>>();
        }
        /// <summary>
        /// 将T2转换成T1,T1中必须存在和T2 [相同名称，相同类型] 的属性.
        /// </summary>
        /// <typeparam name="T1">想要转换成的对象类型.</typeparam>
        /// <typeparam name="T2">继承自IModel的对象类型.</typeparam>
        /// <param name="model">要转换的数据.</param>
        /// <returns>返回T1</returns>
        public static T1 ConvertTo<T1, T2>(T2 model)
            where T2 : IModel, new()
            where T1 : new()
        {
            Type type = typeof(T1);
            PropertyInfo[] propertys = null;
            Dictionary<string, PropertyInfo> dictPropertys;
            if (_dictCache.ContainsKey(type.Name))
            {
                dictPropertys = _dictCache[type.Name];
            }
            else
            {
                dictPropertys = new Dictionary<string, PropertyInfo>();
                propertys = type.GetProperties();
                foreach (var p in propertys)
                {
                    dictPropertys.Add(p.Name, p);
                }
            }
            T1 target = new T1();
            foreach (var field in AttributeCache<T2>.FieldAttribute.Values)
            {
                PropertyInfo t1p = null;
                PropertyInfo t2p = AttributeCache<T2>.GetModelPropertyInfoExact(field.ColumnsName);
                if (t2p == null)
                    continue;
                dictPropertys.TryGetValue(t2p.Name, out t1p);
                if (t1p == null) continue;

                object obj = DynamicBuilder<T2>.CreateValueBuilder(model, field).BuildValue(model);
                t1p.SetValue(target, obj, null);
            }
            return target;

        }

    }
}
