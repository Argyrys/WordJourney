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
            EnsureAudioSources();
            GenerateProceduralClips();
            LoadAudioSettings();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void EnsureAudioSources()
    {
        if (musicSource == null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.loop = true;
            musicSource.playOnAwake = false;
        }
        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.playOnAwake = false;
        }
    }

    private void GenerateProceduralClips()
    {
        if (buttonClickClip == null)
            buttonClickClip = GenerateToneClip(0.06f, 660f, 0.3f, true);
        if (wordFoundClip == null)
            wordFoundClip = GenerateMelodyClip(new float[] { 523.25f, 659.25f, 783.99f }, 0.09f, 0.35f);
        if (levelCompleteClip == null)
            levelCompleteClip = GenerateMelodyClip(new float[] { 523.25f, 659.25f, 783.99f, 1046.5f }, 0.14f, 0.4f);
        if (errorClip == null)
            errorClip = GenerateToneClip(0.18f, 200f, 0.3f, false);
        if (shuffleClip == null)
            shuffleClip = GenerateToneClip(0.12f, 400f, 0.25f, true);
        if (musicClip == null)
            musicClip = GenerateMusicClip();
    }

    private AudioClip GenerateMusicClip()
    {
        int sampleRate = 44100;
        float duration = 8f;
        int sampleCount = Mathf.CeilToInt(sampleRate * duration);
        float[] samples = new float[sampleCount];

        float[] notes = { 261.63f, 329.63f, 392.00f, 523.25f };
        float[] vibratoFreq = { 0.4f, 0.3f, 0.5f, 0.35f };
        float[] vibratoDepth = { 0.6f, 0.8f, 0.7f, 1.0f };

        for (int i = 0; i < sampleCount; i++)
        {
            float time = (float)i / sampleRate;
            float value = 0f;

            for (int n = 0; n < notes.Length; n++)
            {
                float phase = Mathf.Repeat(time * vibratoFreq[n], 1f);
                float env = 0.4f + 0.6f * Mathf.Sin(phase * Mathf.PI * 2f);
                float freq = notes[n] * (1f + 0.05f * Mathf.Sin(time * 0.7f * (n + 1f)));
                value += Mathf.Sin(2f * Mathf.PI * freq * time) * 0.07f * env * vibratoDepth[n];
            }

            samples[i] = Mathf.Clamp(value, -0.5f, 0.5f);
        }

        AudioClip clip = AudioClip.Create("Music", sampleCount, 1, sampleRate, false);
        clip.SetData(samples, 0);
        return clip;
    }

    private AudioClip GenerateToneClip(float duration, float frequency, float volume, bool decay)
    {
        int sampleRate = 44100;
        int sampleCount = Mathf.CeilToInt(sampleRate * duration);
        float[] samples = new float[sampleCount];

        for (int i = 0; i < sampleCount; i++)
        {
            float t = (float)i / sampleRate;
            float env = decay ? 1f - (t / duration) : 1f;
            samples[i] = Mathf.Sin(2f * Mathf.PI * frequency * t) * volume * env;
        }

        AudioClip clip = AudioClip.Create("Tone", sampleCount, 1, sampleRate, false);
        clip.SetData(samples, 0);
        return clip;
    }

    private AudioClip GenerateMelodyClip(float[] frequencies, float noteDuration, float volume)
    {
        int sampleRate = 44100;
        int noteSamples = Mathf.CeilToInt(sampleRate * noteDuration);
        int totalSamples = noteSamples * frequencies.Length;
        float[] samples = new float[totalSamples];

        for (int n = 0; n < frequencies.Length; n++)
        {
            for (int i = 0; i < noteSamples; i++)
            {
                float t = (float)i / sampleRate;
                float env = 1f - ((float)i / noteSamples);
                samples[n * noteSamples + i] = Mathf.Sin(2f * Mathf.PI * frequencies[n] * t) * volume * env;
            }
        }

        AudioClip clip = AudioClip.Create("Melody", totalSamples, 1, sampleRate, false);
        clip.SetData(samples, 0);
        return clip;
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
