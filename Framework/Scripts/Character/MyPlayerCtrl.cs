using Protocol;
using Protocol.Code;
using Protocol.Dto.Fight;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyPlayerCtrl : CharacterBase
{
    private void Awake()
    {
        Bind(CharacterEvent.INIT_MY_CARD,
            CharacterEvent.ADD_MY_CARD,
            CharacterEvent.DEAL_CARD,
            CharacterEvent.REMOVE_MY_CARD);
    }

    public override void Execute(int eventCode, object message)
    {
        switch (eventCode)
        {
            case CharacterEvent.INIT_MY_CARD:
                StartCoroutine(initCardList(message as List<CardDto>));
                break;
            case CharacterEvent.ADD_MY_CARD:
                addTableCard(message as GrabDto);
                break;
            case CharacterEvent.DEAL_CARD:
                dealSelectCard();
                break;
            case CharacterEvent.REMOVE_MY_CARD:
                removeCard(message as List<CardDto>);
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 自身管理的卡牌列表
    /// </summary>
    private List<CardCtrl> cardCtrlList;
    /// <summary>
    /// 卡牌的父物体
    /// </summary>
    private Transform cardParent;

    private SocketMsg socketMsg;

    private PromptMsg promptMsg;

    // Use this for initialization
    void Start()
    {
        cardParent = transform.Find("cardPoint");
        cardCtrlList = new List<CardCtrl>();

        socketMsg = new SocketMsg();
        promptMsg = new PromptMsg();
    }
    /// <summary>
    /// 出选中的牌
    /// </summary>
    private void dealSelectCard()
    {
       
        DealDto dto = new DealDto(getSelectedCard(),Models.GameMode.userDto.Id);
        //进行出牌的合法性判断
        if (dto.IsRegular == false)
        {
            promptMsg.Change("请选择合理的手牌！", Color.red);
            Dispatch(AreaCode.UI, UIEvent.PROMPT_MSG, promptMsg);
            //设置出牌与不出的两个按钮可见
            //Dispatch(AreaCode.UI, UIEvent.SHOW_DEAL_BUTTON, true);
            return;
        }
        //走到这里代表出牌合法
        //向服务器发送出牌命令
        socketMsg.Change(OpCode.FIGHT, FightCode.DEAL_CREQ, dto);
        Dispatch(AreaCode.NET, 0, socketMsg);
    }
    /// <summary>
    /// 移除卡牌的游戏物体
    /// </summary>
    private void removeCard(List<CardDto> remainCardList)
    {
        int index = 0;
        foreach (var cc in cardCtrlList)
        {
            if (remainCardList.Count == 0)
                break;
            
            cc.gameObject.SetActive(true);
            cc.Init(remainCardList[index], index, true);
            index++;
            if (index == remainCardList.Count)
            {
                break;
            }
        }
        //把index之后的牌都隐藏掉
        for (int i = index; i < cardCtrlList.Count; i++)
        {
            cardCtrlList[i].Selected = false;
            //这里是销毁了 也可以设置对象池 或者先隐藏掉
            //销毁的是script所附着的游戏物体 因此实际上是拿到了上面的脚本就能拿到这个游戏物体
            //Destroy(cardCtrlList[i].gameObject);
            cardCtrlList[i].gameObject.SetActive(false);
        }
    }
    /// <summary>
    /// 获取选中的牌
    /// </summary>
    private List<CardDto> getSelectedCard()
    {
        List<CardDto> selectedCardList = new List<CardDto>();
        foreach (var item in cardCtrlList)
        {
            if (item.Selected)
            {
                selectedCardList.Add(item.CardDto);
            }
        }
        return selectedCardList;
    }
    /// <summary>
    /// 添加底牌的
    /// </summary>
    /// <param name="cardList"></param>
    private void addTableCard(GrabDto dto)
    {
        List<CardDto> tableCards = dto.TableCardList;
        List<CardDto> playerCards = dto.PlayerCardList;

        //复用之前创建的卡牌
        int index = 0;
        foreach (var cardCtrl in cardCtrlList)
        {
            cardCtrl.gameObject.SetActive(true);
            cardCtrl.Init(playerCards[index], index, true);
            //if (tableCards.Contains())
            //    cardCtrl.SelectState();
            index++;
        }
        //再创建新的三张卡牌
        GameObject cardPrefab = Resources.Load<GameObject>("Card/MyCard");
        for (int i = index; i < playerCards.Count; i++)
        {
            createGo(playerCards[i], i, cardPrefab);
        }
    }


    /// <summary>
    /// 初始化显示卡牌
    /// </summary>
    private IEnumerator initCardList(List<CardDto> cardList)
    {
        GameObject cardPrefab = Resources.Load<GameObject>("Card/MyCard");

        for (int i = 0; i < cardList.Count; i++)
        {
            createGo(cardList[i], i, cardPrefab);
            yield return new WaitForSeconds(0.1f);
        }
    }

    /// <summary>
    /// 创建卡牌游戏物体
    /// </summary>
    /// <param name="card"></param>
    /// <param name="index"></param>
    private void createGo(CardDto card, int index, GameObject cardPrefab)
    {
        GameObject cardGo = Object.Instantiate(cardPrefab, cardParent) as GameObject;
        cardGo.name = card.Name;
        cardGo.transform.localPosition = new Vector2((0.4f * index), 0);
        CardCtrl cardCtrl = cardGo.GetComponent<CardCtrl>();
        cardCtrl.Init(card, index, true);

        //存储本地
        this.cardCtrlList.Add(cardCtrl);
    }


}
