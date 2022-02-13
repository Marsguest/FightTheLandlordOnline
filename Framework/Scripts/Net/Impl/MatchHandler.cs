using System.Collections;
using System.Collections.Generic;
using Protocol.Code;
using Protocol.Dto;
using UnityEngine;
using UnityEngine.UI;

public class MatchHandler : HandlerBase
{
    public override void OnReceive(int subCode, object value)
    {
        switch (subCode)
        {
            case MatchCode.ENTER_MATCH_QUEUE_SRES:
                enterResponse(value as MatchRoomDto);
                break;
            case MatchCode.ENTER_MATCH_QUEUE_BRO:
                enterBro(value as UserDto);
                break;
            case MatchCode.LEAVE_MATCH_QUEUE_BRO:
                leaveBro((int)value);
                break;
            case MatchCode.READY_BRO:
                readyBro((int)value);
                break;
            case MatchCode.START_BRO:
                startBro();
                break;
            default:
                break;
        }
    }
    //PromptMsg promptMsg = new PromptMsg();
    /// <summary>
    /// 自己进入房间的服务器的反馈
    /// </summary>
    /// <param name="matchRoomDto"></param>
    private void enterResponse(MatchRoomDto _matchRoomDto)
    {
        //存储本地
        GameModel gModel = Models.GameMode;
        gModel.matchRoomDto = _matchRoomDto;
        MatchRoomDto matchRoomDto = gModel.matchRoomDto;
        int myUserId = gModel.userDto.Id;
        //重置一下玩家的位置信息
        //显示现在在的房间内的玩家
        resetPosition();

        //自身角色肯定是在的 更新自身的数据 这件事在这里做不了 还没切到下一个界面 无法捕获这个发出的Dispatch消息
        UserDto myUserDto = matchRoomDto.UIdUserDict[myUserId];
        //Dispatch(AreaCode.UI, UIEvent.SET_MY_PLAYER_DATA, myUserDto);

        //显示进入房间的按钮
        Dispatch(AreaCode.UI, UIEvent.SHOW_ENTER_ROOM_BUTTON, null) ;
    }
    /// <summary>
    /// 他人进入房间的广播
    /// </summary>
    /// <param name="newUser"></param>
    private void enterBro(UserDto newUser)
    {
        
        //更新房间数据
        Models.GameMode.matchRoomDto.AddUser(newUser);
        //重置一下玩家的位置
        //再次发送更新左右角色
        resetPosition();
        //给UI绑定userDto数据
        if (Models.GameMode.matchRoomDto.LeftId != -1)
        {
            UserDto leftUser = Models.GameMode.matchRoomDto.UIdUserDict[Models.GameMode.matchRoomDto.LeftId];
            Dispatch(AreaCode.UI, UIEvent.SET_LEFT_PLAYER_DATA, leftUser);
        }
        if (Models.GameMode.matchRoomDto.RightId != -1)
        {
            UserDto rightUser = Models.GameMode.matchRoomDto.UIdUserDict[Models.GameMode.matchRoomDto.RightId];
            Dispatch(AreaCode.UI, UIEvent.SET_RIGHT_PLAYER_DATA, rightUser);
        }
        


        //显示其他玩家进入后的玩家状态面板下的所有游戏物体
        Dispatch(AreaCode.UI, UIEvent.PLAYER_ENTER, newUser.Id);

        //给用户一个有新玩家进入的提示
        PromptMsg promptMsg = new PromptMsg("新玩家（"+newUser.Name+"）进入房间", Color.blue);
        Dispatch(AreaCode.UI, UIEvent.PROMPT_MSG, promptMsg);
    }
    /// <summary>
    /// 他人离开房间的广播
    /// </summary>
    /// <param name="leaveUserId"></param>
    private void leaveBro(int leaveUserId)
    {
        //离开房间 发消息 隐藏玩家的状态面板下的所有游戏物体
        Dispatch(AreaCode.UI, UIEvent.PLAYER_LEAVE, leaveUserId);

        resetPosition();

        //更新房间数据
        Models.GameMode.matchRoomDto.Leave(leaveUserId);
    }
    /// <summary>
    /// 准备的广播处理
    /// </summary>
    /// <param name="readyUserId"></param>
    private void readyBro(int readyUserId)
    {
        //更新房间数据
        Models.GameMode.matchRoomDto.Ready(readyUserId);
        //显示准备的玩家的已准备文本
        Dispatch(AreaCode.UI, UIEvent.PLAYER_READY, readyUserId);

        //发送消息 隐藏准备按钮 防止多次点击 点击准备是会收到服务器广播信息的
        Dispatch(AreaCode.UI, UIEvent.PLAYER_HIDE_READY_BUTTON, readyUserId);
    }
    /// <summary>
    /// 开始游戏的广播
    /// </summary>
    private void startBro()
    {
        PromptMsg promptMsg = new PromptMsg("所有玩家已准备，即将开始游戏", Color.green);
        Dispatch(AreaCode.UI, UIEvent.PROMPT_MSG, promptMsg);
        //开始游戏 隐藏已准备的文本显示
        Dispatch(AreaCode.UI, UIEvent.PLAYER_HIDE_READY_TXT, null);
    }
    /// <summary>
    /// 重置位置 更新左右角色的state面板
    /// </summary>
    private void resetPosition()
    {
        //重置一下玩家的位置信息
        Models.GameMode.matchRoomDto.ResetPostion(Models.GameMode.userDto.Id);

        //fix bug
        //if (Models.GameMode.matchRoomDto.LeftId != -1)
        //{
        //    UserDto leftUser = Models.GameMode.matchRoomDto.UIdUserDict[Models.GameMode.matchRoomDto.LeftId];
        //    Dispatch(AreaCode.UI, UIEvent.SET_LEFT_PLAYER_DATA, leftUser);
        //}
        //if (Models.GameMode.matchRoomDto.RightId != -1)
        //{
        //    UserDto rightUser = Models.GameMode.matchRoomDto.UIdUserDict[Models.GameMode.matchRoomDto.RightId];
        //    Dispatch(AreaCode.UI, UIEvent.SET_RIGHT_PLAYER_DATA, rightUser);
        //}
    }
}
