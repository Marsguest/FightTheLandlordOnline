using System.Collections;
using System.Collections.Generic;
using Protocol.Dto;
using UnityEngine;
using UnityEngine.UI;

public class infoPanel : UIBase
{
    private void Awake()
    {
        Bind(UIEvent.REFRESH_INFO_PANEL);
    }

    public override void Execute(int eventCode, object message)
    {
        switch (eventCode)
        {
            case UIEvent.REFRESH_INFO_PANEL:
                UserDto dto = message as UserDto;
                refreshPanel(dto.Name,dto.Lv,dto.Exp,dto.Bean);
                break;
            default:
                break;
        }
    }
    //换头像的操作
    //private Image imgHead;
    private Text txtName;
    private Text txtLv;
    private Slider sldExp;
    private Text txtExp;
    private Text txtBean;

    // Start is called before the first frame update
    void Start()
    {
        txtName = transform.Find("txtName").GetComponent<Text>();
        txtLv = transform.Find("txtLv").GetComponent<Text>();
        sldExp = transform.Find("sldExp").GetComponent<Slider>();
        txtExp = transform.Find("txtExp").GetComponent<Text>();
        txtBean = transform.Find("txtBean").GetComponent<Text>();

        setPanelActive(true);
    }
    /// <summary>
    /// 刷新视图
    ///     名字 等级 经验 豆子
    /// </summary>
    public void refreshPanel(string name, int lv, int exp,int bean)
    {
        //Debug.LogError("here coming refreshView");
        txtName.text = name;
        txtLv.text = "Lv." + lv;
        //等级和经验之间的公式：maxExp = lv * 100
        txtExp.text = exp + " / "+lv * 100;
        sldExp.value = (float) exp / (lv * 100);
        txtBean.text = "× "+bean.ToString();
    }
}
