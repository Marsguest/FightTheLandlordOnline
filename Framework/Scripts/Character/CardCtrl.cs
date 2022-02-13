using System.Collections;
using System.Collections.Generic;
using Protocol.Dto.Fight;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 卡牌控制类
/// </summary>
public class CardCtrl : MonoBehaviour
{
    //数据
    public CardDto CardDto { get; set; }
    public bool Selected { get; set; }

    private SpriteRenderer spriteRenderer;
    private bool isMine;
    
    /// <summary>
    /// 初始化卡牌数据
    /// </summary>
    /// <param name="card"></param>
    /// <param name="index">重叠显示顺序</param>
    /// <param name="isMine"></param>
    public void Init(CardDto card,int index,bool _isMine)
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        this.CardDto = card;
        this.isMine = _isMine;
        //为了重用
        if (Selected)
        {
            Selected = false;
            transform.localPosition -= new Vector3(0, .3f, 0);
        }
        string resPath = string.Empty;
        if (isMine)
        {
            resPath = "Poker/" + card.Name;
        }
        else
            resPath = "Poker/CardBack";
        Sprite sp = Resources.Load<Sprite>(resPath);
        spriteRenderer.sprite = sp;
        spriteRenderer.sortingOrder = index;
    }
    void OnMouseDown()
    {
        if (isMine == false)
            return;
        //切换状态
        this.Selected = !Selected;
        //位置上调
        if (Selected)
        {
            transform.localPosition += new Vector3(0, .3f, 0);
        }
        else
        {
            transform.localPosition -= new Vector3(0, .3f, 0);
        }
    }
    /// <summary>
    /// 选择的状态
    /// </summary>
    public void SelectState()
    {
        //位置上调
        if (Selected == false)
        {
            this.Selected = true;
            transform.localPosition += new Vector3(0, .3f, 0);
        }
    }
}
