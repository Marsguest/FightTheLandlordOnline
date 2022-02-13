using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GscsdServer.Util.MTimer
{
    /// <summary>
    /// 当定时器达到时间后的触发 委托
    /// </summary>
    public delegate void TimerDelegate();
    /// <summary>
    /// 定时器任务的数据模型
    /// </summary>
    public class TimerModel
    {
        public int Id;
        /// <summary>
        /// 任务执行的时间
        /// </summary>
        public long Time;

        private TimerDelegate timerDelegate;

        public TimerModel(int _id,long _time, TimerDelegate _td)
        {
            this.Id = _id;
            this.Time = _time;
            this.timerDelegate = _td;
        }
        /// <summary>
        /// 触发任务的
        /// </summary>
        public void Run()
        {
            timerDelegate();
        }

        
    }
}
