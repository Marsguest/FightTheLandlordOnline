using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 场景管理器
/// </summary>
public class SceneMgr : ManagerBase
{
    public static SceneMgr Instance = null;
    private void Awake()
    {
        Instance = this;

        //SceneManager.sceneLoaded += SceneManager_sceneLoaded;

        Add(SceneEvent.LOAD_SCENE, this);
    }
    /// <summary>
    /// 当场景加载完成时调用
    /// </summary>
    /// <param name="arg0"></param>
    /// <param name="arg1"></param>
    private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        if (onSceneLoaded != null)
        {
            onSceneLoaded();
            //fix bug
            onSceneLoaded = null;
        }
           
    }

    public override void Execute(int eventCode, object message)
    {
        switch (eventCode)
        {
            case SceneEvent.LOAD_SCENE:
                LoadSceneMsg msg = message as LoadSceneMsg;
                loadScene(msg);
                break;
            default:
                break;
        }
    }
    /// <summary>
    /// 临时变量
    /// </summary>
    private Action onSceneLoaded = null;
    /// <summary>
    /// 加载场景
    /// </summary>
    /// <param name="sceneIndex"></param>
    private void loadScene(LoadSceneMsg msg)
    {
        if(msg.SceneBuildIndex != -1)
            SceneManager.LoadScene(msg.SceneBuildIndex);

        if (msg.SceneBuildName != null)
            SceneManager.LoadScene(msg.SceneBuildName);

        if (msg.OnSceneLoaded != null)
        {
            onSceneLoaded = msg.OnSceneLoaded;
            SceneManager.sceneLoaded += SceneManager_sceneLoaded;
        }
            
    }
}
