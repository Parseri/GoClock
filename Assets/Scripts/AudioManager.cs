using System;
using System.Collections.Generic;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.Audio;

namespace Parseri.GoClock
{
    public class AudioManager : MonoBehaviour
    {
        public enum AudioType
        {
            General,
            Beep,
            Click,
            End,
            Wroom,
            Button,
        }

        [Serializable]
        public class AudioItem
        {
            public AudioType itemType;
            public AudioClip clip;
            public AudioMixerGroup group;
        }

        private static AudioManager Instance = null;

        [SerializeField] private List<AudioItem> items = new();

        [SerializeField] private AudioMixer masterMixer;

        private AudioSource _wroomChannel;
        private AudioSource _clickChannel;
        private AudioSource _channel;

        private bool _muted = false;

        private Dictionary<AudioType, AudioItem> _items = new();

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                foreach (var i in items)
                {
                    if (!_items.TryAdd(i.itemType, i))
                    {
                        Debug.Log("Failed to add: " + i.itemType.ToString());
                    }
                }
                _wroomChannel = CreateSource(AudioType.Wroom);
                _wroomChannel.clip = _items[AudioType.Wroom].clip;
                _wroomChannel.outputAudioMixerGroup = _items[AudioType.Wroom].group;
                _clickChannel = CreateSource(AudioType.Click);
                _clickChannel.clip = _items[AudioType.Click].clip;
                _clickChannel.outputAudioMixerGroup = _items[AudioType.Click].group;
                _channel = CreateSource(AudioType.General);
                Debug.Log("default pitch: " + _channel.pitch);
            }
        }

        public static void SetMasterVolume(float volume)
        {
            if (Instance == null) return;

            Instance.Internal_SetMasterVolume(volume);
        }

        private void Internal_SetMasterVolume(float volume)
        {
            if (volume > 0)
            {
                _muted = false;
                masterMixer.SetFloat("MasterVol", Mathf.Log10(volume) * 20);
            }
            else
            {
                _muted = true;
            }
        }

        private AudioSource CreateSource(AudioType aType)
        {
            GameObject g = new()
            {
                name = "AudioSource_" + aType.ToString()
            };
            g.transform.parent = gameObject.transform;
            g.transform.localPosition = Vector3.zero;
            g.transform.localRotation = Quaternion.identity;
            g.transform.localScale = Vector3.one;

            return g.AddComponent<AudioSource>();
        }

        public static void MuteChannel(AudioType aType, bool mute)
        {
            if (Instance == null) return;

            Instance.Internal_MuteChannel(aType, mute);
        }

        private void Internal_MuteChannel(AudioType aType, bool mute)
        {
            AudioSource channel;
            switch (aType)
            {
                case AudioType.Click:
                    channel = _clickChannel;
                    break;
                case AudioType.Wroom:
                    channel = _wroomChannel;
                    break;
                default:
                    channel = _channel;
                    break;
            }
            if (channel == null) return;

            channel.mute = mute;
        }

        public static void StopAllSounds()
        {
            if (Instance == null) return;

            Instance.Internal_StopAllSounds();
        }

        private void Internal_StopAllSounds()
        {
            _wroomChannel.Stop();
            _clickChannel.Stop();
            _channel.Stop();
        }

        public static void PlaySound(AudioType audioType, float? overridePitch = null)
        {
            if (Instance == null) return;

            Instance.Internal_PlaySound(audioType, overridePitch);
        }

        private void Internal_PlaySound(AudioType audioType, float? overridePitch = null)
        {
            if (_muted || !_items.ContainsKey(audioType)) return;

            AudioSource channel;
            switch (audioType)
            {
                case AudioType.Click:
                    _clickChannel.Play();
                    return;
                case AudioType.Wroom:
                    _wroomChannel.Play();
                    return;
                default:
                    channel = _channel;
                    break;
            }
            if (channel == null) return;

            if (overridePitch.HasValue)
            {
                channel.pitch = overridePitch.Value;
            }
            else
            {
                channel.pitch = 1;
            }
            var audioItem = _items[audioType];

            _channel.outputAudioMixerGroup = audioItem.group;

            channel.clip = audioItem.clip;
            channel.Play();
        }
    }
}