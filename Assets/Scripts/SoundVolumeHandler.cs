using System.Collections;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Parseri.GoClock
{
    public class SoundVolumeHandler : MonoBehaviour
    {
        [SerializeField] private Slider soundSlider;
        [SerializeField] private TMP_InputField soundInput;
        [SerializeField] private Image sliderImage;

        [SerializeField] private Color neutralColor;
        [SerializeField] private Color highVolumeColor;

        private float _prevValue;



        void Start()
        {
            StartCoroutine(InitValues());
        }

        IEnumerator InitValues()
        {
            while (!PlayerPrefs.HasKey("MASTER_VOLUME")) yield return null;
            float soundOverride = PlayerPrefs.GetFloat("MASTER_VOLUME", 1f);
            soundSlider.SetValueWithoutNotify(soundOverride);
            soundInput.SetTextWithoutNotify(soundOverride.ToString("P0"));
            _prevValue = soundSlider.value;
            ChangeSliderColor(_prevValue);
        }

        public void OnSliderValueChanged(float value)
        {
            PlayerPrefs.SetFloat("MASTER_VOLUME", value);
            soundInput.SetTextWithoutNotify(value.ToString("P0"));
            AudioManager.SetMasterVolume(value);
        }

        private void ChangeSliderColor(float value)
        {
            if (value > 0.85f)
            {
                sliderImage.color = Color.Lerp(neutralColor, highVolumeColor, (value - 0.85f) / 0.65f);
            }
            else
            {
                sliderImage.color = neutralColor;
            }
        }

        private void Update()
        {
            if (_prevValue == soundSlider.value) return;

            _prevValue = soundSlider.value;
            ChangeSliderColor(_prevValue);
        }

        public void OnPitchEntered(string value)
        {
            //Inserted as percentages xxx%
            if (int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture.NumberFormat, out int parsedValue))
            {
                if (parsedValue > 150f) parsedValue = 1;
                if (parsedValue < 0f) parsedValue = 0;
                var percentage = parsedValue / 100f;
                PlayerPrefs.SetFloat("SoundPitch", percentage);
                AudioManager.SetMasterVolume(percentage);
                soundSlider.SetValueWithoutNotify(percentage);
                soundInput.SetTextWithoutNotify(percentage.ToString("P0"));
            }
        }
    }


    //float.Parse(soundOverride, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture.NumberFormat)
}