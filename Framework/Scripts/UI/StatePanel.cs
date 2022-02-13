using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.UI.Msg;
using Protocol.Dto;
using Protocol.Dto.Fight;
using UnityEngine;
using UnityEngine.UI;

public class StatePanel : UIBase
{
    protected Image imgIdentity;
    protected Text txtReady;
    protected Image imgChat;
    protected Text txtChat;
    //角色的数据
    protected UserDto userDto;

    protected virtual void Awake()
    {
        Bind(UIEvent.PLAYER_READY);
        Bind(UIEvent.PLAYER_HIDE_READY_TXT);
        Bind(UIEvent.PLAYER_LEAVE);
        Bind(UIEvent.PLAYER_ENTER);
        Bind(UIEvent.PLAYER_CHAT);
        Bind(UIEvent.CHANGE_IDENTITY);
    }

    public override void Execute(int eventCode, object message)
    {
        switch (eventCode)
        {
            case UIEvent.PLAYER_READY:
                {
                    if (userDto == null)
                        break;
                    int userId = (int)message;
                    //如果是自身角色 就展示已经准备
                    if (userDto.Id == userId)
                        readyState();
                    break;
                }
            //TODO 还有更多操作码需要关心
            case UIEvent.PLAYER_HIDE_READY_TXT:
                txtReady.gameObject.SetActive(false);
                break;
            case UIEvent.PLAYER_LEAVE:
                {
                    if (userDto == null)
                        break;
                    int userId = (int)message;
                    //如果是对应面板的角色 就 隐藏  left or right面板 状态面板下的所有游戏物体
                    if (userDto.Id == userId)
                        setPanelActive(false);
                    break;
                }
            case UIEvent.PLAYER_ENTER:
                {
                    if (userDto == null)
                        break;
                    int userId = (int)message;
                    //如果是对应面板的角色 就 显示  left or right面板 状态面板下的所有游戏物体
                    if (userDto.Id == userId)
                        setPanelActive(true);
                    break;
                }
            case UIEvent.PLAYER_CHAT:
                {
                    if (userDto == null)
                        break;
                    ChatMsg chatMsg = message as ChatMsg;
                    int userId = chatMsg.UserId;
                    int chatType = chatMsg.ChatType;
                    string chatText = chatMsg.Text;
                    if (userDto.Id == userId) 
                        showChat(chatText);
                    break;
                }
            case UIEvent.CHANGE_IDENTITY:
                {
                    if (userDto == null)
                        break;
                    if (userDto.Id == (int)message)
                        setIdentity(1);
                    break;
                }
            default:
                break;
        }
    }

    protected virtual void readyState()
    {
        txtReady.gameObject.SetActive(true);
    }
    protected virtual void Start()
    {
        imgIdentity = transform.Find("imgIdentity").GetComponent<Image>();
        txtReady = transform.Find("txtReady").GetComponent<Text>();
        imgChat = transform.Find("imgChat").GetComponent<Image>();
        txtChat = imgChat.transform.Find("txtChat").GetComponent<Text>();

        //默认状态
        txtReady.gameObject.SetActive(false);
        imgChat.gameObject.SetActive(false);

        setPanelActive(false);
        setIdentity(0);
    }
    /// <summary>
    /// 设置身份
    ///     0 农民 1 地主
    /// </summary>
    protected void setIdentity(int indentity)
    {
        string identityStr = indentity == 0 ? "Farmer" : "Landlord";
        imgIdentity.sprite = Resources.Load<Sprite>("Identity/" + identityStr);
    }
    #region 聊天动画
    /// <summary>
    /// 显示时间
    /// </summary>
    protected int showTime = 3;
    /// <summary>
    /// 计时器
    /// </summary>
    protected float timer = 0f;
    /// <summary>
    /// 是否显示
    /// </summary>
    private bool isShow = false;

    protected virtual void Update()
    {
        //这里只控制关闭显示即可 如何显示是外界调用的
        if (isShow == true)
        {
            timer += Time.deltaTime;
            if (timer >= showTime)
            {
                setChatActive(false);
                timer = 0f;
                isShow = false;
            }
        }
    }
    protected void setChatActive(bool active)
    {
        imgChat.gameObject.SetActive(active);
    }
    #endregion
    /// <summary>
    /// 外界调用的 显示聊天
    /// </summary>
    /// <param name="text"></param>
    protected void showChat(string text)
    {
        //设置文字
        txtChat.text = text;
        //显示动画
        setChatActive(true);
        isShow = true;
    }
}
