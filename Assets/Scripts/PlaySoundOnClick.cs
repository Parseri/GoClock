using UnityEngine;
using UnityEngine.UI;

namespace Parseri.GoClock
{
    [RequireComponent(typeof(Button))]
    public class PlaySoundOnClick : MonoBehaviour
    {
        [SerializeField] private AudioManager.AudioType audioType = AudioManager.AudioType.Button;

        private void Start()
        {
            GetComponent<Button>().onClick.AddListener(OnPlaySoundOnClick);
        }

        private void OnDisable()
        {
            GetComponent<Button>().onClick.RemoveListener(OnPlaySoundOnClick);
        }

        private void OnPlaySoundOnClick()
        {
            AudioManager.PlaySound(audioType);
        }
    }
}
