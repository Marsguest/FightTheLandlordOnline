using System;
using System.Collections;
using System.Collections.Generic;
using Protocol;
using Protocol.Code;
using UnityEngine;
using UnityEngine.UI;

public class DownPanel : UIBase
{
    void Awake()
    {
        Bind(UIEvent.CHANGE_MULTIPLE);
    }

    public override void Execute(int eventCode, object message)
    {
        base.Execute(eventCode, message);
        switch (eventCode)
        {
            case UIEvent.CHANGE_MULTIPLE:
                changeMultiple((int) message);
                break;
            default:
                break;
        }
    }

    private Text txtBean;
    private Text txtBeishu;
    private Image imgChoose;
    private Button btnChat;
    private Button[] btns;

    private SocketMsg socketMsg;

    private void Start()
    {
        initPanel();

        //默认设置
        imgChoose.gameObject.SetActive(false);
        refreshPanel(Models.GameMode.userDto.Bean);

    }
    private void initPanel()
    {
        txtBean = transform.Find("txtBean").GetComponent<Text>();
        txtBeishu = transform.Find("txtBeishu").GetComponent<Text>();
        btnChat = transform.Find("btnChat").GetComponent<Button>();
        imgChoose = transform.Find("imgChoose").GetComponent<Image>();
        btns = new Button[7];
        for (int i = 0; i < 7; i++)
        {
            btns[i] = imgChoose.transform.GetChild(i).GetComponent<Button>();
            //btns[i].onClick.AddListener(
            //    () => btnChatClick(Convert.ToInt32(gameObject.tag))
            //    ); ;
        }
        btns[0].onClick.AddListener(btnChat1Click);
        btns[1].onClick.AddListener(btnChat2Click);
        btns[2].onClick.AddListener(btnChat3Click);
        btns[3].onClick.AddListener(btnChat4Click);
        btns[4].onClick.AddListener(btnChat5Click);
        btns[5].onClick.AddListener(btnChat6Click);
        btns[6].onClick.AddListener(btnChat7Click);
        socketMsg = new SocketMsg();


        btnChat.onClick.AddListener(setChooseActive);
    }
    public override void OnDestory()
    {
        base.OnDestory();
        btnChat.onClick.RemoveAllListeners();
        btns[0].onClick.RemoveAllListeners();
        btns[1].onClick.RemoveAllListeners();
        btns[2].onClick.RemoveAllListeners();
        btns[3].onClick.RemoveAllListeners();
        btns[4].onClick.RemoveAllListeners();
        btns[5].onClick.RemoveAllListeners();
        btns[6].onClick.RemoveAllListeners();
    }
    /// <summary>
    /// 刷新自身面板的豆子
    /// </summary>
    /// <param name="beenCount"></param>
    private void refreshPanel(int _beenCount)
    {
        txtBean.text = " × "+ _beenCount;
    }
    /// <summary>
    /// 改变牌局的倍数
    /// </summary>
    private void changeMultiple(int _multiple)
    {
        txtBeishu.text = _multiple.ToString();
    }
    /// <summary>
    /// 设置选择对话内容面板的隐藏与显示
    /// </summary>
    private void setChooseActive()
    {
        bool active = imgChoose.gameObject.activeInHierarchy;
        imgChoose.gameObject.SetActive(!active);
    }
    /// <summary>
    /// 点击某一句聊天内容时调用
    /// </summary>
    /// <param name="charType"></param>
    private void btnChat1Click()
    {
        Debug.Log("点击了第一个按钮");
        socketMsg.Change(OpCode.CHAT, ChatCode.CREQ, 1);
        Dispatch(AreaCode.NET, 0, socketMsg);
    }
    private void btnChat2Click()
    {
        socketMsg.Change(OpCode.CHAT, ChatCode.CREQ, 2);
        Dispatch(AreaCode.NET, 0, socketMsg);
    }
    private void btnChat3Click()
    {
        socketMsg.Change(OpCode.CHAT, ChatCode.CREQ, 3);
        Dispatch(AreaCode.NET, 0, socketMsg);
    }
    private void btnChat4Click()
    {
        socketMsg.Change(OpCode.CHAT, ChatCode.CREQ, 4);
        Dispatch(AreaCode.NET, 0, socketMsg);
    }
    private void btnChat5Click()
    {
        socketMsg.Change(OpCode.CHAT, ChatCode.CREQ, 5);
        Dispatch(AreaCode.NET, 0, socketMsg);
    }
    private void btnChat6Click()
    {
        socketMsg.Change(OpCode.CHAT, ChatCode.CREQ, 6);
        Dispatch(AreaCode.NET, 0, socketMsg);
    }
    private void btnChat7Click()
    {
        socketMsg.Change(OpCode.CHAT, ChatCode.CREQ, 7);
        Dispatch(AreaCode.NET, 0, socketMsg);
    }
}
