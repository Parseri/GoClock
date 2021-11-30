using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsPage : MonoBehaviour {
    [Serializable]
    public class PlayerTimeInput {
        public GameObject inputParent;
        public InputField mainMinutes;
        public InputField mainSeconds;
        public InputField fischerMinutes;
        public InputField fischerSeconds;
        public InputField japPeriods;
        public InputField japMinutes;
        public InputField japSeconds;

        public void InitValues(ClockLogic.TimeSettings s) {
            mainMinutes.text = s.mainMins.ToString();
            mainSeconds.text = s.mainSecs.ToString();
            fischerMinutes.text = s.fischerMins.ToString();
            fischerSeconds.text = s.fischerSecs.ToString();
            japPeriods.text = s.japPer.ToString();
            japMinutes.text = s.japMins.ToString();
            japSeconds.text = s.japSecs.ToString();
        }

        public ClockLogic.TimeSettings ParseTimeSettings(ClockLogic.TimeSettings current, out bool settingsChanged) {
            var temp = new ClockLogic.TimeSettings();
            settingsChanged = false;
            float value;
            if (string.IsNullOrEmpty(mainMinutes.text)) mainMinutes.text = "0";
            if (float.TryParse(mainMinutes.text, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture.NumberFormat, out value)) {
                settingsChanged = settingsChanged || current.mainMins != value;
                temp.mainMins = value;
            } else {
                mainMinutes.Select();
                return null;
            }
            if (string.IsNullOrEmpty(mainSeconds.text)) mainSeconds.text = "0";
            if (float.TryParse(mainSeconds.text, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture.NumberFormat, out value)) {
                settingsChanged = settingsChanged || current.mainSecs != value;
                temp.mainSecs = value;
            } else {
                mainSeconds.Select();
                return null;
            }
            if (string.IsNullOrEmpty(fischerMinutes.text)) fischerMinutes.text = "0";
            if (float.TryParse(fischerMinutes.text, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture.NumberFormat, out value)) {
                settingsChanged = settingsChanged || current.fischerMins != value;
                temp.fischerMins = value;
            } else {
                fischerMinutes.Select();
                return null;
            }
            if (string.IsNullOrEmpty(fischerSeconds.text)) fischerSeconds.text = "0";
            if (float.TryParse(fischerSeconds.text, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture.NumberFormat, out value)) {
                settingsChanged = settingsChanged || current.fischerSecs != value;
                temp.fischerSecs = value;
            } else {
                fischerSeconds.Select();
                return null;
            }
            if (string.IsNullOrEmpty(japMinutes.text)) japMinutes.text = "0";
            if (float.TryParse(japMinutes.text, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture.NumberFormat, out value)) {
                settingsChanged = settingsChanged || current.japMins != value;
                temp.japMins = value;
            } else {
                japMinutes.Select();
                return null;
            }
            if (string.IsNullOrEmpty(japSeconds.text)) japSeconds.text = "0";
            if (float.TryParse(japSeconds.text, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture.NumberFormat, out value)) {
                settingsChanged = settingsChanged || current.japSecs != value;
                temp.japSecs = value;
            } else {
                japSeconds.Select();
                return null;
            }
            int val;
            if (string.IsNullOrEmpty(japPeriods.text)) japPeriods.text = "0";
            if (int.TryParse(japPeriods.text, NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat, out val)) {
                settingsChanged = settingsChanged || current.japPer != val;
                temp.japPer = val;
            } else {
                japPeriods.Select();
                return null;
            }
            if (temp.japPer > 0 && temp.japMins <= 0 && temp.japSecs <= 0) {
                japPeriods.Select();
                return null;
            }
            return temp;
        }
    }
    [Serializable]
    public class TimePreset {
        [SerializeField]
        public ClockLogic.TimeSettings p1Settings;
        [SerializeField]
        public ClockLogic.TimeSettings p2Settings;
        [SerializeField]
        public ClockLogic.CommonTimeSettings commonSettings;

        public TimePreset() {

        }

        public TimePreset(ClockLogic.TimeSettings p1, ClockLogic.TimeSettings p2, ClockLogic.CommonTimeSettings common) {
            p1Settings = p1;
            p2Settings = p2;
            commonSettings = common;
        }

        public bool IsSame(TimePreset o) {
            return p1Settings.IsSame(o.p1Settings) && p2Settings.IsSame(o.p2Settings) && commonSettings.IsSame(o.commonSettings);
        }

        public string GetNameString() {
            return string.Format("{0:00}:{1:00}", p1Settings.mainMins, p1Settings.mainSecs) + "+" + (p1Settings.fischerMins * 60f + p1Settings.fischerSecs) + "s" + (p1Settings.japPer > 0 ? " " + p1Settings.japPer + "x" + string.Format("{0:00}:{1:00}", p1Settings.japMins, p1Settings.japSecs) : "") + "\n"
            + (!commonSettings.useSameTime ? string.Format("{0:00}:{1:00}", p2Settings.mainMins, p2Settings.mainSecs) + "+" + (p2Settings.fischerMins * 60f + p2Settings.fischerSecs) + "s" + (p2Settings.japPer > 0 ? " " + p2Settings.japPer + "x" + string.Format("{0:00}:{1:00}", p2Settings.japMins, p2Settings.japSecs) : "") : "");
        }
    }

    private static SettingsPage instance = null;
    private bool settingsChanged = true;
    public bool SettingsChanged => settingsChanged;
    [SerializeField]
    private PlayerTimeInput p1Inputs;
    [SerializeField]
    private PlayerTimeInput p2Inputs;

    private bool firstOpen;
    public Transform presetParent;
    public GameObject presetPrefab;
    public Toggle useSameTimeSettingToggle;
    public Toggle canAddTimeToggle;
    public Toggle manttoniSoundToggle;
    public Toggle clickSoundToggle;
    public Toggle themeToggle;
    public InputField beepMinutes;
    public InputField beepSeconds;

    private bool msEnabled = false;
    private bool useSameSettingsChanged = false;
    private bool canAddTime = true;
    private bool clickSoundEnabled = true;
    private bool presetCreated = false;
    public bool ClickSoundEnabled => clickSoundEnabled;

    public bool CanAddTime => canAddTime;
    private ClockLogic.CommonTimeSettings commonTimeSettings;
    private ClockLogic.TimeSettings p1TimeSettings;
    private ClockLogic.TimeSettings p2TimeSettings;

    private List<TimePreset> customPresets;

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
            var dep = JsonConvert.DeserializeObject<ClockLogic.DeprecatedTimeSettings>(str);
            p1TimeSettings = new ClockLogic.TimeSettings(dep.mainMins, dep.mainSecs, dep.fischerMins, dep.fischerSecs, dep.japPer, dep.japMins, dep.japSecs);
            p2TimeSettings = p1TimeSettings;
            commonTimeSettings = new ClockLogic.CommonTimeSettings(dep.beepMins, dep.beepSecs, true);
            PlayerPrefs.DeleteKey("SAVED_TIME");
        } else if (PlayerPrefs.HasKey("SAVED_TIME_v2")) {
            var str = PlayerPrefs.GetString("SAVED_TIME_v2");
            var tsPair = JsonConvert.DeserializeObject<TimePreset>(str);
            p1TimeSettings = tsPair.p1Settings;
            p2TimeSettings = tsPair.p2Settings;
            commonTimeSettings = tsPair.commonSettings;
        } else {
            p1TimeSettings = new ClockLogic.TimeSettings(5, 0, 0, 10, 0, 0, 30);
            p2TimeSettings = p1TimeSettings;
            commonTimeSettings = new ClockLogic.CommonTimeSettings(0, 5, true);
        }
        if (PlayerPrefs.HasKey("PRESETS")) {
            var depPresets = JsonConvert.DeserializeObject<List<ClockLogic.DeprecatedTimeSettings>>(PlayerPrefs.GetString("PRESETS"));
            foreach (var dp in depPresets) {

            }
            PlayerPrefs.DeleteKey("PRESETS");
        }
        if (PlayerPrefs.HasKey("PRESETS_v2")) {
            customPresets = JsonConvert.DeserializeObject<List<TimePreset>>(PlayerPrefs.GetString("PRESETS_v2"));
            foreach (var p in customPresets) {
                CreatePresetButton(p);
            }
        } else {
            customPresets = new List<TimePreset>();
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
        p1Inputs.InitValues(p1TimeSettings);
        p2Inputs.InitValues(p2TimeSettings);
        useSameSettingsChanged = false;
        useSameTimeSettingToggle.isOn = commonTimeSettings.useSameTime;
        p2Inputs.inputParent.SetActive(!commonTimeSettings.useSameTime);
        beepMinutes.text = commonTimeSettings.beepMins.ToString();
        beepSeconds.text = commonTimeSettings.beepSecs.ToString();
    }

    private bool ParseCurrentSettings() {
        settingsChanged = false;
        var temp = p1Inputs.ParseTimeSettings(p1TimeSettings, out settingsChanged);
        if (temp != null) {
            p1TimeSettings = temp;
        } else
            return false;
        temp = p2Inputs.ParseTimeSettings(p2TimeSettings, out settingsChanged);
        if (temp != null) {
            p2TimeSettings = temp;
        } else
            return false;
        float value;
        if (string.IsNullOrEmpty(beepMinutes.text)) beepMinutes.text = "0";
        if (float.TryParse(beepMinutes.text, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture.NumberFormat, out value)) {
            commonTimeSettings.beepMins = value;
        } else {
            beepMinutes.Select();
            return false;
        }
        if (string.IsNullOrEmpty(beepSeconds.text)) beepSeconds.text = "0";
        if (float.TryParse(beepSeconds.text, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture.NumberFormat, out value)) {
            commonTimeSettings.beepSecs = value;
        } else {
            beepSeconds.Select();
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
                var str = JsonConvert.SerializeObject(new TimePreset(p1TimeSettings, p2TimeSettings, commonTimeSettings));
                PlayerPrefs.SetString("SAVED_TIME_v2", str);
            }
            settingsChanged |= firstOpen || presetCreated || useSameSettingsChanged;
            useSameSettingsChanged = false;
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

    private void ApplyTimeSetting(TimePreset setting) {
        //trying to remove preset?
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
            commonTimeSettings = new ClockLogic.CommonTimeSettings(setting.commonSettings.beepMins, setting.commonSettings.beepSecs, setting.commonSettings.useSameTime);
            p1TimeSettings = new ClockLogic.TimeSettings(setting.p1Settings);
            p2TimeSettings = new ClockLogic.TimeSettings(setting.p2Settings);
            p1Inputs.InitValues(p1TimeSettings);
            p2Inputs.InitValues(p2TimeSettings);
            useSameTimeSettingToggle.isOn = commonTimeSettings.useSameTime;
            p2Inputs.inputParent.SetActive(!commonTimeSettings.useSameTime);
            beepMinutes.text = commonTimeSettings.beepMins.ToString();
            beepSeconds.text = commonTimeSettings.beepSecs.ToString();
        }
    }

    public void EnableManttoniSound(bool enabled) {
        msEnabled = enabled;
        PlayerPrefs.SetInt("MANTTONI_SOUND", enabled ? 1 : 0);
    }

    public void UseSameTimeSettingsClicked(bool enabled) {
        commonTimeSettings.useSameTime = enabled;
        p2Inputs.inputParent.SetActive(!enabled);
        useSameSettingsChanged = true;
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

    public static void s_ApplyTimeSetting(TimePreset setting) {
        instance.ApplyTimeSetting(setting);
    }

    public void CreatePreset() {
        if (ParseCurrentSettings()) {
            var ts = new TimePreset(p1TimeSettings, p2TimeSettings, commonTimeSettings);
            foreach (var p in customPresets)
                if (p.IsSame(ts)) return;
            customPresets.Add(ts);
            SaveCustomPresets();
            CreatePresetButton(ts);
            presetCreated = true;
            LayoutRebuilder.ForceRebuildLayoutImmediate(presetParent.GetComponent<RectTransform>());
        }
    }

    private void SaveCustomPresets() {
        var str = JsonConvert.SerializeObject(customPresets);
        PlayerPrefs.SetString("PRESETS_v2", str);
    }

    private void DeletePresetButton(TimePreset ts) {
        for (int i = 0; i < presetParent.childCount; ++i) {
            var go = presetParent.GetChild(i);
            var p = go.GetComponent<PresetV2>();
            if (p.IsSame(ts) && !p.defaultTime) {
                Destroy(go.gameObject);
                return;
            }
        }
    }

    private void CreatePresetButton(TimePreset ts) {
        var go = GameObject.Instantiate(presetPrefab, presetParent);
        var pr = go.GetComponent<PresetV2>();
        pr.p1Settings = ts.p1Settings;
        pr.p2Settings = ts.p2Settings;
        pr.commonTimeSettings = ts.commonSettings;
        pr.defaultTime = false;
        pr.title.text = ts.GetNameString();
    }

    public TimePreset GetTimeSettings() {
        return new TimePreset(p1TimeSettings, p2TimeSettings, commonTimeSettings);
    }
}
