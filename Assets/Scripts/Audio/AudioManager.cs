using System;
using UnityEngine.Audio;
using UnityEngine;
using Unity.VisualScripting;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [SerializeField] private AudioMixerGroup _masterVolume;
    [SerializeField] private AudioMixerGroup _musicVolume;
    [SerializeField] private AudioMixerGroup _sfxVolume;

    [SerializeField] private AudioSource _musicSource1;
    [SerializeField] private AudioSource _musicSource2;
    [SerializeField] private AudioSource _sfxSource;

    [SerializeField] private Sound[] _music;
    [SerializeField] private Sound[] _sfx;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        _musicSource1.outputAudioMixerGroup = _musicVolume;
        _musicSource2.outputAudioMixerGroup = _musicVolume;
        _sfxSource.outputAudioMixerGroup = _sfxVolume;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G)) 
        {
            PlayMusic("TextScroll");
        }
    }
    private void SetAudioSourceClip(AudioSource source, Sound sound)
    {
        source.clip = sound.clip;
        source.volume = sound.volume;
        source.pitch = sound.pitch;
        source.loop = sound.loop;
    }

    public void PlayMusic(string name)
    {
        Sound sound = Array.Find(_music, sound => sound.name == name) ?? throw new Exception("Sound cannot be found");
        //SetAudioSourceClip(_musicSource, sound);
        //_musicSource.Play();
    }

    public void PlaySFX(string name)
    {
        Sound sound = Array.Find(_sfx, sound => sound.name == name) ?? throw new Exception("Sound cannot be found");
        SetAudioSourceClip(_sfxSource, sound);
        _sfxSource.PlayOneShot(_sfxSource.clip);
    }
}
