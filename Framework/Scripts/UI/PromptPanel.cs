using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PromptPanel : UIBase
{
    private void Awake()
    {
        Bind(UIEvent.PROMPT_MSG);
        //DontDestroyOnLoad(gameObject);
    }

    public override void Execute(int eventCode, object message)
    {
        switch (eventCode)
        {
            case UIEvent.PROMPT_MSG:
                PromptMsg msg = message as PromptMsg;
                promptMessage(msg.txt, msg.color) ;
                break;
            default:
                break;
        }
    }
    private Text txt;
    private CanvasGroup cg;

    [SerializeField]
    [Range(0,3)] //控制滑动最大最小值
    private float showTime = 1f;

    private float timer = 0f;
    void Start()
    {
        txt = transform.Find("Text").GetComponent<Text>();
        cg = transform.Find("Text").GetComponent<CanvasGroup>();

        cg.alpha = 0;
    }
    /// <summary>
    /// 提示消息
    /// </summary>
    private void promptMessage(string text,Color color)
    {
        txt.text = text;
        txt.color = color;
        cg.alpha = 0;
        timer = 0;
        //做动画显示
        StartCoroutine(promptAnim());
        
    }
    /// <summary>
    /// 用来做动画
    /// </summary>
    /// <returns></returns>
    IEnumerator promptAnim()
    {
        while (cg.alpha < 1f)
        {
            cg.alpha += Time.deltaTime * 2;
            yield return new WaitForEndOfFrame(); //等待一帧
        }
        //显示1秒
        while (timer < showTime)
        {
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame(); //等待一帧
        }
        while (cg.alpha > 0f)
        {
            cg.alpha -= Time.deltaTime * 2;
            yield return new WaitForEndOfFrame(); //等待一帧
        }
    }
}
