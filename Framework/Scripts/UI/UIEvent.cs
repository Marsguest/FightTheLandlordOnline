using System;

public class UIEvent 
{
    public const int START_PANEL_ACTIVE = 0;
    public const int REGIST_PANEL_ACTIVE = 1;

    public const int REFRESH_INFO_PANEL = 2;//刷新信息面板 参数：目前未知 服务器定的
    public const int SHOW_ENTER_ROOM_BUTTON = 3; //显示进入房间按钮

    public const int CREATE_PANEL_ACTIVE = 4;

    public const int SET_TABLE_CARDS = 5;//设置底牌

    public const int SET_LEFT_PLAYER_DATA = 6;//设置左边角色的数据
    public const int SET_RIGHT_PLAYER_DATA = 13;//设置右边角色的数据

    public const int PLAYER_READY = 7;//角色准备 就是让隐藏的ready txt显示出来
    public const int PLAYER_ENTER = 8;//角色进入
    public const int PLAYER_LEAVE = 9;//角色离开
    public const int PLAYER_CHAT = 10;//角色聊天
    public const int CHANGE_IDENTITY = 11;//角色更改身份
    public const int PLAYER_HIDE_READY_TXT = 12;//开始游戏 就是让显示的ready txt隐藏掉

    public const int SHOW_GRAB_BUTTON = 14; //显示抢地主和不抢地主的按钮
    public const int SHOW_DEAL_BUTTON = 15; //显示出牌按钮
    //public const int SET_MY_PLAYER_DATA = 16; //设置自己角色的数据

    public const int PLAYER_HIDE_READY_BUTTON = 17; //玩家隐藏自己的准备按钮

    public const int CHANGE_MULTIPLE = 18;//更改倍数

    public const int SHOW_OVER_PANEL = 19;//显示结束面板




    //...


    public const int PROMPT_MSG = int.MaxValue;
}
