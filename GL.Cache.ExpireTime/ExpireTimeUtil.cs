using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GL.Cache.ExpireTime
{
    /// <summary>
    /// 过期时间帮助类
    /// </summary>
    public class ExpireTimeUtil
    {
        /// <summary>
        /// 缓存设定间隔及其对应的所有间隔数
        /// 例：假设设定间隔为10分钟，则期对应的所有分钟间隔为：0 10 20 30 40 50
        /// key：intervalType_interval
        /// value：interval List
        /// </summary>
        private static readonly ConcurrentDictionary<string, List<int>> allIntervalDic = new ConcurrentDictionary<string, List<int>>();

        /// <summary>
        /// 缓存设定间隔字段的Key模板
        /// </summary>
        private const string INTERVAL_DIC_KEY_FORMAT = "{0}_{1}";

        #region 获取下一次过期时间/需要的分钟数（对外调用）
        /// <summary>
        /// 获取下一次过期时间（适用于需要设置“DateTime”的场景）
        /// </summary>
        /// <param name="interval">设定的间隔 秒钟/分钟/小时 数（依据 intervalType 入参来界定）</param>
        /// <param name="intervalType">设定类别</param>
        /// <param name="settingTime">设定时间（如果不传，则为当前时间，建议不传）</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">
        /// 1. 当 interval <= 0 时，会提示“设定的间隔数必须大于0”
        /// 2. 当 IntervalType 不是已知枚举时，会提示“未找到对应间隔类别”
        /// </exception>
        public static DateTime GetNextExpireTime(int interval, IntervalType intervalType, DateTime? settingTime = null)
        {
            //入参校验
            CheckParams(interval, intervalType, settingTime);

            switch (intervalType)
            {
                case IntervalType.Second:
                    return GetNextExpireTimeBySecond(interval, settingTime);
                case IntervalType.Minute:
                    return GetNextExpireTimeByMinute(interval, settingTime);
                case IntervalType.Hour:
                    return GetNextExpireTimeByHour(interval, settingTime);
                default:
                    return GetNextExpireTimeByMinute(interval, settingTime);
            }
        }

        /// <summary>
        /// 获取下一次过期需要多少分钟（适用于需要“设置多少分钟后过期”的场景）
        /// </summary>
        /// <param name="interval">设定的间隔 秒钟/分钟/小时 数（依据 intervalType 入参来界定）</param>
        /// <param name="intervalType">设定类别</param>
        /// <param name="settingTime">设定时间（如果不传，则为当前时间，建议不传）</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">
        /// 1. 当 interval <= 0 时，会提示“设定的间隔数必须大于0”
        /// 2. 当 IntervalType 不是已知枚举时，会提示“未找到对应间隔类别”
        /// </exception>
        public static double GetNextExpireMinute(int interval, IntervalType intervalType, DateTime? settingTime = null)
        {
            //入参校验
            CheckParams(interval, intervalType, settingTime);

            switch (intervalType)
            {
                case IntervalType.Second:
                    return GetNextExpireMinuteBySecond(interval, settingTime);
                case IntervalType.Minute:
                    return GetNextExpireMinuteByMinute(interval, settingTime);
                case IntervalType.Hour:
                    return GetNextExpireMinuteByHour(interval, settingTime);
                default:
                    return GetNextExpireMinuteByMinute(interval, settingTime);
            }
        }

        /// <summary>
        /// 方法入参校验
        /// </summary>
        /// <param name="interval"></param>
        /// <param name="intervalType"></param>
        /// <param name="settingTime"></param>
        /// <exception cref="ArgumentException">
        /// 1. 当 interval <= 0 时，会提示“设定的间隔数必须大于0”
        /// 2. 当 IntervalType 不是已知枚举时，会提示“未找到对应间隔类别”
        /// </exception>
        private static void CheckParams(int interval, IntervalType intervalType, DateTime? settingTime)
        {
            if (interval <= 0)
            {
                throw new ArgumentException("设定的间隔数必须大于0");
            }

            int intervalTypeInt = (int)intervalType;
            if (intervalTypeInt <= 0 || intervalTypeInt > 3)
            {
                throw new ArgumentException("未找到对应间隔类别");
            }
        }
        #endregion

        #region 获取下一次过期时间（指定秒钟间隔）
        /// <summary>
        /// 获取下一次过期时间（指定秒钟间隔）（适用于需要设置“DateTime”的场景）
        /// </summary>
        /// <param name="interval">设定的间隔秒钟数</param>
        /// <param name="settingTime">设定时间（如果不传，则为当前时间，建议不传）</param>
        /// <returns>下一次过期时间</returns>
        /// <exception cref="ArgumentException">当给定的 interval 小于等于 0 时，会触发此错误</exception>
        private static DateTime GetNextExpireTimeBySecond(int interval, DateTime? settingTime = null)
        {
            //获取设定时间
            DateTime useTime = settingTime ?? DateTime.Now;
            useTime = useTime.AddMilliseconds(-useTime.Millisecond);

            //获取下一次过期的间隔分钟
            int nextMinute = GetNextInterval(useTime, interval, IntervalType.Second);

            if (nextMinute > 0)
            {
                return useTime.AddSeconds(nextMinute - useTime.Second);
            }
            return useTime.AddMinutes(1).AddSeconds(-useTime.Second);
        }

        /// <summary>
        /// 获取下一次过期需要多少分钟（指定秒钟间隔）（适用于需要“设置多少分钟后过期”的场景）
        /// </summary>
        /// <param name="interval">设定的间隔秒钟数</param>
        /// <param name="settingTime">设定时间（如果不传，则为当前时间，建议不传）</param>
        /// <returns>下一次过期时间</returns>
        /// <exception cref="ArgumentException">当给定的 interval 小于等于 0 时，会触发此错误</exception>
        private static double GetNextExpireMinuteBySecond(int interval, DateTime? settingTime = null)
        {
            DateTime nowTime = DateTime.Now;
            DateTime nextExpireTime = GetNextExpireTimeBySecond(interval, settingTime);
            return (nextExpireTime - nowTime).TotalMinutes;
        }
        #endregion

        #region 获取下一次过期时间（指定分钟间隔）
        /// <summary>
        /// 获取下一次过期时间（指定分钟间隔）（适用于需要设置“DateTime”的场景）
        /// </summary>
        /// <param name="interval">设定的间隔分钟数</param>
        /// <param name="settingTime">设定时间（如果不传，则为当前时间，建议不传）</param>
        /// <returns>下一次过期时间</returns>
        /// <exception cref="ArgumentException">当给定的 interval 小于等于 0 时，会触发此错误</exception>
        private static DateTime GetNextExpireTimeByMinute(int interval, DateTime? settingTime = null)
        {
            //获取设定时间
            DateTime useTime = GetUseTime(settingTime);

            //获取下一次过期的间隔分钟
            int nextMinute = GetNextInterval(useTime, interval, IntervalType.Minute);

            if (nextMinute > 0)
            {
                return useTime.AddMinutes(nextMinute - useTime.Minute);
            }
            return useTime.AddHours(1).AddMinutes(-useTime.Minute);
        }

        /// <summary>
        /// 获取下一次过期需要多少分钟（指定分钟间隔）（适用于需要“设置多少分钟后过期”的场景）
        /// </summary>
        /// <param name="interval">设定的间隔分钟数</param>
        /// <param name="settingTime">设定时间（如果不传，则为当前时间，建议不传）</param>
        /// <returns>下一次过期时间</returns>
        /// <exception cref="ArgumentException">当给定的 interval 小于等于 0 时，会触发此错误</exception>
        private static double GetNextExpireMinuteByMinute(int interval, DateTime? settingTime = null)
        {
            DateTime nowTime = DateTime.Now;
            DateTime nextExpireTime = GetNextExpireTimeByMinute(interval, settingTime);
            return (nextExpireTime - nowTime).TotalMinutes;
        }
        #endregion

        #region 获取下一次过期时间（指定小时间隔）
        /// <summary>
        /// 获取下一次过期时间（指定小时间隔）（适用于需要设置“DateTime”的场景）
        /// </summary>
        /// <param name="interval">设定的间隔小时数</param>
        /// <param name="settingTime">设定时间（如果不传，则为当前时间，建议不传）</param>
        /// <returns>下一次过期时间</returns>
        /// <exception cref="ArgumentException">当给定的 interval 小于等于 0 时，会触发此错误</exception>
        private static DateTime GetNextExpireTimeByHour(int interval, DateTime? settingTime = null)
        {
            //获取设定时间
            DateTime useTime = GetUseTime(settingTime);
            useTime = useTime.AddMinutes(-useTime.Minute);

            //获取下一次过期的间隔小时
            int nextHour = GetNextInterval(useTime, interval, IntervalType.Hour);

            if (nextHour > 0)
            {
                return useTime.AddHours(nextHour - useTime.Hour);
            }
            return useTime.AddDays(1).AddHours(-useTime.Hour);
        }

        /// <summary>
        /// 获取下一次过期需要多少分钟（指定小时间隔）（适用于需要“设置多少分钟后过期”的场景）
        /// </summary>
        /// <param name="interval">设定的间隔小时数</param>
        /// <param name="settingTime">设定时间（如果不传，则为当前时间，建议不传）</param>
        /// <returns>下一次过期需要的分钟数</returns>
        /// <exception cref="ArgumentException">当给定的 interval 小于等于 0 时，会触发此错误</exception>
        private static double GetNextExpireMinuteByHour(int interval, DateTime? settingTime = null)
        {
            DateTime nowTime = DateTime.Now;
            DateTime nextExpireTime = GetNextExpireTimeByHour(interval, settingTime);
            return (nextExpireTime - nowTime).TotalMinutes;
        }
        #endregion

        /// <summary>
        /// 获取设定时间
        /// </summary>
        /// <param name="settingTime"></param>
        /// <returns></returns>
        private static DateTime GetUseTime(DateTime? settingTime)
        {
            //获取设定时间（只要 年月日时分）
            DateTime useTime = settingTime ?? DateTime.Now;
            useTime = useTime.AddSeconds(-useTime.Second).AddMilliseconds(-useTime.Millisecond);
            return useTime;
        }

        /// <summary>
        /// 获取下一次过期的间隔数
        /// </summary>
        /// <param name="useTime">设定时间</param>
        /// <param name="interval">设定的间隔数</param>
        /// <param name="intervalType">设定类别</param>
        /// <returns>下一次过期的间隔分钟</returns>
        private static int GetNextInterval(DateTime useTime, int interval, IntervalType intervalType)
        {
            //获取所有可使用的间隔数集合
            List<int> allIntervalList = GetAllInterval(interval, intervalType);

            int nextInterval = 0;
            foreach (int intervalItem in allIntervalList)
            {
                if (intervalItem > GetNumber(useTime, intervalType))
                {
                    nextInterval = intervalItem;
                    break;
                }
            }

            return nextInterval;
        }

        /// <summary>
        /// 获取指定间隔类别的数字
        /// </summary>
        /// <param name="useTime">设定时间</param>
        /// <param name="intervalType">设定类别</param>
        /// <returns></returns>
        private static int GetNumber(DateTime useTime, IntervalType intervalType)
        {
            switch (intervalType)
            {
                case IntervalType.Second:
                    return useTime.Second;
                case IntervalType.Minute:
                    return useTime.Minute;
                case IntervalType.Hour:
                    return useTime.Hour;
                default:
                    return useTime.Minute;
            }
        }

        /// <summary>
        /// 获取所有可使用的间隔数集合
        /// </summary>
        /// <param name="interval">设定的间隔数</param>
        /// <param name="intervalType">设定类别</param>
        /// <returns>所有可使用的分钟集合</returns>
        private static List<int> GetAllInterval(int interval, IntervalType intervalType)
        {
            //获取缓存的间隔集合
            string dicKey = string.Format(INTERVAL_DIC_KEY_FORMAT, intervalType, interval);
            if (allIntervalDic.ContainsKey(dicKey))
            {
                if (allIntervalDic.TryGetValue(dicKey, out var result))
                {
                    return result;
                }
            }

            //生成间隔集合并缓存
            int nextInterval = 0;
            List<int> allIntervalList = new List<int>(20) { nextInterval };
            for (int i = 0; i < 60; i++)
            {
                nextInterval += interval;
                if (nextInterval >= GetMaxInterval(intervalType))
                {
                    break;
                }

                allIntervalList.Add(nextInterval);
            }

            allIntervalDic.TryAdd(dicKey, allIntervalList);
            return allIntervalList;
        }

        /// <summary>
        /// 获取设定类别的最大间隔
        /// </summary>
        /// <param name="intervalType">设定类别</param>
        /// <returns></returns>
        private static int GetMaxInterval(IntervalType intervalType)
        {
            switch (intervalType)
            {
                case IntervalType.Second:
                    return 60;
                case IntervalType.Minute:
                    return 60;
                case IntervalType.Hour:
                    return 24;
                default:
                    return 60;
            }
        }



        #region 测试：获取下 10 次过期时间
        /// <summary>
        /// 测试：获取下 10 次过期时间（测试用）
        /// </summary>
        /// <param name="interval">设定的间隔数</param>
        /// <param name="intervalType">设定类别</param>
        /// <param name="settingTime">设定时间（如果不传，则为当前时间，建议不传）</param>
        /// <returns>Tuple中：item1：过期时间，item2：多少分钟后过期</returns>
        public static List<Tuple<DateTime, double>> GetNext10ExpireTimeTest(int interval, IntervalType intervalType, DateTime? settingTime = null)
        {
            List<Tuple<DateTime, double>> next10ExpireTimeList = new(10);
            for (int i = 0; i < 10; i++)
            {
                DateTime nextExpireTime = GetNextExpireTime(interval, intervalType, settingTime);
                double nextMinute = GetNextExpireMinute(interval, intervalType, settingTime);
                next10ExpireTimeList.Add(new Tuple<DateTime, double>(nextExpireTime, nextMinute));
                settingTime = nextExpireTime;
            }

            return next10ExpireTimeList;
        }
        #endregion
    }
}
