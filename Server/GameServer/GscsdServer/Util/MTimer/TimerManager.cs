using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using GscsdServer.Util.Concurrent;

namespace GscsdServer.Util.MTimer
{
    /// <summary>
    /// 定时任务（计时器）管理类
    /// </summary>
    public class TimerManager
    {
        /// <summary>
        /// 单例模式
        /// </summary>
        private static TimerManager instance;
        public static TimerManager Instance {
            get
            {
                lock (instance)
                {
                    if (instance == null)
                        instance = new TimerManager();
                    return instance;
                }
            } 
        }

        /// <summary>
        /// 实现定时器的主要功能就是这个Timer类
        /// </summary>
        private Timer timer;
        /// <summary>
        /// 这个字典存储：任务id和任务模型的映射
        /// </summary>
        private ConcurrentDictionary<int, TimerModel> idModelDict = new ConcurrentDictionary<int, TimerModel>();
        /// <summary>
        /// 要移除的任务ID列表
        /// </summary>
        private List<int> removeList = new List<int>();
        /// <summary>
        /// 用来表示id
        /// </summary>
        private ConcurrentInt id = new ConcurrentInt(0);

        public TimerManager()
        {
            timer = new Timer(10);
            timer.Elapsed += Timer_Elapsed;
        }
        /// <summary>
        /// 达到时间间隔时触发
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            lock (removeList)
            {
                TimerModel tmpModel = null;
                foreach (var id in removeList)
                {
                    idModelDict.TryRemove(id, out tmpModel);
                }
                removeList.Clear();
            }
            //不能边循环边清除 所以建立了removeList来清除已经处理好的任务
            foreach (var item in idModelDict)
            {
                // t1 10 + 2  model.Time = 12
                // t2 11  t3 13
                // <=表示已经到这个时间 因此要触发一下
                if(item.Value.Time <= DateTime.Now.Ticks)
                {
                    item.Value.Run();
                    removeList.Add(item.Key);
                }
            }
        }
        /// <summary>
        /// 添加定时任务 指定触发的事件 10点21分
        /// </summary>
        public void AddOnTimerEvent(DateTime dateTime,TimerDelegate timerDelegate)
        {
            long delayTime = dateTime.Ticks - DateTime.Now.Ticks;
            if (delayTime <= 0)
                return;
            AddDelayTimerEvent(delayTime, timerDelegate);
        }
        /// <summary>
        /// 添加定时任务 指定延迟的时间 40s后
        /// </summary>
        /// <param name="delayTime">单位是毫秒</param>
        /// <param name="timerDelegate"></param>
        public void AddDelayTimerEvent(long delayTime,TimerDelegate timerDelegate)
        {
            TimerModel model = new TimerModel(id.Add_Get(),DateTime.Now.Ticks+delayTime,timerDelegate);
            idModelDict.TryAdd(model.Id, model);
        }
    }
}
