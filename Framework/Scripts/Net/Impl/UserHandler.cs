using System.Collections;
using System.Collections.Generic;
using Protocol;
using Protocol.Code;
using Protocol.Dto;
using Protocol.Protocol;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 角色的网络消息处理类
/// </summary>
public class UserHandler : HandlerBase
{

    public override void OnReceive(int subCode, object value)
    {
        switch (subCode)
        {
            case UserCode.CREATE_SRES:
                createResponse((int)value);
                break;
            case UserCode.GET_INFO_SRES:
                getInfoResponse(value as UserDto);
                break;
            case UserCode.ONLINE_SRES:
                onlineResponse((int)value);
                break;
            default:
                break;
        }
    }

    private SocketMsg socketMsg = new SocketMsg();
    /// <summary>
    /// 获取信息的响应
    /// </summary>
    /// <param name="user"></param>
    private void getInfoResponse(UserDto user)
    {
        if (user == null)
        {
            //没有角色
            //显示创建面板
            Dispatch(AreaCode.UI, UIEvent.CREATE_PANEL_ACTIVE, true);
        }
        else
        {
            //有角色
            //隐藏创建名版 这里默认不显示了
            //Dispatch(AreaCode.UI, UIEvent.CREATE_PANEL_ACTIVE, false);
            //角色上线 已经交由服务器来做
            //socketMsg.Change(OpCode.USER, UserCode.ONLINE_CREQ, null);
            //Dispatch(AreaCode.NET, 0, socketMsg);

            //保存服务器发来的角色数据
            Models.GameMode.userDto = user;

            //更新一下本地的显示
            Dispatch(AreaCode.UI, UIEvent.REFRESH_INFO_PANEL, user);
        }
    }
    private PromptMsg promptMsg = new PromptMsg();
    /// <summary>
    /// 上线的响应
    /// </summary>
    /// <param name="result"></param>
    private void onlineResponse(int result)
    {
        //Debug.LogError("online==============="+UserProtocol.Instance.codeMsgDict[result]);
        if (result == UserProtocol.ONLINE_SUCCESS)
        {
            //上线成功
            //string msg = UserProtocol.Instance.codeMsgDict[UserProtocol.ONLINE_SUCCESS];
            //promptMsg.Change(msg, Color.green);
            //Dispatch(AreaCode.UI, UIEvent.PROMPT_MSG, promptMsg);
            Debug.Log("上线成功");
        }
        else if (result == UserProtocol.CREATE_NOT_LAW)
        {
            //没有角色不能上线
            //string msg = UserProtocol.Instance.codeMsgDict[UserProtocol.CREATE_NOT_LAW];
            //promptMsg.Change(msg, Color.red);
            //Dispatch(AreaCode.UI, UIEvent.PROMPT_MSG, promptMsg);
            Debug.LogError("客户端非法登录");
        }
        else if(result == UserProtocol.ONLINE_HAS_NO_USER)
        {
            //没有角色不能上线
            //string msg = UserProtocol.Instance.codeMsgDict[UserProtocol.ONLINE_HAS_NO_USER];
            //promptMsg.Change(msg, Color.red);
            //Dispatch(AreaCode.UI, UIEvent.PROMPT_MSG, promptMsg);
            Debug.LogError("非法操作 没有角色 不能上线");
        }

    }
    /// <summary>
    /// 创建角色的响应
    /// </summary>
    /// <param name="result"></param>
    private void createResponse(int result)
    {
        //Debug.Log("create===========" + UserProtocol.Instance.codeMsgDict[result]);
        if (result == UserProtocol.CREATE_NOT_LAW)
        {
            //客户端非法登录
            //string msg = UserProtocol.Instance.codeMsgDict[UserProtocol.CREATE_NOT_LAW];
            //promptMsg.Change(msg, Color.red);
           // Dispatch(AreaCode.UI, UIEvent.PROMPT_MSG, promptMsg);
            Debug.LogError("客户端非法登录");
        }
        else if (result == UserProtocol.CREATE_ALREADY_HAS_USER)
        {
            //已经有角色 重复创建
            //string msg = UserProtocol.Instance.codeMsgDict[UserProtocol.CREATE_ALREADY_HAS_USER];
            //promptMsg.Change(msg, Color.red);
            //Dispatch(AreaCode.UI, UIEvent.PROMPT_MSG, promptMsg);
            Debug.LogError("已经有角色 重复创建");
        } 
        else if(result == UserProtocol.CREATE_SUCCESS)
        {
            //创建成功
            //string msg = "创建成功";
            //Debug.LogError("createMsg" + msg);
            //promptMsg.Change(msg, Color.green);
            //Dispatch(AreaCode.UI, UIEvent.PROMPT_MSG, promptMsg);
            Debug.Log("创建成功");
            //隐藏创建面板
            Dispatch(AreaCode.UI, UIEvent.CREATE_PANEL_ACTIVE, false);
            //获取角色信息 目的是将角色信息保存到本地
            socketMsg.Change(OpCode.USER, UserCode.GET_INFO_CREQ, null);
            Dispatch(AreaCode.NET, 0, socketMsg);
        }
    }
}
