using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsManager : MonoBehaviour
{
    [Header("References")]
    public MenuManager menuManager;

    [Header("Audio Settings")]
    public Slider musicSlider;
    public Slider sfxSlider;
    public Toggle musicToggle;
    public Toggle sfxToggle;

    [Header("Other Settings")]
    public Toggle notificationsToggle;
    public Button resetProgressButton;
    public Button creditsButton;

    [Header("Panels")]
    public GameObject creditsPanel;
    public Button closeCreditsButton;

    private const string MUSIC_VOLUME = "MusicVolume";
    private const string SFX_VOLUME = "SFXVolume";
    private const string MUSIC_ENABLED = "MusicEnabled";
    private const string SFX_ENABLED = "SFXEnabled";
    private const string NOTIFICATIONS_ENABLED = "NotificationsEnabled";

    private void Start()
    {
        LoadSettings();
        SetupButtons();
    }

    private void SetupButtons()
    {
        musicSlider?.onValueChanged.AddListener(OnMusicVolumeChanged);
        sfxSlider?.onValueChanged.AddListener(OnSFXVolumeChanged);
        musicToggle?.onValueChanged.AddListener(OnMusicToggleChanged);
        sfxToggle?.onValueChanged.AddListener(OnSFXToggleChanged);
        notificationsToggle?.onValueChanged.AddListener(OnNotificationsToggleChanged);
        resetProgressButton?.onClick.AddListener(OnResetProgressClicked);
        creditsButton?.onClick.AddListener(OnCreditsClicked);
        closeCreditsButton?.onClick.AddListener(OnCloseCreditsClicked);
    }

    private void LoadSettings()
    {
        float musicVolume = PlayerPrefs.GetFloat(MUSIC_VOLUME, 1f);
        float sfxVolume = PlayerPrefs.GetFloat(SFX_VOLUME, 1f);
        bool musicEnabled = PlayerPrefs.GetInt(MUSIC_ENABLED, 1) == 1;
        bool sfxEnabled = PlayerPrefs.GetInt(SFX_ENABLED, 1) == 1;
        bool notificationsEnabled = PlayerPrefs.GetInt(NOTIFICATIONS_ENABLED, 1) == 1;

        if (musicSlider != null) musicSlider.value = musicVolume;
        if (sfxSlider != null) sfxSlider.value = sfxVolume;
        if (musicToggle != null) musicToggle.isOn = musicEnabled;
        if (sfxToggle != null) sfxToggle.isOn = sfxEnabled;
        if (notificationsToggle != null) notificationsToggle.isOn = notificationsEnabled;
    }

    private void OnMusicVolumeChanged(float value)
    {
        PlayerPrefs.SetFloat(MUSIC_VOLUME, value);
        PlayerPrefs.Save();
        AudioManager.Instance?.SetMusicVolume(value);
    }

    private void OnSFXVolumeChanged(float value)
    {
        PlayerPrefs.SetFloat(SFX_VOLUME, value);
        PlayerPrefs.Save();
        AudioManager.Instance?.SetSFXVolume(value);
    }

    private void OnMusicToggleChanged(bool enabled)
    {
        PlayerPrefs.SetInt(MUSIC_ENABLED, enabled ? 1 : 0);
        PlayerPrefs.Save();
        AudioManager.Instance?.ToggleMusic(enabled);
    }

    private void OnSFXToggleChanged(bool enabled)
    {
        PlayerPrefs.SetInt(SFX_ENABLED, enabled ? 1 : 0);
        PlayerPrefs.Save();
        AudioManager.Instance?.ToggleSFX(enabled);
    }

    private void OnNotificationsToggleChanged(bool enabled)
    {
        PlayerPrefs.SetInt(NOTIFICATIONS_ENABLED, enabled ? 1 : 0);
        PlayerPrefs.Save();
    }

    private void OnResetProgressClicked()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        GameManager.Instance?.ReturnToMenu();
    }

    private void OnCreditsClicked()
    {
        creditsPanel.SetActive(true);
    }

    private void OnCloseCreditsClicked()
    {
        creditsPanel.SetActive(false);
    }

    public void BackToMenu()
    {
        menuManager?.BackToMainMenu();
    }
}
