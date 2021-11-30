using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
public class ClockLogic : MonoBehaviour {

    [Serializable]
    public class Theme {
        public Sprite bigButton;
        public Sprite pauseButton;
        public Sprite settingsButton;
        public Sprite resetButton;
        public string themeName;
        public Color inactiveColor;
    }

    [Obsolete("Use per player TimeSettings instead.", false)]
    [Serializable]
    public class DeprecatedTimeSettings {
        [SerializeField]
        public float mainSecs;
        [SerializeField]
        public float mainMins;
        [SerializeField]
        public float fischerSecs;
        [SerializeField]
        public float fischerMins;
        [SerializeField]
        public int japPer;
        [SerializeField]
        public float japMins;
        [SerializeField]
        public float japSecs;
        [SerializeField]
        public float beepSecs;
        [SerializeField]
        public float beepMins;
        [JsonConstructor]
        public DeprecatedTimeSettings() {
        }
    }


    [Serializable]
    public class TimeSettings {
        [SerializeField]
        public float mainSecs;
        [SerializeField]
        public float mainMins;
        [SerializeField]
        public float fischerSecs;
        [SerializeField]
        public float fischerMins;
        [SerializeField]
        public int japPer;
        [SerializeField]
        public float japMins;
        [SerializeField]
        public float japSecs;
        [JsonConstructor]
        public TimeSettings() {
        }

        public TimeSettings(float mm, float ms, float fm, float fs, int jp = 0, float jm = 0, float js = 30) {
            mainMins = mm;
            mainSecs = ms;
            fischerMins = fm;
            fischerSecs = fs;
            japPer = jp;
            japMins = jm;
            japSecs = js;
        }

        public TimeSettings(TimeSettings other) {
            mainMins = other.mainMins;
            mainSecs = other.mainSecs;
            fischerMins = other.fischerMins;
            fischerSecs = other.fischerSecs;
            japMins = other.japMins;
            japSecs = other.japSecs;
            japPer = other.japPer;
        }

        public bool IsSame(TimeSettings o) {
            return mainMins == o.mainMins && mainSecs == o.mainSecs && fischerMins == o.fischerMins && fischerSecs == o.fischerSecs && japPer == o.japPer && japMins == o.japMins && japSecs == o.japSecs;
        }

        public string GetNameString() {
            return mainMins.ToString() + ":" + mainSecs.ToString() + "+" + fischerMins + ":" + fischerSecs + "+" + japPer + "x" + japMins + ":" + japSecs;
        }
    }

    [Serializable]
    public class CommonTimeSettings {
        [SerializeField]
        public bool useSameTime;
        [SerializeField]
        public float beepSecs;
        [SerializeField]
        public float beepMins;
        public CommonTimeSettings() {
            beepMins = 0;
            beepSecs = 5;
            useSameTime = true;
        }
        public CommonTimeSettings(float bm, float bs, bool useSame) {
            beepMins = bm;
            beepSecs = bs;
            useSameTime = useSame;
        }

        public bool IsSame(CommonTimeSettings o) {
            return useSameTime == o.useSameTime && beepMins == o.beepMins && beepSecs == o.beepSecs;
        }
    }

    private enum TurnPlayer {
        None,
        Player1,
        Player2
    }

    [SerializeField]
    public Image p1Button;
    [SerializeField]
    public Image p2Button;
    [SerializeField]
    public Image pauseButton;
    [SerializeField]
    public Image settingsButton;
    [SerializeField]
    public Image resetButton;
    private Color inactiveColor = Color.gray;
    [SerializeField]
    public Image mask1Image;
    [SerializeField]
    public Image mask2Image;
    [SerializeField]
    public SettingsPage settingsPage;
    [SerializeField]
    public TMP_Text p1TimeText;
    [SerializeField]
    public TMP_Text p1PeriodsText;
    [SerializeField]
    public TMP_Text p2TimeText;
    [SerializeField]
    public TMP_Text p2PeriodsText;
    [SerializeField]
    public GameObject pauseOverlay1;
    [SerializeField]
    public GameObject pauseOverlay2;
    [SerializeField]
    public GameObject addTime1;
    [SerializeField]
    public GameObject addTime2;
    [SerializeField]
    public AudioClip beepSound;
    [SerializeField]
    public AudioClip endSound;
    [SerializeField]
    public AudioClip manttoniSound;
    private AudioSource manttoniChannel;
    private float timeSinceLastMove;
    [SerializeField] private List<Theme> themes;
    [SerializeField]
    public AudioClip clickSound;
    private AudioSource clickChannel;
    [SerializeField]
    private AudioMixer mixer;
    [SerializeField]
    private AudioMixerGroup mixerGroup;
    private AudioSource channel;
    private TurnPlayer turnPlayer = TurnPlayer.None;


    private float p1Time;
    private float p2Time;
    private int p1PeriodsLeft;
    private int p2PeriodsLeft;
    private bool paused = false;
    private bool ended = false;
    private TimeSettings p1TimeSetting;
    private TimeSettings p2TimeSetting;
    private CommonTimeSettings common;
    private const float maxResetTime = 1f; //seconds
    private float resetClickTimer = -1;
    private int deviceSeed = -1;
    private float deviceRandom = 0;

    void Start() {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        manttoniChannel = CreateSource();
        manttoniChannel.clip = manttoniSound;
        clickChannel = CreateSource();
        clickChannel.clip = clickSound;
        channel = CreateSource();
        channel.clip = beepSound;
        settingsPage.gameObject.SetActive(true);
        if (deviceSeed < 0) {
            var id = SystemInfo.deviceUniqueIdentifier;
            foreach (var c in id) {
                deviceSeed += (int)c;
            }
            UnityEngine.Random.InitState(deviceSeed);
            deviceRandom = UnityEngine.Random.value;
            Debug.Log("randomSeed: " + deviceSeed + ", deviceRandom: " + deviceRandom);
        }
    }

    public void PauseButtonClicked() {
        if (turnPlayer != TurnPlayer.None) {
            paused = !paused;
            pauseOverlay1.SetActive(paused);
            pauseOverlay2.SetActive(paused);
            addTime1.SetActive(paused && settingsPage.CanAddTime);
            addTime2.SetActive(paused && settingsPage.CanAddTime);
        }
    }

    public void PlayBeep(bool ended) {
        if (ended) {
            channel.clip = endSound;
            channel.pitch = 1;
        } else {
            channel.clip = beepSound;
            channel.pitch = 1 + (deviceRandom * 0.6f - 0.35f);
        }
        channel.Play();
    }

    internal AudioSource CreateSource() {
        GameObject g = new GameObject();
        g.name = "AudioSource";
        g.transform.parent = gameObject.transform;
        g.transform.localPosition = Vector3.zero;
        g.transform.localRotation = Quaternion.identity;
        g.transform.localScale = Vector3.one;

        var s = g.AddComponent<AudioSource>();
        s.volume = 1.0F;
        s.spatialBlend = 0.0F;
        s.outputAudioMixerGroup = mixerGroup;
        return s;
    }

    private void ResetGame() {
        var settings = settingsPage.GetTimeSettings();
        common = settings.commonSettings;
        p1TimeSetting = settings.p1Settings;
        p2TimeSetting = common.useSameTime ? settings.p1Settings : settings.p2Settings;

        p1PeriodsLeft = p1TimeSetting.japPer;
        p2PeriodsLeft = p2TimeSetting.japPer;
        p1PeriodsText.gameObject.SetActive(true);
        p2PeriodsText.gameObject.SetActive(true);

        paused = false;
        pauseOverlay1.SetActive(false);
        pauseOverlay2.SetActive(false);
        addTime1.SetActive(false);
        addTime2.SetActive(false);
        ended = false;
        p1Time = p1TimeSetting.mainMins * 60f + p1TimeSetting.mainSecs;
        p1TimeText.text = FormatTime(p1Time);
        p1PeriodsText.text = GetPlayerTimeTypeText(1);

        p2Time = p2TimeSetting.mainMins * 60f + p2TimeSetting.mainSecs;
        p2TimeText.text = FormatTime(p2Time);
        p2PeriodsText.text = GetPlayerTimeTypeText(2);
        turnPlayer = TurnPlayer.None;
        p1Button.color = inactiveColor;
        p2Button.color = inactiveColor;
    }

    public string GetPlayerTimeTypeText(int player) {
        string retVal = "";
        if (player == 1) {
            if (p1TimeSetting.japPer == 0 && p1TimeSetting.fischerMins == 0 && p1TimeSetting.fischerSecs == 0)
                return "SD";
            if (p1TimeSetting.japPer > 0)
                retVal += Mathf.Max(p1PeriodsLeft, 0) + "x(" + FormatTime(p1TimeSetting.japSecs + p1TimeSetting.japMins * 60f) + ") Left\n";
            if (p1TimeSetting.fischerMins > 0 || p1TimeSetting.fischerSecs > 0)
                retVal += "Fischer: " + (p1TimeSetting.fischerMins * 60f + p1TimeSetting.fischerSecs) + "s";
            return retVal;
        } else if (player == 2) {
            if (p2TimeSetting.japPer == 0 && p2TimeSetting.fischerMins == 0 && p2TimeSetting.fischerSecs == 0)
                return "SD";
            if (p2TimeSetting.japPer > 0)
                retVal += Mathf.Max(p2PeriodsLeft, 0) + "x(" + FormatTime(p2TimeSetting.japSecs + p2TimeSetting.japMins * 60f) + ") Left\n";
            if (p2TimeSetting.fischerMins > 0 || p2TimeSetting.fischerSecs > 0)
                retVal += "Fischer: " + (p2TimeSetting.fischerMins * 60f + p2TimeSetting.fischerSecs) + "s";
            return retVal;
        }
        return retVal;
    }

    public void ResetClicked() {
        if (turnPlayer == TurnPlayer.None) {
            ResetGame();
            return;
        }
        if (resetClickTimer > 0) {
            ResetGame();
            resetButton.color = Color.white;
            resetClickTimer = -1;
        } else
            resetClickTimer = maxResetTime; //seconds
    }

    public void SettingsClicked() {
        settingsPage.gameObject.SetActive(true);
        paused = true;
        pauseOverlay1.SetActive(true);
        pauseOverlay2.SetActive(true);
        addTime1.SetActive(paused && settingsPage.CanAddTime);
        addTime2.SetActive(paused && settingsPage.CanAddTime);
    }


    public void SettingsClosed() {
        if (!settingsPage.CloseSettings()) return;
        SetTheme(settingsPage.ThemeName);
        clickChannel.mute = !settingsPage.ClickSoundEnabled;

        if (settingsPage.SettingsChanged) {
            ResetGame();
        } else {
            var settings = settingsPage.GetTimeSettings();
            common = settings.commonSettings;
        }
        if (turnPlayer == TurnPlayer.None) {
            pauseOverlay1.SetActive(false);
            pauseOverlay2.SetActive(false);
            paused = false;
            p1Button.color = inactiveColor;
            p2Button.color = inactiveColor;
        } else if (turnPlayer == TurnPlayer.Player1)
            p2Button.color = inactiveColor;
        else if (turnPlayer == TurnPlayer.Player2)
            p1Button.color = inactiveColor;
        addTime1.SetActive(paused && settingsPage.CanAddTime);
        addTime2.SetActive(paused && settingsPage.CanAddTime);
    }

    private void SetTheme(string theme) {
        var t = themes.Find(th => th.themeName.Equals(theme));
        if (t != null) {
            p1Button.sprite = t.bigButton;
            p2Button.sprite = t.bigButton;
            mask1Image.sprite = t.bigButton;
            mask2Image.sprite = t.bigButton;
            pauseButton.sprite = t.pauseButton;
            pauseButton.GetComponentInChildren<Text>().enabled = false;
            settingsButton.sprite = t.settingsButton;
            settingsButton.GetComponentInChildren<Text>().enabled = false;
            resetButton.sprite = t.resetButton;
            resetButton.GetComponentInChildren<Text>().enabled = false;
            inactiveColor = t.inactiveColor;
        } else {
            p1Button.sprite = null;
            p2Button.sprite = null;
            mask1Image.sprite = null;
            mask2Image.sprite = null;
            pauseButton.sprite = null;
            pauseButton.GetComponentInChildren<Text>().enabled = true;
            settingsButton.sprite = null;
            settingsButton.GetComponentInChildren<Text>().enabled = true;
            resetButton.sprite = null;
            resetButton.GetComponentInChildren<Text>().enabled = true;
            inactiveColor = Color.gray;
        }
    }

    public void AddTime(int player) {
        if (paused) {
            if (player == 0) {
                p1Time += 10;
                p1TimeText.text = FormatTime(p1Time);
            } else if (player == 1) {
                p2Time += 10;
                p2TimeText.text = FormatTime(p2Time);
            }
            if (ended && p1Time > 0 && p2Time > 0) ended = false;
            if (!ended)
                if (turnPlayer == TurnPlayer.Player1) p1Button.color = Color.yellow;
            if (turnPlayer == TurnPlayer.Player2) p2Button.color = Color.yellow;
        }

    }

    public void P1ButtonClicked() {
        if (ended || paused || p1Time < 0) return;
        if (turnPlayer == TurnPlayer.None || turnPlayer == TurnPlayer.Player1) {
            clickChannel.Play();
            if (SettingsPage.MsEnabled() && Time.realtimeSinceStartup - timeSinceLastMove < 1) manttoniChannel.Play();
            p2Button.color = Color.yellow;
            p1Button.color = inactiveColor;
            if (turnPlayer != TurnPlayer.None) {
                if (p1PeriodsLeft == p1TimeSetting.japPer) {
                    p1Time += (p1TimeSetting.fischerMins * 60f + p1TimeSetting.fischerSecs);
                    p1TimeText.text = FormatTime(p1Time);
                } else if (p1TimeSetting.japPer > 0 && p1PeriodsLeft >= 0) {
                    p1Time = p1TimeSetting.japSecs + p1TimeSetting.japMins * 60f;
                    p1TimeText.text = FormatTime(p1Time);
                }
            }
            turnPlayer = TurnPlayer.Player2;
            timeSinceLastMove = Time.realtimeSinceStartup;
        }
    }

    public void P2ButtonClicked() {
        if (ended || paused || p2Time < 0) return;
        if (turnPlayer == TurnPlayer.None || turnPlayer == TurnPlayer.Player2) {
            clickChannel.Play();
            if (SettingsPage.MsEnabled() && Time.realtimeSinceStartup - timeSinceLastMove < 1) manttoniChannel.Play();
            p1Button.color = Color.yellow;
            p2Button.color = inactiveColor;
            if (turnPlayer != TurnPlayer.None) {
                if (p2PeriodsLeft == p2TimeSetting.japPer) {
                    p2Time += p2TimeSetting.fischerMins * 60f + p2TimeSetting.fischerSecs;
                    p2TimeText.text = FormatTime(p2Time);
                } else if (p2TimeSetting.japPer > 0 && p2PeriodsLeft >= 0) {
                    p2Time = p2TimeSetting.japSecs + p2TimeSetting.japMins * 60f;
                    p2TimeText.text = FormatTime(p2Time);
                }
            }
            turnPlayer = TurnPlayer.Player1;
            timeSinceLastMove = Time.realtimeSinceStartup;
        }

    }

    void Update() {
        if (resetClickTimer >= 0) {
            resetClickTimer -= Time.deltaTime;
            resetButton.color = Color.Lerp(Color.green, Color.white, 1 - resetClickTimer / maxResetTime);
        }
        if (resetClickTimer <= 0) {
            resetClickTimer = -1;
            resetButton.color = Color.white;
        }
    }

    void FixedUpdate() {
        if (paused || ended) return;
        if (turnPlayer == TurnPlayer.Player1) {
            if (p1Time > 0) {
                var timeBefore = p1Time;
                p1Time -= Time.fixedDeltaTime;
                if (p1Time < (common.beepMins * 60f + common.beepSecs) && Mathf.FloorToInt(timeBefore) > Mathf.FloorToInt(p1Time))
                    PlayBeep(p1Time <= 0 && p1PeriodsLeft == 0);
            } else if (p1PeriodsLeft > 0) {
                p1PeriodsLeft--;
                p1Time = p1TimeSetting.japSecs + p1TimeSetting.japMins * 60f;
            } else {
                ended = true;
                p1Time = 0;
                p1Button.color = Color.red;
            }
            p1TimeText.text = FormatTime(p1Time);
            p1PeriodsText.text = GetPlayerTimeTypeText(1);
        } else if (turnPlayer == TurnPlayer.Player2) {
            if (p2Time > 0) {
                var timeBefore = p2Time;
                p2Time -= Time.fixedDeltaTime;
                if (p2Time < (common.beepMins * 60f + common.beepSecs) && Mathf.FloorToInt(timeBefore) > Mathf.FloorToInt(p2Time))
                    PlayBeep(p2Time <= 0 && p2PeriodsLeft == 0);
            } else if (p2PeriodsLeft > 0) {
                p2PeriodsLeft--;
                p2Time = p2TimeSetting.japSecs + p2TimeSetting.japMins * 60f;
            } else {
                ended = true;
                p2Time = 0;
                p2Button.color = Color.red;
            }
            p2TimeText.text = FormatTime(p2Time);
            p2PeriodsText.text = GetPlayerTimeTypeText(2);
        }
    }

    private string FormatTime(float time) {
        if (time < 0) time = 0;
        int minutes = (int)time / 60;
        int seconds = (int)time - 60 * minutes;
        int milliseconds = (int)(100 * (time - minutes * 60 - seconds));
        if (minutes == 0 && seconds < 10)
            return string.Format("{0:00}.{1:00}", seconds, milliseconds);
        else
            return string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
