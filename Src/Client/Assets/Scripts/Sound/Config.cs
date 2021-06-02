using UnityEngine;

class Config
{
    public static bool MusicOn
    {
        get
        {
            return PlayerPrefs.GetInt("Music", 1) == 1;
        }
        set
        {
            PlayerPrefs.SetInt("Music", value ? 1 : 0);
            SoundManager.Instance.MusicOn = value;
        }
    }

    public static bool SoundOn
    {
        get
        {
            return PlayerPrefs.GetInt("Sound", 1) == 1;
        }
        set
        {
            PlayerPrefs.SetInt("Sound", value ? 1 : 0);
            SoundManager.Instance.SoundOn = value;
        }
    }

    public static int MusicVolume
    {
        get
        {
            return PlayerPrefs.GetInt("MusicVol", 100);
        }
        set
        {
            PlayerPrefs.SetInt("MusicVol", value);
            SoundManager.Instance.MusicVolume = value;
        }
    }

    public static int SoundVolume
    {
        get
        {
            return PlayerPrefs.GetInt("SoundVol", 100);
        }
        set
        {
            PlayerPrefs.SetInt("SoundVol", value);
            SoundManager.Instance.SoundVolume = value;
        }
    }

    ~Config()
    {
        PlayerPrefs.Save();
    }
}
