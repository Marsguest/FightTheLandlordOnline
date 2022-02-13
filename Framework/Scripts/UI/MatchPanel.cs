using System.Collections;
using System.Collections.Generic;
using Protocol;
using Protocol.Code;
using UnityEngine;
using UnityEngine.UI;

public class MatchPanel : UIBase
{
    private void Awake()
    {
        Bind(UIEvent.SHOW_ENTER_ROOM_BUTTON);
    }

    public override void Execute(int eventCode, object message)
    {
        switch (eventCode)
        {
            case UIEvent.SHOW_ENTER_ROOM_BUTTON:
                btnEnter.gameObject.SetActive(true);
                break;
            default:
                break;
        }
    }

    private Button btnMatch;
    private Image imgBg;
    private Text txtDes;//描述
    private Button btnCancel;
    private Button btnEnter;

    private SocketMsg socketMsg;

    void Start()
    {
        btnMatch = transform.Find("btnMatch").GetComponent<Button>();
        imgBg = transform.Find("imgBg").GetComponent<Image>();
        txtDes = transform.Find("txtDes").GetComponent<Text>();
        btnCancel = transform.Find("btnCancel").GetComponent<Button>();
        btnEnter = transform.Find("btnEnter").GetComponent<Button>();

        btnMatch.onClick.AddListener(btnMatchClick);
        btnCancel.onClick.AddListener(btnCancelClick);
        btnEnter.onClick.AddListener(btnEnterClick);
        //默认状态
        setObjectsActive(false);
        btnEnter.gameObject.SetActive(false);

        socketMsg = new SocketMsg();
    }

    void Update()
    {
        if (txtDes.gameObject.activeInHierarchy == false)
            return;
        timer += Time.deltaTime;
        if (timer >= intervalTime)
        {
            doAnimation();
            timer = 0f;
        }
    }

    public override void OnDestory()
    {
        base.OnDestory();

        btnMatch.onClick.RemoveListener(btnMatchClick);
        btnCancel.onClick.RemoveListener(btnCancelClick);
        btnEnter.onClick.RemoveListener(btnEnterClick);
    }

    private void btnMatchClick()
    {
        //向服务器发起匹配的请求
        //加入客户端保存了自身角色id的id 发消息的时候都把自身的id发给服务器
        socketMsg.Change(OpCode.MATCH, MatchCode.ENTER_MATCH_QUEUE_CREQ, null);
        Dispatch(AreaCode.NET, 0, socketMsg);

        setObjectsActive(true);

        //将快速匹配按钮隐藏掉
        btnMatch.gameObject.SetActive(false);

    }

    private void btnCancelClick()
    {
        //TODO 向服务器发起离开匹配的请求
        socketMsg.Change(OpCode.MATCH, MatchCode.LEAVE_MATCH_QUEUE_CREQ, null);
        Dispatch(AreaCode.NET, 0, socketMsg);

        setObjectsActive(false);
        dotCount = 0;

        //将快速匹配按钮显示出来
        btnEnter.gameObject.SetActive(false);
        btnMatch.gameObject.SetActive(true);
    }
    /// <summary>
    /// 控制点击匹配按钮之后显示的物体
    /// </summary>
    /// <param name="active"></param>
    private void setObjectsActive(bool active)
    {
        imgBg.gameObject.SetActive(active);
        btnCancel.gameObject.SetActive(active);
        txtDes.gameObject.SetActive(active);
    }

    #region 正在寻找房间的动画
    private string defaultText = "正在寻找房间";
    //点的数量
    private int dotCount = 0;
    //private bool isMatching = false;
    //动画的间隔时间
    private float intervalTime = 1f;
    //计时器
    private float timer = 0f;
    /// <summary>
    /// 做动画
    /// </summary>
    private void doAnimation()
    {
        txtDes.text = defaultText;
        //点增加
        dotCount++;
        if (dotCount > 5)
        {
            dotCount = 1;
        }
        for (int i = 0; i < dotCount; i++)
        {
            txtDes.text += ".";
        }
    }
    #endregion

    private void btnEnterClick()
    {
        Dispatch(AreaCode.SCENE, SceneEvent.LOAD_SCENE, new LoadSceneMsg(2,null));
    }


}
