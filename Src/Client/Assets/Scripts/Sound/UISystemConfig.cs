using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISystemConfig : UIWindow
{
    public Slider MusicSlider;
    public Slider SoundSlider;

    public Image MusicOff;
    public Image SoundOff;

    public Toggle MusicToggle;
    public Toggle SoundToggle;

    private void Start()
    {
        MusicToggle.isOn = Config.MusicOn;
        SoundToggle.isOn = Config.SoundOn;
        MusicSlider.value = Config.MusicVolume;
        SoundSlider.value = Config.SoundVolume;
    }

    public override void OnClickConfirm()
    {
        SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Click);
        PlayerPrefs.Save();
        base.OnClickConfirm();
    }

    public void OnClickMusicVolume(float volume)
    {
        if (!MusicToggle.isOn) return;
        Config.MusicVolume = (int)volume;
        TestSound();
    }

    public void OnClickMusicToggle(bool ifOn)
    {
        MusicOff.enabled = !ifOn;
        Config.MusicOn = ifOn;
        SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Click);
    }

    public void OnClickSoundVolume(float volume)
    {
        if (!SoundToggle.isOn) return;
        Config.SoundVolume = (int)volume;
        TestSound();
    }

    public void OnClickSoundToggle(bool ifOn)
    {
        SoundOff.enabled = !ifOn;
        Config.SoundOn = ifOn;
        SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Click);
    }

    private float lastPlayed = 0;
    private void TestSound()
    {
        if(Time.realtimeSinceStartup - lastPlayed > 0.1)
        {
            lastPlayed = Time.realtimeSinceStartup;
            SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Click);
        }
    }
}
