using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class Models 
{
    /// <summary>
    /// 游戏数据
    /// </summary>
    public static GameModel GameMode;

    static Models()
    {
        GameMode = new GameModel();
    }

}
