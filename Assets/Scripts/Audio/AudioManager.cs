using System;
using System.Collections;
using UnityEngine.Audio;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [SerializeField] private AudioMixerGroup _masterVolume;
    [SerializeField] private AudioMixerGroup _musicVolume;
    [SerializeField] private AudioMixerGroup _sfxVolume;

    [SerializeField] private AudioSource _musicSource1;
    [SerializeField] private AudioSource _musicSource2;
    [SerializeField] private GameObject _sfxSources;
    private bool _isPlayingMusicSource1;

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

        foreach (Sound sound in _sfx)
        {
            sound.source = _sfxSources.AddComponent<AudioSource>();
            SetAudioSourceClip(sound.source, sound);
            sound.source.outputAudioMixerGroup = _sfxVolume;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G)) 
        {
            SwapMusic("Explore");
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            StartCoroutine(FadeMusic("Home", 5f, 0.5f));
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            SetAudioSourceClip(_musicSource1, FindMusic("Home"));
        }
    }
    private void SetAudioSourceClip(AudioSource source, Sound sound)
    {
        source.clip = sound.clip;
        source.volume = sound.volume;
        source.pitch = sound.pitch;
        source.loop = sound.loop;
    }

    private Sound FindMusic(string name)
    {
        return Array.Find(_music, sound => sound.name == name) ?? throw new Exception("Sound cannot be found");
    }
    private Sound FindSFX(string name)
    {
        return Array.Find(_sfx, sound => sound.name == name) ?? throw new Exception("Sound cannot be found");
    }
    public void PlaySFX(string name)
    {
        Sound sfx = FindSFX(name);
        sfx.source.Play();
    }

    public void StopSFX(string name)
    {
        Sound sfx = FindSFX(name);
        sfx.source.Stop();
    }
    public void PlayOneShotSFX(string name)
    {
        Sound sfx = FindSFX(name);
        sfx.source.PlayOneShot(sfx.clip);
    }

    public bool IsSFXPlaying(string name)
    {
        Sound sfx = FindSFX(name);
        return sfx.source.isPlaying;
    }

    public void SetSFXSpeed(string name, float speed)
    {
        Sound sfx = FindSFX(name);
        sfx.source.pitch = speed;
        _sfxVolume.audioMixer.SetFloat("SFXPitch", 1f / speed);
    }
    public void ResetSFXSpeed(string name)
    {
        Sound sfx = FindSFX(name);
        sfx.source.pitch = 1;
        _sfxVolume.audioMixer.SetFloat("SFXPitch", 1f);
    }
    public void SwapMusic(string name)
    {
        Sound newSound = FindMusic(name);
        if (_isPlayingMusicSource1)
        {
            _isPlayingMusicSource1 = false;
            SetAudioSourceClip(_musicSource2, newSound);
            _musicSource2.Play();
            _musicSource1.Stop();
        }
        else
        {
            _isPlayingMusicSource1 = true;
            SetAudioSourceClip(_musicSource1, newSound);
            _musicSource1.Play();
            _musicSource2.Stop();
        }
    }

    // Fade into the volume specified by the clip
    public IEnumerator FadeMusic(string name, float fadeDuration) 
    {
        Sound newSound = FindMusic(name);
        float timeElapsed = 0f;
        if (_isPlayingMusicSource1)
        {
            _isPlayingMusicSource1 = false;
            SetAudioSourceClip(_musicSource2, newSound);
            _musicSource2.Play();

            float track1StartingVolume = _musicSource1.volume;
            float track2TargetVolume = _musicSource2.volume;

            while (timeElapsed < fadeDuration)
            {
                _musicSource1.volume = Mathf.Lerp(track1StartingVolume, 0, timeElapsed / fadeDuration);
                _musicSource2.volume = Mathf.Lerp(0, track2TargetVolume, timeElapsed / fadeDuration);
                timeElapsed += Time.deltaTime;
                yield return null;
            }
        }
        else
        {
            _isPlayingMusicSource1 = true;
            SetAudioSourceClip(_musicSource1, newSound);
            _musicSource1.Play();

            float track2StartingVolume = _musicSource2.volume;
            float track1TargetVolume = _musicSource1.volume;

            while (timeElapsed < fadeDuration)
            {
                _musicSource2.volume = Mathf.Lerp(track2StartingVolume, 0, timeElapsed / fadeDuration);
                _musicSource1.volume = Mathf.Lerp(0, track1TargetVolume, timeElapsed / fadeDuration);
                timeElapsed += Time.deltaTime;
                yield return null;
            }
        }
    }

    // Fade into the volume specified by targetVolume
    public IEnumerator FadeMusic(string name, float fadeDuration, float targetVolume)
    {
        if (targetVolume < 0 || targetVolume > 1)
        {
            throw new ArgumentOutOfRangeException("Target volume is out of range");
        }
        Sound newSound = FindMusic(name);
        float timeElapsed = 0f;
        if (_isPlayingMusicSource1)
        {
            _isPlayingMusicSource1 = false;
            SetAudioSourceClip(_musicSource2, newSound);
            _musicSource2.Play();

            float track1StartingVolume = _musicSource1.volume;
            float track2TargetVolume = targetVolume;

            while (timeElapsed < fadeDuration)
            {
                _musicSource1.volume = Mathf.Lerp(track1StartingVolume, 0, timeElapsed / fadeDuration);
                _musicSource2.volume = Mathf.Lerp(0, track2TargetVolume, timeElapsed / fadeDuration);
                timeElapsed += Time.deltaTime;
                yield return null;
            }
        }
        else
        {
            _isPlayingMusicSource1 = true;
            SetAudioSourceClip(_musicSource1, newSound);
            _musicSource1.Play();

            float track2StartingVolume = _musicSource2.volume;
            float track1TargetVolume = targetVolume;

            while (timeElapsed < fadeDuration)
            {
                _musicSource2.volume = Mathf.Lerp(track2StartingVolume, 0, timeElapsed / fadeDuration);
                _musicSource1.volume = Mathf.Lerp(0, track1TargetVolume, timeElapsed / fadeDuration);
                timeElapsed += Time.deltaTime;
                yield return null;
            }
        }
    }
}
