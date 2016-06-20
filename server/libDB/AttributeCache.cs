using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Reflection;
using libDB;
namespace libDB
{
    /// <summary>
    /// IModel的属性缓存.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal static class AttributeCache<T> where T : IModel, new()
    {
        internal static Dictionary<string, PropertyInfo> cacheExactProperties;
        internal static Dictionary<string, FieldAttribute> FieldAttribute;
        internal static Dictionary<string, PropertyInfo> cacheProperties;
        public static ModelAttribute ModelAttribute;
        /// <summary>
        /// 连接字符串.
        /// </summary>
        public static string Conn;
        static AttributeCache()
        {
            FieldAttribute = new Dictionary<string, FieldAttribute>();
            cacheExactProperties = new Dictionary<string, PropertyInfo>();
            cacheProperties = new Dictionary<string, PropertyInfo>();
            foreach (var p in typeof(T).GetProperties())
            {
                ModelFieldAttribute mf = p.GetCustomAttributes(false).SingleOrDefault(obj => obj is ModelFieldAttribute) as ModelFieldAttribute;
                if (mf == null) continue;
                string[] attributes = mf.Attributes.Split(',');
                FieldAttribute fileAttribute = new FieldAttribute();
                fileAttribute.ColumnsName = attributes[0];
                fileAttribute.CSharpType = DalHelper.GetCsharpType(attributes[2]);
                fileAttribute.DbType = Convert.ToByte(attributes[2]);
                fileAttribute.HasDefaultValue = attributes[3] == "1" ? true : false;
                fileAttribute.Index = Convert.ToByte(attributes[6]);
                fileAttribute.IsGuid = attributes[5] == "1" ? true : false; ;
                fileAttribute.Isidentity = attributes[4] == "1" ? true : false;
                fileAttribute.IsPK = attributes[1] == "1" ? true : false;
                cacheProperties.Add(p.Name.ToUpper(), p);
                cacheExactProperties.Add(fileAttribute.ColumnsName.ToUpper(), p);
                FieldAttribute.Add(p.Name.ToUpper(), fileAttribute);
            }
            ModelAttribute = typeof(T).GetCustomAttributes(false).Where<object>(obj => obj is ModelAttribute).Cast<ModelAttribute>().FirstOrDefault();
            Conn = DalSetting.GetConn(ModelAttribute.DbName);
            ModelAttribute.DbName = DalSetting.GetDbName(ModelAttribute.DbName);
        }

        /// <summary>
        /// 获得缓存的PropertyInfo
        /// </summary>
        /// <param name="properInfoName">属性名称.</param>
        /// <returns></returns>
        public static PropertyInfo GetModelPropertyInfo(string properInfoName)
        {
            try
            {
                return cacheProperties[properInfoName.ToUpper()];
            }
            catch (Exception ex)
            {
                DBTrace.Fatal(properInfoName + "错误", ex);
                return null;
            }
        }

        /// <summary>
        /// 精确获得缓存的PropertyInfo,当需要提交，修改数据库时必须调用此方法.
        /// </summary>
        /// <param name="properInfoName">属性名称.</param>
        /// <returns></returns>
        public static PropertyInfo GetModelPropertyInfoExact(string dbColumnsName)
        {
            try
            {
                return cacheExactProperties[dbColumnsName.ToUpper()];
            }
            catch
            {
                return null;
            }
        }


        /// <summary>
        /// 获得缓存的FieldAttribute
        /// </summary>
        /// <param name="properInfoName">属性名称.</param>
        /// <returns></returns>
        internal static FieldAttribute GetModelFieldAttribute(string properInfoName)
        {
            return FieldAttribute[properInfoName.ToUpper()];
        }
    }
}
