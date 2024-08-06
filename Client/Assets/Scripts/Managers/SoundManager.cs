using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class SoundManager
{
    #region Fields
    GameObject m_root;
    AudioSource[] m_audioSources = new AudioSource[(int)Define.Sound.Max];
    Dictionary<string, AudioClip> m_audioClips = new Dictionary<string, AudioClip>();
    #endregion
    #region Init
    public void Init()
    {
        m_root = new GameObject("@SoundRoot");
        Object.DontDestroyOnLoad(m_root);

        string[] typeNames = System.Enum.GetNames(typeof(Define.Sound));
        for (int i = 0; i < typeNames.Length - 1; i++)
        {
            GameObject go = new GameObject(typeNames[i]);
            go.transform.SetParent(m_root.transform);

            m_audioSources[i] = go.AddComponent<AudioSource>();

            if (i == (int)Define.Sound.Bgm)
                m_audioSources[i].loop = true;
        }

        Managers.Data.Setting.IsBgm.Subscribe(OnChangeBgm);
    }

    public void Clear()
    {
        foreach (AudioSource source in m_audioSources)
            source.Stop();

        m_audioClips.Clear();
    }
    #endregion
    #region Control
    public void Play(Define.Sound type, string path, float volume = 1.0f)
    {
        if (string.IsNullOrEmpty(path) || !IsPlay(type))
            return;

        AudioSource source = m_audioSources[(int)type];
        source.volume = volume;

        AudioClip clip = GetAudioClip(type, $"Sounds/{path}");
        if (clip == null)
            return;

        switch (type)
        {
            case Define.Sound.Bgm:
                {
                    if (source.isPlaying)
                        source.Stop();

                    source.clip = clip;
                    source.Play();
                }
                break;
            case Define.Sound.Sfx:
                source.PlayOneShot(clip);
                break;
        }
    }

    public void Stop(Define.Sound type)
    {
        m_audioSources[(int)type].Stop();
    }
    #endregion
    #region Is
    bool IsPlay(Define.Sound type)
    {
        switch (type)
        {
            case Define.Sound.Bgm:
                return Managers.Data.Setting.IsBgm.Value;
            case Define.Sound.Sfx:
                return Managers.Data.Setting.IsSfx.Value;
        }

        return false;
    }
    #endregion
    #region Get
    AudioClip GetAudioClip(Define.Sound type, string path)
    {
        if (m_audioClips.TryGetValue(path, out AudioClip clip))
            return clip;

        clip = Managers.Resource.Load<AudioClip>(path);
        if (clip == null)
        {
            Debug.LogError($"[SoundManager:GetAudioClip] AudioClip not found at path: {path}");
            return null;
        }

        m_audioClips.Add(path, clip);
        return clip;
    }
    #endregion
    #region Callback
    void OnChangeBgm(bool isOn)
    {
        if (isOn)
            Play(Define.Sound.Bgm, "Bgm", 0.3f);
        else
            Stop(Define.Sound.Bgm);
    }
    #endregion
}
