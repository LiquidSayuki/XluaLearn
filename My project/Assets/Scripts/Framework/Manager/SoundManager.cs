using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoSingleton<SoundManager>
{
    AudioSource m_MusicAudio;
    AudioSource m_SoundAudio;

    private float SoundVolume
    {
        get { return PlayerPrefs.GetFloat("SoundVolume", 1.0f); }
        set
        {
            m_SoundAudio.volume = value;
            PlayerPrefs.SetFloat("SoundVolume", value);
        }
    }
    private float MusicVolume
    {
        get { return PlayerPrefs.GetFloat("MusicVolume", 1.0f); }
        set
        {
            m_MusicAudio.volume= value;
            PlayerPrefs.SetFloat("MusicVolume", value);
        }
    }

    private void Awake()
    {
        m_MusicAudio = this.gameObject.AddComponent<AudioSource>();
        m_MusicAudio.playOnAwake = false;
        m_MusicAudio.loop= true;

        m_SoundAudio = this.gameObject.AddComponent<AudioSource>();
        m_SoundAudio.playOnAwake = false;
    }

    public void PlayMusic(string name)
    {
        if (this.MusicVolume < 0.1f)
        {
            return;
        }
        // 不重复播放相同音乐
        string oldName = "";
        if (m_MusicAudio.clip != null)
        {
            oldName = m_MusicAudio.clip.name;
        }
        if (oldName == name)
        {
            m_MusicAudio.Play();
            return;
        }

        GameManager.Resource.LoadMusic(name, (UnityEngine.Object obj) =>
        {
            m_MusicAudio.clip = obj as AudioClip;
            m_MusicAudio.Play();
        });
    }

    public void PauseMusic()
    {
        m_MusicAudio.Pause();
    }
    public void UnpauseMusic()
    {
        m_MusicAudio.UnPause();
    }
    public void StopMusic()
    {
        m_MusicAudio.Stop();
    }

    public void PlaySound(string name)
    {
        if (this.SoundVolume < 0.1f)
        {
            return;
        }
        GameManager.Resource.LoadSound(name, (UnityEngine.Object obj) =>
        {
            m_SoundAudio.PlayOneShot(obj as AudioClip);
        });
    }

    public void SetMusicVolume(float value)
    {
        this.MusicVolume= value;
    }

    public void SetSoundVolume(float value)
    {
        this.SoundVolume = value;
    }
}
