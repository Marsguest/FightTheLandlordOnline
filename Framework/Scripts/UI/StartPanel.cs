using System.Collections;
using System.Collections.Generic;
using Protocol;
using Protocol.Code;
using Protocol.Dto;
using UnityEngine;
using UnityEngine.UI;

public class StartPanel : UIBase
{
    private Button btnLogin;
    private Button btnClose;
    private InputField inputAccount;
    private InputField inputPassword;

    private PromptMsg promptMsg;
    private SocketMsg socketMsg;
    void Awake()
    {
        Bind(UIEvent.START_PANEL_ACTIVE);
    }

    public override void Execute(int eventCode, object message)
    {
        switch (eventCode)
        {
            case UIEvent.START_PANEL_ACTIVE:
                setPanelActive((bool)message);
                break;
            default:
                break;
        }
    }

    void Start()
    {
        btnLogin = transform.Find("btnLogin").GetComponent<Button>();
        btnClose = transform.Find("btnClose").GetComponent<Button>();
        inputAccount = transform.Find("inputAccount").GetComponent<InputField>();
        inputPassword = transform.Find("inputPassword").GetComponent<InputField>();

        btnClose.onClick.AddListener(btnCloseClick);
        btnLogin.onClick.AddListener(btnLoginClick);

        promptMsg = new PromptMsg();
        socketMsg = new SocketMsg();

        setPanelActive(false);
    }

    public override void OnDestory()
    {
        base.OnDestory();
        btnClose.onClick.RemoveListener(btnCloseClick);
        btnLogin.onClick.RemoveListener(btnLoginClick);
    }
    private void btnLoginClick()
    {
        //连接服务器操作
        if (string.IsNullOrEmpty(inputAccount.text))
        {
            promptMsg.Change("登录的用户名不能为空",Color.red);
            Dispatch(AreaCode.UI, UIEvent.PROMPT_MSG, promptMsg);
            return;
        }
        
        if (string.IsNullOrEmpty(inputPassword.text)
            ||inputPassword.text.Length < 4
            ||inputPassword.text.Length > 16)
        {
            promptMsg.Change("登录的密码不合法", Color.red);
            Dispatch(AreaCode.UI, UIEvent.PROMPT_MSG, promptMsg);
            return;
        }
           
        AccountDto dto = new AccountDto(inputAccount.text, inputPassword.text);
        socketMsg.Change(OpCode.ACCOUNT, AccountCode.LOGIN_CREQ, dto);

        Dispatch(AreaCode.NET, 0, socketMsg);
    }

    private void btnCloseClick()
    {
        setPanelActive(false);
    }
}
