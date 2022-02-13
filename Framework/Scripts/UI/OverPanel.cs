using System.Collections;
using System.Collections.Generic;
using Protocol;
using Protocol.Code;
using Protocol.Constant;
using Protocol.Dto.Fight;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
/// <summary>
/// 游戏结束面板
/// </summary>
public class OverPanel : UIBase
{
    private void Awake()
    {
        Bind(UIEvent.SHOW_OVER_PANEL);
    }
    public override void Execute(int eventCode, object message)
    {
        switch (eventCode)
        {
            case UIEvent.SHOW_OVER_PANEL:
                refreshPanel(message as OverDto);
                break;
            default:
                break;
        }
    }

    private Text txtWinIdentity;
    private Text txtWinBeen;
    private Button btnBack;

    void Start()
    {
        txtWinIdentity = transform.Find("txtWinIdentity").GetComponent<Text>();
        txtWinBeen = transform.Find("txtWinBeen").GetComponent<Text>();
        btnBack = transform.Find("btnBack").GetComponent<Button>();

        btnBack.onClick.AddListener(btnBackClick);

        //默认状态
        setPanelActive(false);
    }

    public override void OnDestory()
    {
        base.OnDestory();
        btnBack.onClick.RemoveAllListeners();
    }

    /// <summary>
    /// 返回主菜单点击事件
    /// </summary>
    private void btnBackClick()
    {
        //LoadSceneMsg loadSceneMsg = new LoadSceneMsg(1,
        //        () =>
        //        {
        //            //向服务器获取信息
        //            //Debug.Log("加载完成！");
        //            SocketMsg socketNsg = new SocketMsg(OpCode.USER, UserCode.GET_INFO_CREQ, null);
        //            Dispatch(AreaCode.NET, 0, socketNsg);
        //        });
        //Dispatch(AreaCode.SCENE, SceneEvent.LOAD_SCENE, loadSceneMsg);
        //TODO loadScene
        SceneManager.LoadScene(1);
        SceneManager.sceneLoaded += loadedEve;
    }
    private void loadedEve(Scene s, LoadSceneMode l)
    {
        SocketMsg socketNsg = new SocketMsg(OpCode.USER, UserCode.GET_INFO_CREQ, null);
        Dispatch(AreaCode.NET, 0, socketNsg);
    }
    /// <summary>
    /// 刷新显示
    /// </summary>
    private void refreshPanel(OverDto dto)
    {
        setPanelActive(true);
        //TODO 这里感觉不太合理 胜利的身份加上了自己是否胜利？  应该是自己的身份加上自己是否胜利吧
        //显示谁胜利
        txtWinIdentity.text = Identity.GetString(dto.WinIdentity);
        txtWinIdentity.text += "胜利";
        //判断自己是否胜利
        if (dto.WinUIdList.Contains(Models.GameMode.userDto.Id))
        {
            //txtWinIdentity.text += "胜利";
            txtWinBeen.text = "欢乐豆 + ";
        }
        else
        {
            //txtWinIdentity.text += "失败 ";
            txtWinBeen.text = "欢乐豆 - ";
        }
        //显示豆子数量
        txtWinBeen.text += dto.BeanCount;
    }
}
