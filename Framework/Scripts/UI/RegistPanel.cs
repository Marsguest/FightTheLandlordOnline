using System.Collections;
using System.Collections.Generic;
using Protocol;
using Protocol.Code;
using Protocol.Dto;
using UnityEngine;
using UnityEngine.UI;

public class RegistPanel : UIBase
{
    private Button btnRegist;
    private Button btnClose;
    private InputField inputAccount;
    private InputField inputPassword;
    private InputField inputRepeat;

    private PromptMsg promptMsg;
    private SocketMsg socketMsg;
    void Awake()
    {
        Bind(UIEvent.REGIST_PANEL_ACTIVE);
    }

    public override void Execute(int eventCode, object message)
    {
        switch (eventCode)
        {
            case UIEvent.REGIST_PANEL_ACTIVE:
                setPanelActive((bool)message);
                break;
            default:
                break;
        }
    }

    void Start()
    {
        btnRegist = transform.Find("btnRegist").GetComponent<Button>();
        btnClose = transform.Find("btnClose").GetComponent<Button>();
        inputAccount = transform.Find("inputAccount").GetComponent<InputField>();
        inputPassword = transform.Find("inputPassword").GetComponent<InputField>();
        inputRepeat = transform.Find("inputRepeat").GetComponent<InputField>();

        btnClose.onClick.AddListener(btnCloseClick);
        btnRegist.onClick.AddListener(btnRegistClick);

        promptMsg = new PromptMsg();
        socketMsg = new SocketMsg();

        setPanelActive(false);
    }

    public override void OnDestory()
    {
        base.OnDestory();
        btnClose.onClick.RemoveListener(btnCloseClick);
        btnRegist.onClick.RemoveListener(btnRegistClick);
    }

    AccountDto dto = new AccountDto();
    /// <summary>
    /// 注册按钮的点击事件处理
    /// </summary>
    private void btnRegistClick()
    {
        //连接服务器操作
        //TODO
        if (string.IsNullOrEmpty(inputAccount.text))
        {
            promptMsg.Change("注册的用户名不能为空", Color.red);
            Dispatch(AreaCode.UI, UIEvent.PROMPT_MSG, promptMsg);
            return;
        }
        if (string.IsNullOrEmpty(inputPassword.text)
            || inputPassword.text.Length < 4
            || inputPassword.text.Length > 16)
        {
            promptMsg.Change("注册的密码不合法", Color.red);
            Dispatch(AreaCode.UI, UIEvent.PROMPT_MSG, promptMsg);
            return;
        }
        if (inputPassword.text!=inputRepeat.text)
        {
            promptMsg.Change("请确保两次输入的密码一致", Color.red);
            Dispatch(AreaCode.UI, UIEvent.PROMPT_MSG, promptMsg);
            return;
        }

        dto.Account = inputAccount.text;
        dto.Password = inputPassword.text;
        //SocketMsg socketMsg = new SocketMsg(OpCode.ACCOUNT, AccountCode.REGIST_CREQ, dto);
        socketMsg.Change(OpCode.ACCOUNT, AccountCode.REGIST_CREQ, dto);

        Dispatch(AreaCode.NET, 0, socketMsg);

    }

    private void btnCloseClick()
    {
        setPanelActive(false);
    }

}
