using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EffectAudio : AudioBase
{
    void Awake()
    {
        Bind(AudioEvent.PLAY_EFFECT_AUDIO);
    }

    public override void Execute(int eventCode, object message)
    {
        base.Execute(eventCode, message);
        switch (eventCode)
        {
            case AudioEvent.PLAY_EFFECT_AUDIO:
                {
                    playEffectAudio(message.ToString());
                    break;
                }
            default:
                break;
        }
    }
    /// <summary>
    /// 播放音乐的组件
    /// </summary>
    private AudioSource audioSource;
    
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    /// <summary>
    /// 播放音乐
    /// </summary>
    private void playEffectAudio(string assertName)
    {
        AudioClip ac = Resources.Load<AudioClip>("Sound/"+ assertName);
        //Debug.Log("要播放的音频文件路径为：" + "Sound/" + assertName);
        if (ac == null)
        {
            Debug.LogError("未找到音频文件 文件路径："+ "Sound/" + assertName);
        }
        audioSource.clip = ac;
        audioSource.Play();
    }
}
