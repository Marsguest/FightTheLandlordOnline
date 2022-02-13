using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioBase : MonoBase
{
    /// <summary>
    /// 自身关心的消息集合
    /// </summary>
    public List<int> list = new List<int>();


    /// <summary>
    /// 绑定一个或多个消息
    /// </summary>
    /// <param name="eventCodes"></param>
    protected void Bind(params int[] eventCodes)
    {
        list.AddRange(eventCodes);
        //这种单例是开发框架时用的  具体开发时并不会用到这个单例
        AudioManager.Instance.Add(list.ToArray(), this);
    }
    /// <summary>
    /// 解除绑定的消息
    /// </summary>
    protected void UnBind()
    {
        AudioManager.Instance.Remove(list.ToArray(), this);
        list.Clear();
    }
    public virtual void OnDestory()
    {
        if (list != null)
            UnBind();
    }
    /// <summary>
    /// 发消息
    /// </summary>
    /// <param name="areaCode"></param>
    /// <param name="eventCode"></param>
    /// <param name="message"></param>
    public void Dispatch(int areaCode, int eventCode, object message)
    {
        MsgCenter.Instance.Dispatch(areaCode, eventCode, message);
    }

}
