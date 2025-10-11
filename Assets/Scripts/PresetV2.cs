using TMPro;
using UnityEngine;

namespace Parseri.GoClock
{
    public class PresetV2 : MonoBehaviour
    {
        public string presetName;
        public TMP_Text title;
        public ClockLogic.TimeSettings p1Settings;
        public ClockLogic.TimeSettings p2Settings;
        public ClockLogic.CommonTimeSettings commonTimeSettings;
        public bool defaultTime = true;

        public bool IsSame(SettingsPage.TimePreset o)
        {
            return p1Settings.IsSame(o.p1Settings) && p2Settings.IsSame(o.p2Settings) && commonTimeSettings.IsSame(o.commonSettings);
        }

        public void PresetClicked()
        {
            SettingsPage.ApplyPreset(new SettingsPage.TimePreset(p1Settings, p2Settings, commonTimeSettings));
        }
    }
}