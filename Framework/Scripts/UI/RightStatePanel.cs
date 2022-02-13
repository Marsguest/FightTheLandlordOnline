using System.Collections;
using System.Collections.Generic;
using Protocol.Dto;
using UnityEngine;
using UnityEngine.UI;

public class RightStatePanel : StatePanel
{
    protected override void Awake()
    {
        base.Awake();
        Bind(UIEvent.SET_RIGHT_PLAYER_DATA);
    }
    public override void Execute(int eventCode, object message)
    {
        base.Execute(eventCode, message);
        switch (eventCode)
        {
            case UIEvent.SET_RIGHT_PLAYER_DATA:
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
        GameModel gm = Models.GameMode;
        MatchRoomDto room = Models.GameMode.matchRoomDto;
        //如果不等于-1代表有角色
        if (gm.GetMatchRoomRightId() != -1)
        {
            UserDto rightUser = gm.GetUserDto(Models.GameMode.GetMatchRoomRightId());
            this.userDto = rightUser;
            setPanelActive(true);
            if (room.ReadyUIdList.Contains(Models.GameMode.GetMatchRoomRightId()))
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
