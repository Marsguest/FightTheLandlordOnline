using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.UI.Msg;
using Protocol.Code;
using Protocol.Constant;
using Protocol.Dto;
using UnityEngine;
using UnityEngine.UI;

public class ChatHandler : HandlerBase
{

    private ChatMsg msg = new ChatMsg();
    public override void OnReceive(int subCode, object value)
    {
        switch (subCode)
        {
            case ChatCode.SRES:
                {
                    ChatDto dto = value as ChatDto;
                    int userId = dto.UserId;
                    int chatType = dto.ChatType;
                    string text = Constant.GetChatText(chatType);
                    msg.Change(userId, chatType, text);
                    //显示文字
                    Dispatch(AreaCode.UI, UIEvent.PLAYER_CHAT, msg);
                    //播放声音
                    Dispatch(AreaCode.AUDIO, AudioEvent.PLAY_EFFECT_AUDIO, "Chat/Chat_" + chatType);
                    break;
                }
            default:
                break;
        }
    }

    void Awake()
    {

    }
    void Start()
    {

    }
    void Update()
    {

    }
}
