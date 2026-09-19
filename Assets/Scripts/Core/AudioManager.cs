using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("Audio Clips")]
    public AudioClip musicClip;
    public AudioClip buttonClickClip;
    public AudioClip wordFoundClip;
    public AudioClip levelCompleteClip;
    public AudioClip errorClip;
    public AudioClip shuffleClip;

    private float musicVolume = 1f;
    private float sfxVolume = 1f;
    private bool musicEnabled = true;
    private bool sfxEnabled = true;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadAudioSettings();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (musicEnabled && musicClip != null)
        {
            PlayMusic();
        }
    }

    private void LoadAudioSettings()
    {
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
        musicEnabled = PlayerPrefs.GetInt("MusicEnabled", 1) == 1;
        sfxEnabled = PlayerPrefs.GetInt("SFXEnabled", 1) == 1;
    }

    public void PlayMusic()
    {
        if (musicSource != null && musicClip != null)
        {
            musicSource.clip = musicClip;
            musicSource.loop = true;
            musicSource.volume = musicEnabled ? musicVolume : 0f;
            musicSource.Play();
        }
    }

    public void StopMusic()
    {
        if (musicSource != null)
        {
            musicSource.Stop();
        }
    }

    public void PlaySFX(AudioClip clip)
    {
        if (sfxSource != null && clip != null && sfxEnabled)
        {
            sfxSource.PlayOneShot(clip, sfxVolume);
        }
    }

    public void PlayButtonClick()
    {
        PlaySFX(buttonClickClip);
    }

    public void PlayWordFound()
    {
        PlaySFX(wordFoundClip);
    }

    public void PlayLevelComplete()
    {
        PlaySFX(levelCompleteClip);
    }

    public void PlayError()
    {
        PlaySFX(errorClip);
    }

    public void PlayShuffle()
    {
        PlaySFX(shuffleClip);
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = volume;
        if (musicSource != null)
        {
            musicSource.volume = musicEnabled ? volume : 0f;
        }
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = volume;
    }

    public void ToggleMusic(bool enabled)
    {
        musicEnabled = enabled;
        if (musicSource != null)
        {
            musicSource.volume = enabled ? musicVolume : 0f;
        }
    }

    public void ToggleSFX(bool enabled)
    {
        sfxEnabled = enabled;
    }
}
