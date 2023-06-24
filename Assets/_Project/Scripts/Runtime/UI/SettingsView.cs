using DG.Tweening;
using FMOD.Studio;
using FMODUnity;
using System;
using UnityEngine;
using UnityEngine.UI;

public class SettingsView : MonoBehaviour
{
    [SerializeField] private Button settingsButton;
    [SerializeField] private RectTransform panel;
    [SerializeField] private GameObject clickBlocker;
    [Space]
    [SerializeField] private ToggleButton fullscreenToggle;
    [SerializeField] private ToggleButton muteToggle;
    [SerializeField] private Slider mainVolumeSlider;
    [SerializeField] private Slider ambientVolumeSlider;
    [SerializeField] private Slider effectsVolumeSlider;
    [SerializeField] private Slider uiVolumeSlider;
    [Space]
    [SerializeField] private StudioEventEmitter swipeSoundEvent;

    private bool isOpen;
    private Bus audioBus;
    private Bus audioBusAmbient;
    private Bus audioBusEffects;
    private Bus audioBusUI;

    private void Awake()
    {
        audioBus = RuntimeManager.GetBus("bus:/");
        audioBusAmbient = RuntimeManager.GetBus("bus:/Ambient");
        audioBusEffects = RuntimeManager.GetBus("bus:/Effects");
        audioBusUI = RuntimeManager.GetBus("bus:/UI");

        clickBlocker.SetActive(false);
        panel.gameObject.SetActive(false);
        panel.anchorMin = new(1, 0);
        panel.anchorMax = new(2, 1);

        settingsButton.onClick.AddListener(ToggleSettingsPanel);

        var savedFullscreenSetting = LoadBool("Fullscreen", false);
        var savedMuteSettings = LoadBool("AudioMuted", true);
        var savedMainVolume = LoadFloat("Volume-Main", 0.5f);
        var savedAmbientVolume = LoadFloat("Volume-Ambient", 1);
        var savedEffectsVolume = LoadFloat("Volume-Effects", 1);
        var savedUIVolume = LoadFloat("Volume-UI", 1);

        SetFullscreenBoilerplate(savedFullscreenSetting);
        audioBus.setMute(savedMuteSettings);
        audioBus.setVolume(savedMainVolume);
        audioBusAmbient.setVolume(savedAmbientVolume);
        audioBusEffects.setVolume(savedEffectsVolume);
        audioBusUI.setVolume(savedUIVolume);


        fullscreenToggle.SetState(savedFullscreenSetting);
        fullscreenToggle.OnChanged += v =>
        {
            SetFullscreenBoilerplate(v);
            Save("Fullscreen", v);
        };

        muteToggle.SetState(savedMuteSettings);
        muteToggle.OnChanged += v =>
        {
            audioBus.setMute(v);
            Save("AudioMuted", v);
        };

        mainVolumeSlider.value = savedMainVolume;
        mainVolumeSlider.onValueChanged.AddListener(v =>
        {
            audioBus.setVolume(v);
            Save("Volume-Main", v);
        });

        ambientVolumeSlider.value = savedAmbientVolume;
        ambientVolumeSlider.onValueChanged.AddListener(v =>
        {
            audioBusAmbient.setVolume(v);
            Save("Volume-Ambient", v);
        });

        effectsVolumeSlider.value = savedEffectsVolume;
        effectsVolumeSlider.onValueChanged.AddListener(v =>
        {
            audioBusEffects.setVolume(v);
            Save("Volume-Effects", v);
        });

        uiVolumeSlider.value = savedUIVolume;
        uiVolumeSlider.onValueChanged.AddListener(v =>
        {
            audioBusUI.setVolume(v);
            Save("Volume-UI", v);
        });
    }

    private static void SetFullscreenBoilerplate(bool v)
    {
        if (v)
        {
            var info = Screen.mainWindowDisplayInfo;
            Screen.SetResolution(info.width, info.height, FullScreenMode.FullScreenWindow);
        }
        else
        {
            var info = Screen.mainWindowDisplayInfo;
            Screen.SetResolution(info.width / 2, info.height / 2, FullScreenMode.Windowed);
        }
    }

    private void ToggleSettingsPanel()
    {
        if (isOpen)
        {
            ClosePanel();
        }
        else
        {
            OpenPanel();
        }
    }

    public void OpenPanel()
    {
        if (isOpen) return;
        isOpen = true;

        if (swipeSoundEvent != null) swipeSoundEvent.Play();

        panel.DOKill();
        panel.gameObject.SetActive(true);
        clickBlocker.SetActive(true);
        panel.DOAnchorMin(new(0, 0), 0.5f);
        panel.DOAnchorMax(new(1, 1), 0.5f);
    }

    public void ClosePanel()
    {
        if (!isOpen) return;
        isOpen = false;

        if (swipeSoundEvent != null) swipeSoundEvent.Play();

        panel.DOAnchorMin(new(1, 0), 0.5f);
        panel.DOAnchorMax(new(2, 1), 0.5f)
            .OnComplete(() =>
            {
                panel.gameObject.SetActive(false);
                clickBlocker.SetActive(false);
            });
    }

    private bool LoadBool(string key, bool defaultValue)
    {
        return PlayerPrefs.GetInt(key, defaultValue ? 1 : 0) != 0;
    }
    private void Save(string key, bool value)
    {
        PlayerPrefs.SetInt(key, value ? 1 : 0);
    }
    private float LoadFloat(string key, float defaultValue)
    {
        return PlayerPrefs.GetFloat(key, defaultValue);
    }
    private void Save(string key, float value)
    {
        PlayerPrefs.SetFloat(key, value);
    }
}
