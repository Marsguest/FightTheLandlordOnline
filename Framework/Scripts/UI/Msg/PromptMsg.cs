using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PromptMsg
{
    public string txt;
    public Color color;

    public PromptMsg()
    {

    }
    public PromptMsg(string _txt,Color _color)
    {
        this.txt = _txt;
        this.color = _color;
    }
    /// <summary>
    /// 避免了重复new对象
    /// </summary>
    /// <param name="_txt"></param>
    /// <param name="_color"></param>
    public void Change(string _txt, Color _color)
    {
        this.txt = _txt;
        this.color = _color;
    }
}
