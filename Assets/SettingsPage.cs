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
    public Toggle manttoniSoundToggle;
    public bool msEnabled = false;

    private ClockLogic.TimeSettings timeSettings;

    private List<ClockLogic.TimeSettings> customPresets;

    public GameObject Dimmer;
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
        msEnabled = PlayerPrefs.GetInt("MANTTONI_SOUND", 0) == 1;
        manttoniSoundToggle.isOn = msEnabled;
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
        if (float.TryParse(mainMinutes.text, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture.NumberFormat, out value)) {
            settingsChanged = settingsChanged || timeSettings.mainMins != value;
            timeSettings.mainMins = value;
        } else {
            mainMinutes.Select();
            return false;
        }
        if (float.TryParse(mainSeconds.text, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture.NumberFormat, out value)) {
            settingsChanged = settingsChanged || timeSettings.mainSecs != value;
            timeSettings.mainSecs = value;
        } else {
            mainSeconds.Select();
            return false;
        }
        if (float.TryParse(fischerMinutes.text, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture.NumberFormat, out value)) {
            settingsChanged = settingsChanged || timeSettings.fischerMins != value;
            timeSettings.fischerMins = value;
        } else {
            fischerMinutes.Select();
            return false;
        }
        if (float.TryParse(fischerSeconds.text, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture.NumberFormat, out value)) {
            settingsChanged = settingsChanged || timeSettings.fischerSecs != value;
            timeSettings.fischerSecs = value;
        } else {
            fischerSeconds.Select();
            return false;
        }
        if (float.TryParse(beepMinutes.text, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture.NumberFormat, out value)) {
            timeSettings.beepMins = value;
        } else {
            beepMinutes.Select();
            return false;
        }
        if (float.TryParse(beepSeconds.text, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture.NumberFormat, out value)) {
            timeSettings.beepSecs = value;
        } else {
            beepSeconds.Select();
            return false;
        }
        if (float.TryParse(japMinutes.text, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture.NumberFormat, out value)) {
            settingsChanged = settingsChanged || timeSettings.japMins != value;
            timeSettings.japMins = value;
        } else {
            japMinutes.Select();
            return false;
        }
        if (float.TryParse(japSeconds.text, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture.NumberFormat, out value)) {
            settingsChanged = settingsChanged || timeSettings.japSecs != value;
            timeSettings.japSecs = value;
        } else {
            japSeconds.Select();
            return false;
        }
        int val;
        if (int.TryParse(japPeriods.text, NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat, out val)) {
            settingsChanged = settingsChanged || timeSettings.japPer != val;
            timeSettings.japPer = val;
        } else {
            japPeriods.Select();
            return false;
        }
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
            settingsChanged |= firstOpen;
            firstOpen = false;
            gameObject.SetActive(false);
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

    public void ManttoniSoundEnabled(bool enabled) {
        msEnabled = enabled;
        PlayerPrefs.SetInt("MANTTONI_SOUND", enabled ? 1 : 0);
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