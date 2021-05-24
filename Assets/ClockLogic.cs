using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
public class ClockLogic : MonoBehaviour
{
    private enum TurnPlayer
    {
        None,
        Player1,
        Player2
    }
    [SerializeField]
    public Image p1Button;
    [SerializeField]
    public Image p2Button;
    [SerializeField]
    public GameObject settingsPage;
    [SerializeField]
    public Text p1TimeText;
    [SerializeField]
    public Text p2TimeText;
    [SerializeField]
    public GameObject pauseOverlay;
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
    private bool paused = false;
    private bool ended = false;
    private bool settingsChanged = false;
    private float fisherAddition = 5f;
    private float initialTime = 5 * 60f;
    private float beepAfterSeconds = 0;

    private float fisherSecs = 0;
    private float fisherMins = 0;
    private float mainSecs = 0;
    private float mainMins = 0;
    private float beepMins = 0;
    private float beepSecs = 0;

    // Start is called before the first frame update
    void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        channel = CreateSource();
        channel.clip = beepSound;
        ResetGame();
    }

    public void PauseButtonClicked()
    {
        if (turnPlayer != TurnPlayer.None)
        {
            paused = !paused;
            pauseOverlay.SetActive(paused);
        }
    }

    private void PlayBeep(bool ended)
    {
        if (ended)
            channel.clip = endSound;
        else 
            channel.clip = beepSound;
        channel.Play();
    }

    internal AudioSource CreateSource()
    {
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

    private void ResetGame()
    {
        paused = false;
        pauseOverlay.SetActive(false);
        ended = false;
        p1Time = initialTime;//5 * 60f;
        p1TimeText.text = FormatTime(p1Time);
        p2Time = initialTime;//5 * 60f;
        p2TimeText.text = FormatTime(p2Time);
        turnPlayer = TurnPlayer.None;
        p1Button.color = Color.gray;
        p2Button.color = Color.gray;
    }

    public void ResetClicked()
    {
        ResetGame();
    }

    public void SettingsClicked()
    {
        settingsPage.SetActive(true);
        paused = true;
        pauseOverlay.SetActive(true);
        settingsChanged = false;
    }

    public void Setting1Clicked()
    {
        SettingsChanged(5 * 60f, 5f);
    }

    public void Setting2Clicked()
    {
        SettingsChanged(10 * 60f, 5f);
    }

    public void Setting3Clicked()
    {
        SettingsChanged(5 * 60f, 10f);
    }

    public void MainTimeMinsEntered(string mainTime)
    {
        float main;
        if (float.TryParse(mainTime, out main))
        {
            mainMins = main;
            settingsChanged = true;
        }
    }

    public void FisherTimeMinsEntered(string fisherTime)
    {
        float fisher;
        if (float.TryParse(fisherTime, out fisher))
        {
            fisherMins = fisher;
            settingsChanged = true;
        }
    }

    public void MainTimeSecsEntered(string mainTime)
    {
        float main;
        if (float.TryParse(mainTime, out main))
        {
            mainSecs = main;
            settingsChanged = true;
        }
    }

    public void FisherTimeSecsEntered(string fisherTime)
    {
        float fisher;
        if (float.TryParse(fisherTime, out fisher))
        {
            fisherSecs = fisher;
            settingsChanged = true;
        }
    }

    public void BeepTimeMinsEntered(string bMins)
    {
        float bm;
        if (float.TryParse(bMins, out bm))
        {
            beepMins = bm;
            settingsChanged = true;
        }
    }

    public void BeepTimeSecsEntered(string bSecs)
    {
        float bs;
        if (float.TryParse(bSecs, out bs))
        {
            beepSecs = bs;
            settingsChanged = true;
        }
    }


    public void SettingsChanged(float initial, float addition)
    {
        initialTime = initial;
        fisherAddition = addition;
        ResetGame();
        settingsPage.SetActive(false);
    }

    public void SettingsClosed()
    {
        if (settingsChanged)
        {
            initialTime = mainSecs + mainMins * 60f;
            fisherAddition = fisherSecs + fisherMins * 60f;
            beepAfterSeconds = beepSecs + beepMins * 60f;
            ResetGame();
            settingsChanged = false;
        }
        settingsPage.SetActive(false);
    }

    public void P1ButtonClicked()
    {
        if (ended || paused) return;
        if (turnPlayer == TurnPlayer.None || turnPlayer == TurnPlayer.Player1)
        {
            p2Button.color = Color.yellow;
            p1Button.color = Color.gray;
            if (turnPlayer != TurnPlayer.None)
            {
                p1Time += fisherAddition;
                p1TimeText.text = FormatTime(p1Time);
            }
            turnPlayer = TurnPlayer.Player2;
        }
    }

    public void P2ButtonClicked()
    {
        if (ended || paused) return;
        if (turnPlayer == TurnPlayer.None || turnPlayer == TurnPlayer.Player2)
        {
            p1Button.color = Color.yellow;
            p2Button.color = Color.gray;
            if (turnPlayer != TurnPlayer.None)
            {
                p2Time += fisherAddition;
                p2TimeText.text = FormatTime(p2Time);
            }
            turnPlayer = TurnPlayer.Player1;
        }

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (paused || ended) return;
        if (turnPlayer == TurnPlayer.Player1)
        {
            if (p1Time > 0)
            {
                var timeBefore = p1Time;
                p1Time -= Time.fixedDeltaTime;
                if (p1Time < beepAfterSeconds && Mathf.FloorToInt(timeBefore) > Mathf.FloorToInt(p1Time))
                    PlayBeep(p1Time <= 0);
            }
            else
            {
                ended = true;
                p1Time = 0;
                p1Button.color = Color.red;
            }
            p1TimeText.text = FormatTime(p1Time);
        }
        else if (turnPlayer == TurnPlayer.Player2)
        {
            if (p2Time > 0)
            {
                var timeBefore = p2Time;
                p2Time -= Time.fixedDeltaTime;
                if (p2Time < beepAfterSeconds && Mathf.FloorToInt(timeBefore) > Mathf.FloorToInt(p2Time))
                    PlayBeep(p2Time <= 0);
            }
            else
            {
                ended = true;
                p2Time = 0;
                p2Button.color = Color.red;
            }
            p2TimeText.text = FormatTime(p2Time);
        }
    }

    private string FormatTime(float time)
    {
        int minutes = (int)time / 60;
        int seconds = (int)time - 60 * minutes;
        int milliseconds = (int)(100 * (time - minutes * 60 - seconds));
        if (minutes == 0 && seconds < 10)
            return string.Format("{0:00}:{1:00}", seconds, milliseconds);
        else
            return string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
