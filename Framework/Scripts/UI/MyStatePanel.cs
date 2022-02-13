using System.Collections;
using System.Collections.Generic;
using Protocol;
using Protocol.Code;
using Protocol.Dto;
using UnityEngine;
using UnityEngine.UI;

public class MyStatePanel : StatePanel
{
    protected override void Awake()
    {
        base.Awake();

        Bind(UIEvent.SHOW_GRAB_BUTTON,
            UIEvent.SHOW_DEAL_BUTTON,
            UIEvent.PLAYER_HIDE_READY_BUTTON);
    }
    public override void Execute(int eventCode, object message)
    {
        base.Execute(eventCode, message);
        switch (eventCode)
        {
            case UIEvent.SHOW_GRAB_BUTTON:
                {
                    bool active = (bool)message;
                    btnGrab.gameObject.SetActive(active);
                    btnNGrab.gameObject.SetActive(active);
                    break;
                }
                
            case UIEvent.SHOW_DEAL_BUTTON:
                {
                    bool active = (bool)message;
                    btnDeal.gameObject.SetActive(active);
                    btnNDeal.gameObject.SetActive(active);
                    break;
                }
            //case UIEvent.SET_MY_PLAYER_DATA:
            //    {
            //        this.userDto = message as UserDto;
            //        break;
            //    }
            case UIEvent.PLAYER_HIDE_READY_BUTTON:
                {
                    int readyUserId = (int)message;
                    if(userDto.Id == readyUserId)
                        btnReady.gameObject.SetActive(false);
                    break;
                }
            default:
                break;
        }
    }

    protected override void readyState()
    {
        base.readyState();
        btnReady.gameObject.SetActive(false);
    }

    private Button btnDeal;
    private Button btnNDeal;
    private Button btnGrab;
    private Button btnNGrab;
    private Button btnReady;

    private SocketMsg socketMsg;

    protected override void Start()
    {
        base.Start();
        //显示自身可见
        setPanelActive(true);
        //绑定自己的UI数据
        userDto = Models.GameMode.userDto;

        btnDeal = transform.Find("btnDeal").GetComponent<Button>();
        btnNDeal = transform.Find("btnNDeal").GetComponent<Button>();
        btnGrab = transform.Find("btnGrab").GetComponent<Button>();
        btnNGrab = transform.Find("btnNGrab").GetComponent<Button>();
        btnReady = transform.Find("btnReady").GetComponent<Button>();

        socketMsg = new SocketMsg();

        //默认状态
        btnDeal.gameObject.SetActive(false);
        btnNDeal.gameObject.SetActive(false);
        btnGrab.gameObject.SetActive(false);
        btnNGrab.gameObject.SetActive(false);

        btnDeal.onClick.AddListener(btnDealClick);
        btnNDeal.onClick.AddListener(btnNDealClick);

        btnGrab.onClick.AddListener(() => btnGrabClick(true));
        btnNGrab.onClick.AddListener(() => btnGrabClick(false));
        btnReady.onClick.AddListener(btnReadyClick);
    }

    public override void OnDestory()
    {
        base.OnDestory();
        btnDeal.onClick.RemoveAllListeners();
        btnNDeal.onClick.RemoveAllListeners();
        btnGrab.onClick.RemoveAllListeners();
        btnNGrab.onClick.RemoveAllListeners();
        btnReady.onClick.RemoveAllListeners();
    }

    private void btnDealClick()
    {
       // Debug.Log("点击了出牌按钮");
        //通知角色模块出牌 由角色模块那边来获取选中的牌
        Dispatch(AreaCode.CHARACTER, CharacterEvent.DEAL_CARD, null);

        //隐藏掉两个按钮 TODO这里不太好 直接隐藏可能会出问题
        //btnDeal.gameObject.SetActive(false);
        //btnNDeal.gameObject.SetActive(false);
    }
    private void btnNDealClick()
    {
        //向服务器发送不出牌
        socketMsg.Change(OpCode.FIGHT, FightCode.PASS_CREQ, true);
        Dispatch(AreaCode.NET, 0, socketMsg);

        //隐藏掉两个按钮 TODO这里不太好 直接隐藏可能会出问题（如果网络出现错误的话）
        //btnDeal.gameObject.SetActive(false);
        //btnNDeal.gameObject.SetActive(false);
    }

    private void btnGrabClick(bool result)
    {
        if(result == true)
        {
            //抢地主
            socketMsg.Change(OpCode.FIGHT, FightCode.GRAB_LANDLORD_CREQ, true);
            Dispatch(AreaCode.NET, 0, socketMsg);
        }
        else
        {
            //不抢地主
            socketMsg.Change(OpCode.FIGHT, FightCode.GRAB_LANDLORD_CREQ, false);
            Dispatch(AreaCode.NET, 0, socketMsg);
        }
        //点击之后隐藏两个按钮
        btnGrab.gameObject.SetActive(false);
        btnNGrab.gameObject.SetActive(false);
    }

    private void btnReadyClick()
    {
        //向服务器发送准备
        socketMsg.Change(OpCode.MATCH, MatchCode.READY_CREQ, null);
        Dispatch(AreaCode.NET, 0, socketMsg);
    }

}
