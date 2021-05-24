using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

public class SettingsPage : MonoBehaviour {
    private bool settingsChanged = true;
    public bool SettingsChanged => settingsChanged;

    private bool firstOpen;

    public InputField mainMinutes;
    public InputField mainSeconds;
    public InputField fischerMinutes;
    public InputField fischerSeconds;
    public InputField beepMinutes;
    public InputField beepSeconds;
    public InputField japPeriods;
    public InputField japMinutes;
    public InputField japSeconds;

    private ClockLogic.TimeSettings timeSettings;

    void Start() {
        firstOpen = true;
        if (PlayerPrefs.HasKey("SAVED_TIME")) {
            var str = PlayerPrefs.GetString("SAVED_TIME");
            timeSettings = JsonConvert.DeserializeObject<ClockLogic.TimeSettings>(str);
        } else {
            timeSettings = new ClockLogic.TimeSettings(5, 0, 0, 10, 0, 5, 0, 0, 30);
        }
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
    public bool CloseSettings() {
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
            settingsChanged = settingsChanged || timeSettings.beepMins != value;
            timeSettings.beepMins = value;
        } else {
            beepMinutes.Select();
            return false;
        }
        if (float.TryParse(beepSeconds.text, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture.NumberFormat, out value)) {
            settingsChanged = settingsChanged || timeSettings.beepSecs != value;
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
        if (settingsChanged) {
            Debug.Log("SettingsChanged: pers: " + timeSettings.japPer);
            var str = JsonConvert.SerializeObject(timeSettings);
            PlayerPrefs.SetString("SAVED_TIME", str);
        }
        settingsChanged |= firstOpen;
        firstOpen = false;
        gameObject.SetActive(false);
        return true;
    }

    public ClockLogic.TimeSettings GetTimeSettings() {
        return timeSettings;
    }
}