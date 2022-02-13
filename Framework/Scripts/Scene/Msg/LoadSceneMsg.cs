using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadSceneMsg
{
    public int SceneBuildIndex;
    public string SceneBuildName;
    public Action OnSceneLoaded;

    public LoadSceneMsg()
    {
        this.SceneBuildIndex = -1;
        this.SceneBuildName = null;
        this.OnSceneLoaded = null;
    }

    public LoadSceneMsg(int index,string name,Action aciton)
    {
        this.SceneBuildIndex = index;
        this.SceneBuildName = name;
        this.OnSceneLoaded = aciton;
    }

    public LoadSceneMsg(int index, Action aciton)
    {
        this.SceneBuildIndex = index;
        this.SceneBuildName = null;
        this.OnSceneLoaded = aciton;
    }

    public LoadSceneMsg(string name, Action aciton)
    {
        this.SceneBuildIndex = -1;
        this.SceneBuildName = name;
        this.OnSceneLoaded = aciton;
    }

    public void Change(int index, string name, Action aciton)
    {
        this.SceneBuildIndex = index;
        this.SceneBuildName = name;
        this.OnSceneLoaded = aciton;
    }
}
