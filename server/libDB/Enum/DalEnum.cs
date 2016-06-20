using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace libDB
{
    /// <summary>
    /// MSSQL锁类型.
    /// </summary>

    public enum LockEnum
    {
        /// <summary>
        /// 默认.
        /// </summary>
        None = -1,
        /// <summary>
        /// 无锁.
        /// </summary>
        WithNoLock = 0,
        /// <summary>
        /// 行级锁.
        /// </summary>
        WithRowLock = 1
    }

    internal enum InsertEnum
    {
        Commond = 1,
        Guid = 2,
        Identity = 3
    }
}
