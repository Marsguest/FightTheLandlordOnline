using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 消息处理中心
///     只负责消息的转发
///     ui -》msgCenter -》 character
/// </summary>
public class MsgCenter : MonoBase
{
    public static MsgCenter Instance = null;

    void Awake()
    {
        Instance = this;

        gameObject.AddComponent<AudioManager>();
        gameObject.AddComponent<UIManager>();
        gameObject.AddComponent<NetManager>();
        gameObject.AddComponent<CharacterManager>();
        gameObject.AddComponent<SceneMgr>();

        DontDestroyOnLoad(gameObject);
    }
    /// <summary>
    /// 发送消息 系统里面所有的发消息 都通过这个方法来发
    ///     如何转发？根据不同的模块来发给不同的模块
    ///     如何识别模块？通过AreaCode
    /// </summary>
    /// <param name="areaCode">区域码</param>
    /// <param name="eventCode">事件码 用于区分来做什么事情的</param>
    /// <param name="message">动作事件的参数</param>
    public void Dispatch(int areaCode,int eventCode,object message)
    {
        switch(areaCode)
        {
            case AreaCode.AUDIO:
                AudioManager.Instance.Execute(eventCode, message);
                break;
            case AreaCode.CHARACTER:
                CharacterManager.Instance.Execute(eventCode, message);
                break;
            case AreaCode.NET:
                NetManager.Instance.Execute(eventCode, message);
                break;
            case AreaCode.GAME:
                break;
            case AreaCode.UI:
                UIManager.Instance.Execute(eventCode, message);
                break;
            case AreaCode.SCENE:
                SceneMgr.Instance.Execute(eventCode, message);
                break;
            default:
                break;
        }
    }
}
