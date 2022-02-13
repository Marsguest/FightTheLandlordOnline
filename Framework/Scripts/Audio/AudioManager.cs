using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 声音模块管理器
/// </summary>
public class AudioManager : ManagerBase
{

    public static AudioManager Instance = null;
    void Awake()
    {
        Instance = this;
        //Debug.LogWarning("这是未挂载到对象的Awake方法产生的");
    }
}
