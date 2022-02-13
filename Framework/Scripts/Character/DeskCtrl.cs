using System.Collections;
using System.Collections.Generic;
using Protocol.Dto.Fight;
using UnityEngine;
using UnityEngine.UI;

public class DeskCtrl : CharacterBase
{
    void Awake()
    {
        Bind(CharacterEvent.UODATE_SHOE_DESK);
    }

    public override void Execute(int eventCode, object message)
    {
        switch (eventCode)
        {
            case CharacterEvent.UODATE_SHOE_DESK:
                updateShowDesk(message as List<CardDto>);
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

    // Use this for initialization
    void Start()
    {
        cardParent = transform.Find("cardPoint");
        cardCtrlList = new List<CardCtrl>();
    }
    /// <summary>
    /// 更新显示桌面的牌
    /// </summary>
    /// <param name="cardList"></param>
    private void updateShowDesk(List<CardDto> cardList)
    {
        if (cardCtrlList.Count > cardList.Count)
        {
            //原来比现在多
            //复用之前创建的卡牌
            int index = 0;
            foreach (var cardCtrl in cardCtrlList)
            {
                cardCtrl.gameObject.SetActive(true);
                cardCtrl.Init(cardList[index], index, true);
                //if (tableCards.Contains())
                //    cardCtrl.SelectState();
                index++;
                //如果牌没了 提前结束
                if (index == cardList.Count)
                {
                    break;
                }
            }
            //将多的隐藏掉
            for (int i = index; i < cardCtrlList.Count; i++)
            {
                cardCtrlList[i].gameObject.SetActive(false);
            }
        }
        else
        {
            //原来比现在少
            int index = 0;
            foreach (var cardCtrl in cardCtrlList)
            {
                cardCtrl.gameObject.SetActive(true);
                cardCtrl.Init(cardList[index], index, true);
                index++;
            }
            //再创建新的n张
            GameObject cardPrefab = Resources.Load<GameObject>("Card/DeskCard");
            for (int i = index; i < cardList.Count; i++)
            {
                createGo(cardList[i], i, cardPrefab);
            }

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
        cardGo.transform.localPosition = new Vector2((0.3f * index), 0);
        CardCtrl cardCtrl = cardGo.GetComponent<CardCtrl>();
        cardCtrl.Init(card, index, true);

        //存储本地
        this.cardCtrlList.Add(cardCtrl);
    }
}
