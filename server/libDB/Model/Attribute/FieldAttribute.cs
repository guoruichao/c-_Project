using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace libDB
{
    public sealed class FieldAttribute
    {
        /// <summary>
        /// 是否主键.
        /// </summary>
        public bool IsPK { get; set; }
        /// <summary>
        /// 列名.
        /// </summary>
        public string ColumnsName { get; set; }
        /// <summary>
        /// 数据库类型.
        /// </summary>
        public byte DbType { get; set; }

        /// <summary>
        /// 是否有默认值.
        /// </summary>
        public bool HasDefaultValue { get; set; }

        /// <summary>
        /// 是否自动编号.
        /// </summary>
        public bool Isidentity { get; set; }

        /// <summary>
        /// C#类型.
        /// </summary>
        public Type CSharpType { get; set; }
        /// <summary>
        /// 是否guid.
        /// </summary>
        public bool IsGuid { get; set; }
        /// <summary>
        /// 索引.
        /// </summary>
        public byte Index { get; set; }
    }
}
