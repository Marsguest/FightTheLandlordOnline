using System.Collections;
using System.Collections.Generic;
using Protocol.Dto;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 游戏数据的存储类
/// </summary>
public class GameModel 
{
    public UserDto userDto { get; set; }//登录用户的数据

    public MatchRoomDto matchRoomDto { get; set; }//加入的房间的数据模型

    public UserDto GetUserDto(int userId)
    {
        return matchRoomDto.UIdUserDict[userId];
    }

    public int GetMatchRoomRightId()
    {
        return matchRoomDto.RightId;
    }
    public int GetMatchRoomLeftId()
    {
        return matchRoomDto.LeftId;
    }

}
