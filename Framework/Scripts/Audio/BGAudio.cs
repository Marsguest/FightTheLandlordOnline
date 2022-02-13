using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BGAudio : AudioBase
{
    void Awake()
    {
        Bind(AudioEvent.PLAY_BACKGROUND_AUDIO,
            AudioEvent.SET_BG_VOLUME,
            AudioEvent.STOP_BG_AUDIO);
    }

    public override void Execute(int eventCode, object message)
    {
        switch (eventCode)
        {
            case AudioEvent.PLAY_BACKGROUND_AUDIO:
                playAudio();
                break;
            case AudioEvent.SET_BG_VOLUME:
                setVolume((float)message);
                break;
            case AudioEvent.STOP_BG_AUDIO:
                stopAudio();
                break;
            default:
                break;
        }
    }
    /// <summary>
    /// 播放的声音源
    /// </summary>
    [SerializeField]
    private AudioSource audioSource;

    private void playAudio()
    {
        audioSource.Play();
    }
    private void setVolume(float value)
    {
        audioSource.volume = value;
    }
    private void stopAudio()
    {
        audioSource.Stop();
    }
}
