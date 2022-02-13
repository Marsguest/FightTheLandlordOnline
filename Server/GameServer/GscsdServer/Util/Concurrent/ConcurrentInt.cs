using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GscsdServer.Util.Concurrent
{
    /// <summary>
    /// 线程安全的int类型
    /// </summary>
    public class ConcurrentInt
    {
        private int value;

        public ConcurrentInt(int _value)
        {
            this.value = _value;
        }
        /// <summary>
        /// 添加并获取
        /// </summary>
        /// <returns></returns>
        public int Add_Get()
        {
            lock (this)
            {
                value++;
                return value;
            }
        }
        /// <summary>
        /// 减少并获取
        /// </summary>
        /// <returns></returns>
        public int Reduce_Get()
        {
            lock (this)
            {
                value--;
                return value;
            }
        }

        public int Get()
        {
            return value;
        }
    }
}
