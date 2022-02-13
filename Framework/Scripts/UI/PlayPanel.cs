using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayPanel : UIBase
{
    private Button btnStart;
    private Button btnRegist;
   
    // Start is called before the first frame update
    void Start()
    {
        btnStart = transform.Find("btnStart").GetComponent<Button>();
        btnRegist = transform.Find("btnRegist").GetComponent<Button>();

        btnStart.onClick.AddListener(btnStartClick);
        btnRegist.onClick.AddListener(btnRegistClick);

    }

    public override void OnDestory()
    {
        base.OnDestory();
        btnStart.onClick.RemoveListener(btnStartClick);
        btnRegist.onClick.RemoveListener(btnRegistClick);
    }

    private void btnStartClick()
    {
        Dispatch(AreaCode.UI,UIEvent.START_PANEL_ACTIVE,true);
    }

    private void btnRegistClick()
    {
        Dispatch(AreaCode.UI, UIEvent.REGIST_PANEL_ACTIVE, true);
    }



}
