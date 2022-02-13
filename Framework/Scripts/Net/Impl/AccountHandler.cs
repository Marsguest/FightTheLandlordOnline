using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Protocol;
using Protocol.Code;
using Protocol.Protocol;
using UnityEngine;
/// <summary>
/// 账号的网络消息处理类 实际上是服务器发送来消息后做的一系列处理
/// </summary>
public class AccountHandler : HandlerBase
{
    public override void OnReceive(int subCode, object value)
    {
        switch (subCode)
        {
            case AccountCode.LOGIN_SRES:
                loginResponse((int)value);
                break;
            case AccountCode.REGIST_SRES:
                registResponse((int)value);
                break;
            default:
                break;
        }
    }
    private PromptMsg promptMsg = new PromptMsg();

    private void loginResponse(int result)
    {
        string msg = AccountProtocol.Instance.codeMsgDict[result];
        //Debug.LogError("msg:" + msg);
        if (result == AccountProtocol.LOGIN_SUCCESS)
        {
            //跳转场景
            promptMsg.Change(msg, Color.green);
            Dispatch(AreaCode.UI, UIEvent.PROMPT_MSG, promptMsg);
            //LoadSceneMsg loadSceneMsg = new LoadSceneMsg(1, 
            //    delegate ()//匿名委托
            //    {
            //        //TODO 向服务器获取信息
            //        Debug.Log("加载完成！");
            //    });
            LoadSceneMsg loadSceneMsg = new LoadSceneMsg(1,
                ()=> 
                {
                    //向服务器获取信息
                    Debug.Log("加载完成！");
                    SocketMsg socketNsg = new SocketMsg(OpCode.USER, UserCode.GET_INFO_CREQ,null);
                    Dispatch(AreaCode.NET, 0, socketNsg);
                });
            Dispatch(AreaCode.SCENE, SceneEvent.LOAD_SCENE, loadSceneMsg);
            return;
        }

        promptMsg.Change(msg, Color.red);
        Dispatch(AreaCode.UI, UIEvent.PROMPT_MSG, promptMsg);
    }
    /// <summary>
    /// 注册响应
    /// </summary>
    /// <param name="result"></param>
    private void registResponse(int result)
    {
        string msg = AccountProtocol.Instance.codeMsgDict[result];
        if (result == AccountProtocol.REGIST_SUCCESS)
        {
            //TODO 跳转场景
            promptMsg.Change(msg, Color.green);
            Dispatch(AreaCode.UI, UIEvent.PROMPT_MSG, promptMsg);
            return;
        }

        promptMsg.Change(msg, Color.red);
        Dispatch(AreaCode.UI, UIEvent.PROMPT_MSG, promptMsg);
    }
}
