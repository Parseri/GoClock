using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Preset : MonoBehaviour {
    public string presetName;
    public TMP_Text title;
    public ClockLogic.TimeSettings settings;

    public void PresetClicked() {
        SettingsPage.s_ApplyTimeSetting(settings);
    }
}
