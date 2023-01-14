using Microsoft.VisualStudio.TestTools.UnitTesting;
using GL.Cache.ExpireTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography.X509Certificates;

namespace GL.Cache.ExpireTime.Tests
{
    [TestClass()]
    public class ExpireTimeUtilTests
    {
        #region 按设定分钟间隔获取下一次过期时间 单元测试
        /// <summary>
        /// 基础使用测试
        /// </summary>
        [TestMethod()]
        public void GetNextExpireTimeTest1()
        {
            DateTime nextExpireTime = ExpireTimeUtil.GetNextExpireTime(10, IntervalType.Minute);
            Assert.IsTrue(nextExpireTime > DateTime.Now);
        }

        /// <summary>
        /// 设定时间测试
        /// </summary>
        [TestMethod()]
        public void GetNextExpireTimeTest2()
        {
            DateTime settingTime = DateTime.Now.AddDays(16);
            DateTime nextExpireTime = ExpireTimeUtil.GetNextExpireTime(10, IntervalType.Minute, settingTime);
            Assert.IsTrue(nextExpireTime > settingTime);
        }

        /// <summary>
        /// 多线程访问测试
        /// </summary>
        [TestMethod()]
        public void GetNextExpireTimeTest3()
        {
            Thread t1 = new Thread(x => ExpireTimeUtil.GetNextExpireTime(10, IntervalType.Minute));
            Thread t2 = new Thread(x => ExpireTimeUtil.GetNextExpireTime(10, IntervalType.Minute));
            Thread t3 = new Thread(x => ExpireTimeUtil.GetNextExpireTime(10, IntervalType.Minute));
            Thread t4 = new Thread(x => ExpireTimeUtil.GetNextExpireTime(10, IntervalType.Minute));
            Thread t5 = new Thread(x => ExpireTimeUtil.GetNextExpireTime(10, IntervalType.Minute));

            Thread t6 = new Thread(x => ExpireTimeUtil.GetNextExpireTime(17, IntervalType.Minute));
            Thread t7 = new Thread(x => ExpireTimeUtil.GetNextExpireTime(17, IntervalType.Minute));
            Thread t8 = new Thread(x => ExpireTimeUtil.GetNextExpireTime(17, IntervalType.Minute));
            Thread t9 = new Thread(x => ExpireTimeUtil.GetNextExpireTime(17, IntervalType.Minute));
            Thread t10 = new Thread(x => ExpireTimeUtil.GetNextExpireTime(17, IntervalType.Minute));

            t1.Start();
            t2.Start();
            t3.Start();
            t4.Start();
            t5.Start();
            t6.Start();
            t7.Start();
            t8.Start();
            t9.Start();
            t10.Start();

            for (int i = 0; i < 10000; i++)
            {
                Task.Factory.StartNew(() => { ExpireTimeUtil.GetNextExpireTime(27, IntervalType.Minute); });
            }

            Thread.Sleep(TimeSpan.FromSeconds(10));

            Assert.IsTrue(true);
        }
        #endregion


        #region 按设定小时间隔获取下一次过期时间 单元测试
        /// <summary>
        /// 基础使用测试
        /// </summary>
        [TestMethod()]
        public void GetNextExpireTimeTest4()
        {
            DateTime nextExpireTime = ExpireTimeUtil.GetNextExpireTime(5, IntervalType.Hour);
            Assert.IsTrue(nextExpireTime > DateTime.Now);
        }

        /// <summary>
        /// 设定时间测试
        /// </summary>
        [TestMethod()]
        public void GetNextExpireTimeTest5()
        {
            DateTime settingTime = DateTime.Now.AddDays(16);
            DateTime nextExpireTime = ExpireTimeUtil.GetNextExpireTime(10, IntervalType.Hour, settingTime);
            Assert.IsTrue(nextExpireTime > settingTime);
        }

        /// <summary>
        /// 多线程访问测试
        /// </summary>
        [TestMethod()]
        public void GetNextExpireTimeTest6()
        {
            Thread t1 = new Thread(x => ExpireTimeUtil.GetNextExpireTime(10, IntervalType.Hour));
            Thread t2 = new Thread(x => ExpireTimeUtil.GetNextExpireTime(10, IntervalType.Hour));
            Thread t3 = new Thread(x => ExpireTimeUtil.GetNextExpireTime(10, IntervalType.Hour));
            Thread t4 = new Thread(x => ExpireTimeUtil.GetNextExpireTime(10, IntervalType.Hour));
            Thread t5 = new Thread(x => ExpireTimeUtil.GetNextExpireTime(10, IntervalType.Hour));

            Thread t6 = new Thread(x => ExpireTimeUtil.GetNextExpireTime(17, IntervalType.Hour));
            Thread t7 = new Thread(x => ExpireTimeUtil.GetNextExpireTime(17, IntervalType.Hour));
            Thread t8 = new Thread(x => ExpireTimeUtil.GetNextExpireTime(17, IntervalType.Hour));
            Thread t9 = new Thread(x => ExpireTimeUtil.GetNextExpireTime(17, IntervalType.Hour));
            Thread t10 = new Thread(x => ExpireTimeUtil.GetNextExpireTime(17, IntervalType.Hour));

            t1.Start();
            t2.Start();
            t3.Start();
            t4.Start();
            t5.Start();
            t6.Start();
            t7.Start();
            t8.Start();
            t9.Start();
            t10.Start();

            for (int i = 0; i < 10000; i++)
            {
                Task.Factory.StartNew(() => { ExpireTimeUtil.GetNextExpireTime(1, IntervalType.Hour); });
            }

            Thread.Sleep(TimeSpan.FromSeconds(10));

            Assert.IsTrue(true);
        }
        #endregion

        [TestMethod()]
        public void GetNext10ExpireTimeTestTest()
        {
            List<Tuple<DateTime, double>> dataList = ExpireTimeUtil.GetNext10ExpireTimeTest(10, IntervalType.Minute);
            Console.WriteLine($"开始输出最近10次的时间，当前时间为：{DateTime.Now}");
            foreach (Tuple<DateTime, double> data in dataList)
            {
                Console.WriteLine($"{data.Item1}，过期分钟为：{data.Item2}，组合当前时间后的新时间为：{DateTime.Now.AddMinutes(data.Item2)}");
            }

            Assert.IsTrue(dataList.Count == 10);
        }
    }
}