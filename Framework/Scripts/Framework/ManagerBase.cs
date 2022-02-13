using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 每个模块的基类
///     1.保存自身注册的一系列消息
/// </summary>
public class ManagerBase : MonoBase
{
    /// <summary>
    /// 处理自身的消息
    /// </summary>
    public override void Execute(int eventCode,object message)
    {
        if (!dict.ContainsKey(eventCode))
        {
            Debug.LogWarning("没有注册: "+ eventCode);
            return;
        }
        //一旦注册过这个消息 给所有的脚本 发过去
        List<MonoBase> list = dict[eventCode];
        for (int i = 0; i < list.Count; i++)
        {
            list[i].Execute(eventCode, message);
        }
    }

    /// <summary>
    /// 存储消息的事件码EventCode和哪个脚本关联的字典
    /// 角色模块有个动作是移动
    ///     移动模块需要关心这个事件 控制角色位置进行移动
    ///     动画模块也需要关心这个事件 控制动画
    ///     音效模块也需要关系 控制脚步声音
    /// </summary>
    private Dictionary<int, List<MonoBase>> dict = new Dictionary<int, List<MonoBase>>();
    /// <summary>
    /// 添加事件
    /// </summary>
    /// <param name="eventCode"></param>
    /// <param name="mono"></param>
    public void Add(int eventCode,MonoBase mono)
    {
        List<MonoBase> monoList = null;
        if (!dict.ContainsKey(eventCode))
        {
            //之前没有注册过
            monoList = new List<MonoBase>();
            monoList.Add(mono);
            dict.Add(eventCode, monoList);
            return;
        }
        //之前注册过
        monoList = dict[eventCode];
        monoList.Add(mono);
    }
    /// <summary>
    /// 添加多个事件
    ///     一个脚本关心多个事件
    /// </summary>
    /// <param name="eventCode"></param>
    public void Add(int[] eventCodeArr,MonoBase mono)
    {
        for (int i = 0; i < eventCodeArr.Length; i++)
        {
            Add(eventCodeArr[i], mono);
        }
    }
    /// <summary>
    /// 移除事件对应的脚本
    /// </summary>
    /// <param name="eventCode"></param>
    /// <param name="mono"></param>
    public void Remove(int eventCode,MonoBase mono)
    {
        //没注册过 没法移除 警告
        if (!dict.ContainsKey(eventCode))
        {
            Debug.LogWarning("没有这个事件"+ eventCode + "注册");
            return;
        }

        List<MonoBase> list = dict[eventCode];
        if (list.Count == 1)
            dict.Remove(eventCode);
        else
            list.Remove(mono);
    }

    public void Remove(int[] eventCodeArr, MonoBase mono)
    {
        for (int i = 0; i < eventCodeArr.Length; i++)
        {
            Remove(eventCodeArr[i], mono);
        }
    }
}
