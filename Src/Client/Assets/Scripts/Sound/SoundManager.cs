using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Audio;

public class SoundManager : MonoSingleton<SoundManager>
{
    public AudioMixer audioMixer;
    public AudioSource musicSource;
    public AudioSource soundSource;

    private string musicPath = "Music/";
    private string soundPath = "Sound/";

    private bool musicOn;
    public bool MusicOn
    {
        get { return musicOn; }
        set
        {
            musicOn = value;
            MusicMute(!musicOn);
        }
    }

    private bool soundOn;
    public bool SoundOn
    {
        get { return soundOn; }
        set
        {
            soundOn = value;
            SoundMute(!soundOn);
        }
    }

    private int musicVol;
    public int MusicVolume
    {
        get { return musicVol; }
        set
        {
            musicVol = value;
            SetVolume("MusicVolume", musicVol);
        }
    }

    private int soundVol;
    public int SoundVolume
    {
        get { return soundVol; }
        set
        {
            soundVol = value;
            SetVolume("SoundVolume", soundVol);
        }
    }

    private void Start()
    {
        MusicOn = Config.MusicOn;
        SoundOn = Config.SoundOn;
        MusicVolume = Config.MusicVolume;
        SoundVolume = Config.SoundVolume;
    }

    private void MusicMute(bool ifMute)
    {
        SetVolume("MusicVolume", ifMute ? 0 : musicVol);
    }

    private void SoundMute(bool ifMute)
    {
        SetVolume("SoundVolume", ifMute ? 0 : soundVol);
    }

    private void SetVolume(string sourceParam, int vol)
    {
        var volume = vol * 0.5f - 50f;
        audioMixer.SetFloat(sourceParam,volume);
    }

    public void PlayMusic(string source)
    {
        AudioClip clip = Resloader.Load<AudioClip>(musicPath + source);

        if(clip == null)
        {
            Debug.LogErrorFormat("PlayMusic: Clip:{0} does not exist", source);
            return;
        }
        if(musicSource.isPlaying)
        {
            musicSource.Stop();
        }

        musicSource.clip = clip;
        musicSource.Play();
    }

    public void PlaySound(string source)
    {
        AudioClip clip = Resloader.Load<AudioClip>(soundPath + source);

        if (clip == null)
        {
            Debug.LogErrorFormat("PlaySound: Clip:{0} does not exist", source);
            return;
        }
        soundSource.PlayOneShot(clip);
    }

    protected void PlayClipOnAudioSource(AudioSource source, AudioClip clip, bool ifLoop)
    {

    }
}
