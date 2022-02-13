using System.Collections;
using System.Collections.Generic;
using Protocol.Code;
using Protocol.Constant;
using Protocol.Dto.Fight;
using UnityEngine;
using UnityEngine.UI;

public class FightHandler : HandlerBase
{
    public override void OnReceive(int subCode, object value)
    {
        switch (subCode)
        {
            case FightCode.GET_CARD_SRES:
                //Debug.LogWarning("GET_CARD_SRES 消息接收成功");
                getCards(value as List<CardDto>);
                break;
            case FightCode.TURN_GRAB_BRO:
                turnGrabBro((int) value);
                break;
            case FightCode.GRAB_LANDLORD_BRO:
                grabLandlordBro(value as GrabDto);
                break;
            case FightCode.TURN_DEAL_BRO:
                turnDealBro((int)value);
                break;
            case FightCode.DEAL_BRO:
                dealBro(value as DealDto);
                break;
            case FightCode.DEAL_SRES:
                dealResponse((int) value);
                break;
            case FightCode.PASS_SRES:
                passResponse((int)value);
                break;
            case FightCode.OVER_BRO:
                overBro(value as OverDto);
                break;
            default:
                break;
        }
    }
    /// <summary>
    /// 结束广播
    /// </summary>
    /// <param name="dto"></param>
    private void overBro(OverDto dto)
    {
        //播放结束音效
        if (dto.WinUIdList.Contains(Models.GameMode.userDto.Id))
        {
            Dispatch(AreaCode.AUDIO, AudioEvent.PLAY_EFFECT_AUDIO, "Fight/MusicEx_Win");
        }
        else
        {
            Dispatch(AreaCode.AUDIO, AudioEvent.PLAY_EFFECT_AUDIO, "Fight/MusicEx_Lose");
        }
        //显示结束面板
        Dispatch(AreaCode.UI, UIEvent.SHOW_OVER_PANEL, dto);
    }

    /// <summary>
    /// 不出的响应
    /// </summary>
    /// <param name="result">-1不能不出 0可以不出</param>
    private void passResponse(int result)
    {
        if (result == -1)
        {
            //玩家不能不出
            PromptMsg promptMsg = new PromptMsg("该回合你不能不出", Color.red);
            Dispatch(AreaCode.UI, UIEvent.PROMPT_MSG, promptMsg);
            
        }
        else if (result == 0)
        {
            //隐藏出牌按钮
            Dispatch(AreaCode.UI, UIEvent.SHOW_DEAL_BUTTON, false);
        }
    }
    /// <summary>
    /// 出牌响应 个人
    /// </summary>
    private void dealResponse(int result)
    {
        if (result == -1)
        {
            //玩家管不上上一个玩家出的牌
            PromptMsg promptMsg = new PromptMsg("玩家管不上上一个玩家出的牌", Color.red);
            Dispatch(AreaCode.UI, UIEvent.PROMPT_MSG, promptMsg);
        }
        else
        {
            //走到这里代表牌已经出去了 能管上
            //隐藏出牌按钮
            Dispatch(AreaCode.UI, UIEvent.SHOW_DEAL_BUTTON, false);
        }
    }
    /// <summary>
    /// 同步出牌
    /// </summary>
    private void dealBro(DealDto dealDto)
    {
        //移除出完的手牌
        if (dealDto.UserId == Models.GameMode.GetMatchRoomLeftId())
        {
            Dispatch(AreaCode.CHARACTER, CharacterEvent.REMOVE_LEFT_CARD, dealDto.RemainCardList);
        }
        else if (dealDto.UserId == Models.GameMode.GetMatchRoomRightId())
        {
            Dispatch(AreaCode.CHARACTER, CharacterEvent.REMOVE_RIGHT_CARD, dealDto.RemainCardList);
        }
        else if (dealDto.UserId == Models.GameMode.userDto.Id)
        {
            Dispatch(AreaCode.CHARACTER, CharacterEvent.REMOVE_MY_CARD, dealDto.RemainCardList);
        }
        //显示到桌面上
        Dispatch(AreaCode.CHARACTER, CharacterEvent.UODATE_SHOE_DESK, dealDto.SelectCardList);
        //播放出牌音效
        playDealAudio(dealDto.Type, dealDto.Weight);

    }
    /// <summary>
    /// 播放出牌音效
    /// </summary>
    private void playDealAudio(int cardType,int weight)
    {
        string audioName = "Fight/";
        switch (cardType)
        {
            case CardType.SINGLE:
                audioName += "Woman_" + weight;
                break;
            case CardType.DOUBLE:
                audioName += "Woman_dui" + weight / 2;
                break;
            case CardType.STRAIGHT:
                audioName += "Woman_shunzi";
                break;
            case CardType.DOUBLE_STRAIGHT:
                audioName += "Woman_liandui";
                break;
            case CardType.TRIPLE_STRAIGHT:
                audioName += "Woman_feiji";
                break;
            case CardType.THREE:
                audioName += "Woman_tuple" + weight / 3;
                break;
            case CardType.THREE_ONE:
                audioName += "Woman_sandaiyi";
                break;
            case CardType.THREE_TWO:
                audioName += "Woman_sandaiyidui";
                break;
            case CardType.BOOM:
                audioName += "Woman_zhadan";
                break;
            case CardType.JOKER_BOOM:
                audioName += "Woman_wangzha";
                break;

            default:
                break;
        }
        Dispatch(AreaCode.AUDIO, AudioEvent.PLAY_EFFECT_AUDIO, audioName);
    }
    /// <summary>
    /// 转换出牌
    /// </summary>
    /// <param name="usreId">出牌者id</param>
    private void turnDealBro(int userId)
    {
        //如果是该自己出牌 则显示两个按钮
        if (userId == Models.GameMode.userDto.Id)
        {
            Dispatch(AreaCode.UI, UIEvent.SHOW_DEAL_BUTTON, true);
        }
    }
    /// <summary>
    /// 抢地主成功的处理
    /// </summary>
    private void grabLandlordBro(GrabDto grabDto)
    {
        //更改ui的身份显示
        Dispatch(AreaCode.UI, UIEvent.CHANGE_IDENTITY, grabDto.UserId);
        //播放抢地主的音效
        Dispatch(AreaCode.AUDIO, AudioEvent.PLAY_EFFECT_AUDIO, "Fight/Woman_Order");
        //显示三张底牌
        Dispatch(AreaCode.UI, UIEvent.SET_TABLE_CARDS, grabDto.TableCardList);
        //给对应的地主玩家添加手牌显示出来
        if (grabDto.UserId == Models.GameMode.GetMatchRoomLeftId())
        {
            Dispatch(AreaCode.CHARACTER, CharacterEvent.ADD_LEFT_CARD, null);
        }
        else if (grabDto.UserId == Models.GameMode.GetMatchRoomRightId())
        {
            Dispatch(AreaCode.CHARACTER, CharacterEvent.ADD_RIGHT_CARD, null);
        }
        else if (grabDto.UserId == Models.GameMode.userDto.Id)
        {
            Dispatch(AreaCode.CHARACTER, CharacterEvent.ADD_MY_CARD, grabDto);
        }


    }
    /// <summary>
    /// 是否是第一个玩家抢地主 而不是 因为别的玩家不叫地主而转换的
    /// </summary>
    private bool isFirst = true;

    /// <summary>
    /// 转换抢地主
    /// </summary>
    /// <param name="userId"></param>
    private void turnGrabBro(int userId)
    {
        if (isFirst == true)
        {
            isFirst = false;
        }
        else
        {
            //播放声音
            Dispatch(AreaCode.AUDIO, AudioEvent.PLAY_EFFECT_AUDIO, "Fight/Woman_NoOrder");
        }
        //如果是自身就显示两个按钮(抢地主 不抢地主)
        if (userId == Models.GameMode.userDto.Id)
        {
            Dispatch(AreaCode.UI, UIEvent.SHOW_GRAB_BUTTON, true);
        }
        else
        {
            Dispatch(AreaCode.UI, UIEvent.SHOW_GRAB_BUTTON, false);
        }
    }
    /// <summary>
    /// 获取到卡牌的处理
    /// </summary>
    /// <param name="cardList"></param>
    private void getCards(List<CardDto> cardList)
    {
        //给自己玩家创建牌的对象
        Dispatch(AreaCode.CHARACTER, CharacterEvent.INIT_MY_CARD, cardList);
        Dispatch(AreaCode.CHARACTER, CharacterEvent.INIT_LEFT_CARD, null);
        Dispatch(AreaCode.CHARACTER, CharacterEvent.INIT_RIGHT_CARD, null);
        //改变倍数为1
        Dispatch(AreaCode.UI, UIEvent.CHANGE_MULTIPLE, 1);

    }
}
