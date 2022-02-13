using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetPanel : UIBase
{
    private void Awake()
    {
        Bind();
    }

    public override void Execute(int eventCode, object message)
    {
        switch (eventCode)
        {
            case UIEvent.SHOW_ENTER_ROOM_BUTTON:
                break;
            default:
                break;
        }
    }

    private Button btnSet;
    private Image imgBg;
    private Button btnClose;
    private Toggle togAudio;
    private Slider sldVolume;
    private Button btnQuit;
    private Text txtAudio;
    private Text txtVolume;
    
    void Start()
    {
        btnSet = transform.Find("btnSet").GetComponent<Button>();
        imgBg = transform.Find("imgBg").GetComponent<Image>();
        btnClose = transform.Find("btnClose").GetComponent<Button>();
        togAudio = transform.Find("togAudio").GetComponent<Toggle>();
        sldVolume = transform.Find("sldVolume").GetComponent<Slider>();
        btnQuit = transform.Find("btnQuit").GetComponent<Button>();
        txtAudio = transform.Find("txtAudio").GetComponent<Text>();
        txtVolume = transform.Find("txtVolume").GetComponent<Text>();

        btnSet.onClick.AddListener(btnSetClick);
        btnClose.onClick.AddListener(btnCloseClick);
        btnQuit.onClick.AddListener(btnQuieClick);
        togAudio.onValueChanged.AddListener(togAudioValueChanged);
        sldVolume.onValueChanged.AddListener(sldVolumeValueChanged);

        //默认状态
        setObjectsActive(false);
        

    }

    public override void OnDestory()
    {
        base.OnDestory();

        btnSet.onClick.RemoveAllListeners();
        btnClose.onClick.RemoveAllListeners();
        btnQuit.onClick.RemoveAllListeners();
        togAudio.onValueChanged.RemoveAllListeners();
        sldVolume.onValueChanged.RemoveAllListeners();

    }
    private void setObjectsActive(bool active)
    {
        imgBg.gameObject.SetActive(active);
        btnClose.gameObject.SetActive(active);
        togAudio.gameObject.SetActive(active);
        sldVolume.gameObject.SetActive(active);
        btnQuit.gameObject.SetActive(active);
        txtAudio.gameObject.SetActive(active);
        txtVolume.gameObject.SetActive(active);
    }

    private void btnSetClick()
    {
        setObjectsActive(true);
    }

    private void btnCloseClick()
    {
        setObjectsActive(false);
    }

    private void btnQuieClick()
    {
        Application.Quit();
    }
    /// <summary>
    /// 开关点击的时候调用
    /// </summary>
    /// <param name="result">勾上是true</param>
    private void togAudioValueChanged(bool result)
    {
        //操作声音
        if (result == true)
        {
            Dispatch(AreaCode.AUDIO, AudioEvent.PLAY_BACKGROUND_AUDIO, null);
        }
        else
        {
            Dispatch(AreaCode.AUDIO, AudioEvent.STOP_BG_AUDIO, null);
        }
    }
    /// <summary>
    /// 当滑动条滑动的时候会调用
    /// </summary>
    /// <param name="value">滑动条的值</param>
    private void sldVolumeValueChanged(float value)
    {
        //操作声音
        Dispatch(AreaCode.AUDIO, AudioEvent.SET_BG_VOLUME, value);
    }
}
