using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
public class ClockLogic : MonoBehaviour {

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
        public float beepSecs;
        [SerializeField]
        public float beepMins;
        [SerializeField]
        public int japPer;
        [SerializeField]
        public float japMins;
        [SerializeField]
        public float japSecs;
        public TimeSettings(float mm, float ms, float fm, float fs, float bm, float bs, int jp = 0, float jm = 0, float js = 30) {
            mainMins = mm;
            mainSecs = ms;
            fischerMins = fm;
            fischerSecs = fs;
            beepMins = bm;
            beepSecs = bs;
            japPer = jp;
            japMins = jm;
            japSecs = js;
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
    public SettingsPage settingsPage;
    [SerializeField]
    public Text p1TimeText;
    [SerializeField]
    public Text p1PeriodsText;
    [SerializeField]
    public Text p2TimeText;
    [SerializeField]
    public Text p2PeriodsText;
    [SerializeField]
    public GameObject pauseOverlay1;
    [SerializeField]
    public GameObject pauseOverlay2;
    [SerializeField]
    public AudioClip beepSound;
    [SerializeField]
    public AudioClip endSound;
    [SerializeField]
    private AudioMixer mixer;
    [SerializeField]
    private AudioMixerGroup mixerGroup;
    private AudioSource channel;
    private TurnPlayer turnPlayer = TurnPlayer.None;


    private float p1Time;
    private float p2Time;
    private int p1Periods;
    private int p2Periods;
    private bool paused = false;
    private bool ended = false;
    private float fisherAddition = 5f;
    private float initialTime = 5 * 60f;
    private float beepAfterSeconds = 0;
    private int japPeriods = 0;
    private float japMins = 0;
    private float japSeconds = 0;

    void Start() {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        channel = CreateSource();
        channel.clip = beepSound;
        settingsPage.gameObject.SetActive(true);
    }

    public void PauseButtonClicked() {
        if (turnPlayer != TurnPlayer.None) {
            paused = !paused;
            pauseOverlay1.SetActive(paused);
            pauseOverlay2.SetActive(paused);
        }
    }

    private void PlayBeep(bool ended) {
        if (ended)
            channel.clip = endSound;
        else
            channel.clip = beepSound;
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
        initialTime = settings.mainSecs + settings.mainMins * 60f;
        fisherAddition = settings.fischerSecs + settings.fischerMins * 60f;
        beepAfterSeconds = settings.beepSecs + settings.beepMins * 60f;
        japPeriods = settings.japPer;
        p1Periods = settings.japPer;
        p2Periods = settings.japPer;
        p1PeriodsText.gameObject.SetActive(p1Periods != 0);
        p2PeriodsText.gameObject.SetActive(p2Periods != 0);
        japMins = settings.japMins;
        japSeconds = settings.japSecs;
        paused = false;
        pauseOverlay1.SetActive(false);
        pauseOverlay2.SetActive(false);
        ended = false;
        p1Time = initialTime;//5 * 60f;
        p1TimeText.text = FormatTime(p1Time);
        p1PeriodsText.text = Mathf.Max(p1Periods, 0) + "x(" + FormatTime(japSeconds + japMins * 60f) + ")";
        p2Time = initialTime;//5 * 60f;
        p2TimeText.text = FormatTime(p2Time);
        p2PeriodsText.text = Mathf.Max(p2Periods, 0) + "x(" + FormatTime(japSeconds + japMins * 60f) + ")";
        turnPlayer = TurnPlayer.None;
        p1Button.color = Color.gray;
        p2Button.color = Color.gray;
    }

    public void ResetClicked() {
        ResetGame();
    }

    public void SettingsClicked() {
        settingsPage.gameObject.SetActive(true);
        paused = true;
        pauseOverlay1.SetActive(true);
        pauseOverlay2.SetActive(true);
    }


    public void SettingsClosed() {
        if (!settingsPage.CloseSettings()) return;
        if (settingsPage.SettingsChanged) {
            ResetGame();
        }
        if (turnPlayer == TurnPlayer.None) {
            pauseOverlay1.SetActive(false);
            pauseOverlay2.SetActive(false);
            paused = false;
        }
    }

    public void P1ButtonClicked() {
        if (ended || paused) return;
        if (turnPlayer == TurnPlayer.None || turnPlayer == TurnPlayer.Player1) {
            p2Button.color = Color.yellow;
            p1Button.color = Color.gray;
            if (turnPlayer != TurnPlayer.None) {
                if (p1Periods == japPeriods) {
                    p1Time += fisherAddition;
                    p1TimeText.text = FormatTime(p1Time);
                } else if (japPeriods > 0 && p1Periods >= 0) {
                    p1Time = japSeconds + japMins * 60f;
                    p1TimeText.text = FormatTime(p1Time);
                }
            }
            turnPlayer = TurnPlayer.Player2;
        }
    }

    public void P2ButtonClicked() {
        if (ended || paused) return;
        if (turnPlayer == TurnPlayer.None || turnPlayer == TurnPlayer.Player2) {
            p1Button.color = Color.yellow;
            p2Button.color = Color.gray;
            if (turnPlayer != TurnPlayer.None) {
                if (p2Periods == japPeriods) {
                    p2Time += fisherAddition;
                    p2TimeText.text = FormatTime(p2Time);
                } else if (p2Periods > 0) {
                    p2Time = japSeconds + japMins * 60f;
                    p2TimeText.text = FormatTime(p2Time);
                }
            }
            turnPlayer = TurnPlayer.Player1;
        }

    }

    // Update is called once per frame
    void FixedUpdate() {
        if (paused || ended) return;
        if (turnPlayer == TurnPlayer.Player1) {
            if (p1Time > 0) {
                var timeBefore = p1Time;
                p1Time -= Time.fixedDeltaTime;
                if (p1Time < beepAfterSeconds && Mathf.FloorToInt(timeBefore) > Mathf.FloorToInt(p1Time))
                    PlayBeep(p1Time <= 0 && p1Periods == 0);
            } else if (p1Periods > 0) {
                p1Periods--;
                p1Time = japSeconds + japMins * 60f;
            } else {
                ended = true;
                p1Time = 0;
                p1Button.color = Color.red;
            }
            p1TimeText.text = FormatTime(p1Time);
            p1PeriodsText.text = Mathf.Max(p1Periods, 0) + "x(" + FormatTime(japSeconds + japMins * 60f) + ")";
        } else if (turnPlayer == TurnPlayer.Player2) {
            if (p2Time > 0) {
                var timeBefore = p2Time;
                p2Time -= Time.fixedDeltaTime;
                if (p2Time < beepAfterSeconds && Mathf.FloorToInt(timeBefore) > Mathf.FloorToInt(p2Time))
                    PlayBeep(p2Time <= 0 && p2Periods == 0);
            } else if (p2Periods > 0) {
                p2Periods--;
                p2Time = japSeconds + japMins * 60f;
            } else {
                ended = true;
                p2Time = 0;
                p2Button.color = Color.red;
            }
            p2TimeText.text = FormatTime(p2Time);
            p2PeriodsText.text = Mathf.Max(p2Periods, 0) + "x(" + FormatTime(japSeconds + japMins * 60f) + ")";
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
