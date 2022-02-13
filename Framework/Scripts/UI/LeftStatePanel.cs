using System.Collections;
using System.Collections.Generic;
using Protocol.Dto;
using UnityEngine;
using UnityEngine.UI;

public class LeftStatePanel : StatePanel
{
    protected override void Awake()
    {
        base.Awake();
        Bind(UIEvent.SET_LEFT_PLAYER_DATA);
    }
    public override void Execute(int eventCode, object message)
    {
        base.Execute(eventCode, message);
        switch (eventCode)
        {
            case UIEvent.SET_LEFT_PLAYER_DATA:
                this.userDto = message as UserDto;
                //setPanelActive(true);
                break;
            
            default:
                break;
        }
    }
    protected override void Start()
    {
        base.Start();

        //fix bug
        //如果不等于-1代表有角色
        MatchRoomDto room = Models.GameMode.matchRoomDto;
        int leftId = room.LeftId;

        if (Models.GameMode.GetMatchRoomLeftId() != -1)
        {
            UserDto leftUser = Models.GameMode.GetUserDto(Models.GameMode.GetMatchRoomLeftId());
            this.userDto = leftUser;
            setPanelActive(true);
            if (room.ReadyUIdList.Contains(leftId))
            {
                readyState();
            }
            
        }
        else
        {
            setPanelActive(false);
        }
    }
}
