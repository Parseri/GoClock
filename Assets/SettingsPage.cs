using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsPage : MonoBehaviour {
    private static SettingsPage instance = null;
    private bool settingsChanged = true;
    public bool SettingsChanged => settingsChanged;

    private bool firstOpen;
    public Transform presetParent;
    public GameObject presetPrefab;

    public InputField mainMinutes;
    public InputField mainSeconds;
    public InputField fischerMinutes;
    public InputField fischerSeconds;
    public InputField beepMinutes;
    public InputField beepSeconds;
    public InputField japPeriods;
    public InputField japMinutes;
    public InputField japSeconds;
    public Toggle canAddTimeToggle;
    public Toggle manttoniSoundToggle;
    public Toggle clickSoundToggle;
    public Toggle themeToggle;
    private bool msEnabled = false;
    private bool canAddTime = true;
    private bool clickSoundEnabled = true;
    private bool presetCreated = false;
    public bool ClickSoundEnabled => clickSoundEnabled;

    public bool CanAddTime => canAddTime;

    private ClockLogic.TimeSettings timeSettings;

    private List<ClockLogic.TimeSettings> customPresets;

    public GameObject Dimmer;
    private string themeName;
    public string ThemeName => themeName;

    public TMP_Text deleteButtonText;

    void Start() {
        if (instance == null) instance = this;
        else return;
        firstOpen = true;
        if (PlayerPrefs.HasKey("SAVED_TIME")) {
            var str = PlayerPrefs.GetString("SAVED_TIME");
            timeSettings = JsonConvert.DeserializeObject<ClockLogic.TimeSettings>(str);
        } else {
            timeSettings = new ClockLogic.TimeSettings(5, 0, 0, 10, 0, 5, 0, 0, 30);
        }
        if (PlayerPrefs.HasKey("PRESETS")) {
            customPresets = JsonConvert.DeserializeObject<List<ClockLogic.TimeSettings>>(PlayerPrefs.GetString("PRESETS"));
            foreach (var p in customPresets) {
                CreatePresetButton(p);
            }
        } else {
            customPresets = new List<ClockLogic.TimeSettings>();
        }
        if (PlayerPrefs.HasKey("THEME")) {
            themeName = PlayerPrefs.GetString("THEME");
        }
        themeToggle.isOn = !string.IsNullOrEmpty(themeName);
        canAddTime = PlayerPrefs.GetInt("CAN_ADD_TIME", 0) == 1;
        canAddTimeToggle.isOn = canAddTime;
        msEnabled = PlayerPrefs.GetInt("MANTTONI_SOUND", 0) == 1;
        manttoniSoundToggle.isOn = msEnabled;
        clickSoundEnabled = PlayerPrefs.GetInt("CLICK_SOUND", 0) == 1;
        clickSoundToggle.isOn = clickSoundEnabled;
        mainMinutes.text = timeSettings.mainMins.ToString();
        mainSeconds.text = timeSettings.mainSecs.ToString();
        fischerMinutes.text = timeSettings.fischerMins.ToString();
        fischerSeconds.text = timeSettings.fischerSecs.ToString();
        beepMinutes.text = timeSettings.beepMins.ToString();
        beepSeconds.text = timeSettings.beepSecs.ToString();
        japPeriods.text = timeSettings.japPer.ToString();
        japMinutes.text = timeSettings.japMins.ToString();
        japSeconds.text = timeSettings.japSecs.ToString();

    }

    private bool ParseCurrentSettings() {
        float value;
        settingsChanged = false;
        var temp = new ClockLogic.TimeSettings();
        if (string.IsNullOrEmpty(mainMinutes.text)) mainMinutes.text = "0";
        if (float.TryParse(mainMinutes.text, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture.NumberFormat, out value)) {
            settingsChanged = settingsChanged || timeSettings.mainMins != value;
            temp.mainMins = value;
        } else {
            mainMinutes.Select();
            return false;
        }
        if (string.IsNullOrEmpty(mainSeconds.text)) mainSeconds.text = "0";
        if (float.TryParse(mainSeconds.text, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture.NumberFormat, out value)) {
            settingsChanged = settingsChanged || timeSettings.mainSecs != value;
            temp.mainSecs = value;
        } else {
            mainSeconds.Select();
            return false;
        }
        if (string.IsNullOrEmpty(fischerMinutes.text)) fischerMinutes.text = "0";
        if (float.TryParse(fischerMinutes.text, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture.NumberFormat, out value)) {
            settingsChanged = settingsChanged || timeSettings.fischerMins != value;
            temp.fischerMins = value;
        } else {
            fischerMinutes.Select();
            return false;
        }
        if (string.IsNullOrEmpty(fischerSeconds.text)) fischerSeconds.text = "0";
        if (float.TryParse(fischerSeconds.text, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture.NumberFormat, out value)) {
            settingsChanged = settingsChanged || timeSettings.fischerSecs != value;
            temp.fischerSecs = value;
        } else {
            fischerSeconds.Select();
            return false;
        }
        if (string.IsNullOrEmpty(beepMinutes.text)) beepMinutes.text = "0";
        if (float.TryParse(beepMinutes.text, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture.NumberFormat, out value)) {
            temp.beepMins = value;
        } else {
            beepMinutes.Select();
            return false;
        }
        if (string.IsNullOrEmpty(beepSeconds.text)) beepSeconds.text = "0";
        if (float.TryParse(beepSeconds.text, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture.NumberFormat, out value)) {
            temp.beepSecs = value;
        } else {
            beepSeconds.Select();
            return false;
        }
        if (string.IsNullOrEmpty(japMinutes.text)) japMinutes.text = "0";
        if (float.TryParse(japMinutes.text, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture.NumberFormat, out value)) {
            settingsChanged = settingsChanged || timeSettings.japMins != value;
            temp.japMins = value;
        } else {
            japMinutes.Select();
            return false;
        }
        if (string.IsNullOrEmpty(japSeconds.text)) japSeconds.text = "0";
        if (float.TryParse(japSeconds.text, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture.NumberFormat, out value)) {
            settingsChanged = settingsChanged || timeSettings.japSecs != value;
            temp.japSecs = value;
        } else {
            japSeconds.Select();
            return false;
        }
        int val;
        if (string.IsNullOrEmpty(japPeriods.text)) japPeriods.text = "0";
        if (int.TryParse(japPeriods.text, NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat, out val)) {
            settingsChanged = settingsChanged || timeSettings.japPer != val;
            temp.japPer = val;
        } else {
            japPeriods.Select();
            return false;
        }
        if (temp.japPer > 0 && temp.japMins <= 0 && temp.japSecs <= 0) {
            japPeriods.Select();
            return false;
        }
        timeSettings = temp;

        return true;
    }

    public static bool MsEnabled() {
        return instance.msEnabled;
    }

    public bool CloseSettings() {
        if (Dimmer.activeSelf) {
            Dimmer.SetActive(false);
            deleteButtonText.text = "Delete Preset";
            return false;
        }
        if (ParseCurrentSettings()) {
            if (settingsChanged) {
                Debug.Log("SettingsChanged: pers: " + timeSettings.japPer);
                var str = JsonConvert.SerializeObject(timeSettings);
                PlayerPrefs.SetString("SAVED_TIME", str);
            }
            settingsChanged |= firstOpen || presetCreated;
            firstOpen = false;
            gameObject.SetActive(false);
            presetCreated = false;
            return true;
        } else return false;
    }

    public void DeletePreset() {
        if (Dimmer.activeSelf) {
            Dimmer.SetActive(false);
            deleteButtonText.text = "Delete Preset";
        } else {
            Dimmer.SetActive(true);
            deleteButtonText.text = "Cancel";
        }
    }

    private void ApplyTimeSetting(ClockLogic.TimeSettings setting) {
        if (Dimmer.activeSelf) {
            for (int i = 0; i < customPresets.Count; ++i) {
                if (customPresets[i].IsSame(setting)) {
                    customPresets.RemoveAt(i);
                    break;
                }
            }
            DeletePresetButton(setting);
            SaveCustomPresets();
        } else {
            mainMinutes.text = setting.mainMins.ToString();
            mainSeconds.text = setting.mainSecs.ToString();
            fischerMinutes.text = setting.fischerMins.ToString();
            fischerSeconds.text = setting.fischerSecs.ToString();
            beepMinutes.text = setting.beepMins.ToString();
            beepSeconds.text = setting.beepSecs.ToString();
            japPeriods.text = setting.japPer.ToString();
            japMinutes.text = setting.japMins.ToString();
            japSeconds.text = setting.japSecs.ToString();
        }
    }

    public void EnableManttoniSound(bool enabled) {
        msEnabled = enabled;
        PlayerPrefs.SetInt("MANTTONI_SOUND", enabled ? 1 : 0);
    }

    public void EnableCanAddTime(bool enabled) {
        canAddTime = enabled;
        PlayerPrefs.SetInt("CAN_ADD_TIME", enabled ? 1 : 0);
    }

    public void EnableClickSound(bool enabled) {
        clickSoundEnabled = enabled;
        PlayerPrefs.SetInt("CLICK_SOUND", enabled ? 1 : 0);
    }

    public void EnableBubbleThemeSound(bool enabled) {
        themeName = enabled ? "bubbleTheme" : null;
        PlayerPrefs.SetString("THEME", themeName);
    }

    public static void s_ApplyTimeSetting(ClockLogic.TimeSettings setting) {
        instance.ApplyTimeSetting(setting);
    }

    public void CreatePreset() {
        if (ParseCurrentSettings()) {
            var ts = new ClockLogic.TimeSettings(timeSettings);
            foreach (var p in customPresets)
                if (p.IsSame(ts)) return;
            customPresets.Add(ts);
            SaveCustomPresets();
            CreatePresetButton(ts);
            presetCreated = true;
        }
    }

    private void SaveCustomPresets() {
        var str = JsonConvert.SerializeObject(customPresets);
        PlayerPrefs.SetString("PRESETS", str);
    }

    private void DeletePresetButton(ClockLogic.TimeSettings ts) {
        for (int i = 0; i < presetParent.childCount; ++i) {
            var go = presetParent.GetChild(i);
            var p = go.GetComponent<Preset>();
            if (p.settings.IsSame(ts)) {
                Destroy(go.gameObject);
                return;
            }
        }
    }

    private void CreatePresetButton(ClockLogic.TimeSettings ts) {
        var go = GameObject.Instantiate(presetPrefab, presetParent);
        var pr = go.GetComponent<Preset>();
        pr.settings = ts;
        pr.title.text = ts.GetNameString();
    }

    public ClockLogic.TimeSettings GetTimeSettings() {
        return timeSettings;
    }
}
