using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GscsdServer
{
    /// <summary>
    /// 一个需要执行的方法
    /// </summary>
    public delegate void ExecuteDelegate();

    /// <summary>
    /// 单线程池
    /// </summary>
    public class SingleExecute
    {
        /// <summary>
        /// 单例
        /// </summary>
        private static SingleExecute instance = null;
        private static object Singleton_Lock = new object(); //锁同步
        public static SingleExecute Instance
        {
            get
            {
                lock (Singleton_Lock)
                {
                    if(instance == null)
                        instance = new SingleExecute();
                    return instance;
                }
            }
        }

        /// <summary>
        /// 互斥锁
        /// </summary>
        public Mutex mutex;

        private SingleExecute()
        {
            mutex = new Mutex();
        }
        /// <summary>
        /// 单线程处理逻辑
        /// </summary>
        /// <param name="executeDelgate"></param>
        public void Execute(ExecuteDelegate executeDelgate)
        {
            lock (this)
            {
                mutex.WaitOne();
                executeDelgate();
                mutex.ReleaseMutex();
            }
        }
    }
}
