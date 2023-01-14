using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GL.Cache.ExpireTime
{
    /// <summary>
    /// 间隔类别
    /// </summary>
    public enum IntervalType
    {
        /// <summary>
        /// 无意义 | 0
        /// </summary>
        None = 0,

        /// <summary>
        /// 指定秒钟间隔 | 1
        /// </summary>
        Second = 1,

        /// <summary>
        /// 指定分钟间隔 | 2
        /// </summary>
        Minute = 2,

        /// <summary>
        /// 指定小时间隔 | 3
        /// </summary>
        Hour = 3
    }
}
