using System.Collections;
using System.Collections.Generic;
using Protocol;
using Protocol.Code;
using UnityEngine;
using UnityEngine.UI;

public class CreatePanel : UIBase
{
    private void Awake()
    {
        Bind(UIEvent.CREATE_PANEL_ACTIVE);
    }

    public override void Execute(int eventCode, object message)
    {
        switch (eventCode)
        {
            case UIEvent.CREATE_PANEL_ACTIVE:
                gameObject.SetActive((bool)message);
                break;
            default:
                break;
        }
    }
    private InputField inputName;
    private Button btnCreate;

    private PromptMsg promptMsg;
    private SocketMsg socketMsg;
  
    void Start()
    {
        inputName = transform.Find("inputName").GetComponent<InputField>();
        btnCreate = transform.Find("btnCreate").GetComponent<Button>();

        btnCreate.onClick.AddListener(btnCreateClick);

        promptMsg = new PromptMsg();
        socketMsg = new SocketMsg();
        //默认不显示
        setPanelActive(false);
    }

    public override void OnDestory()
    {
        base.OnDestory();

        btnCreate.onClick.RemoveAllListeners();
    }
    private void btnCreateClick()
    {
        if (string.IsNullOrEmpty(inputName.text))
        {
            //非法输入
            promptMsg.Change("请正确输入您的名称", Color.red);
            Dispatch(AreaCode.UI, UIEvent.PROMPT_MSG, promptMsg);
            return;
        }
        //进行一些其他二点判断 如 长度 符号..
        //向服务器发送一个创建的请求
        socketMsg.Change(OpCode.USER,UserCode.CREATE_CREQ,inputName.text);
        Dispatch(AreaCode.NET, 0, socketMsg);
    }
}
