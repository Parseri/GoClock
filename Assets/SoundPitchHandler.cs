using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SoundPitchHandler : MonoBehaviour {
    public Toggle overrideSoundToggle;
    public Slider soundSlider;
    public TMP_InputField soundInput;
    void Start() {
        InitValues();
    }

    void InitValues() {
        bool isOverride = PlayerPrefs.GetInt("OverrideSoundPitch", 0) == 1;
        float soundOverride = PlayerPrefs.GetFloat("SoundPitch", 5f);
        overrideSoundToggle.isOn = isOverride;
        soundSlider.value = isOverride ? soundOverride : 5f;
        soundSlider.interactable = isOverride;
        soundInput.text = isOverride ? soundOverride.ToString() : "5";
        soundInput.interactable = isOverride;
    }

    public void OnOverrideToggleChanged(bool enabled) {
        PlayerPrefs.SetInt("OverrideSoundPitch", enabled ? 1 : 0);
        InitValues();
    }

    public void OnSliderValueChanged(float value) {
        PlayerPrefs.SetFloat("SoundPitch", value);
        soundInput.text = value.ToString();
    }

    public void OnPitchEntered(string value) {
        float parsedValue;
        if (float.TryParse(value, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture.NumberFormat, out parsedValue)) {
            if (parsedValue >= 10f) parsedValue = 10;
            if (parsedValue < 0f) parsedValue = 0;
            PlayerPrefs.SetFloat("SoundPitch", parsedValue);
            soundSlider.value = parsedValue;
        }
    }
}


//float.Parse(soundOverride, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture.NumberFormat)
