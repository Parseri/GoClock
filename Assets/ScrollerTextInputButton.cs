using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Parseri.GoClock.UI
{
    [RequireComponent(typeof(Button))]
    public class ScrollerTextInputButton : MonoBehaviour
    {
        [SerializeField] private TMP_InputField inputField;
        private Button inputButton;

        private void Start()
        {
            inputButton = GetComponent<Button>();
            inputField.enabled = false;
            inputButton.onClick.AddListener(ButtonClicked);
            inputField.onEndEdit.AddListener(TextEntered);
        }

        public void TextEntered(string text)
        {
            if (string.IsNullOrEmpty(text))
                inputField.text = "0";
            inputField.enabled = false;
            inputButton.enabled = true;
        }

        public void ButtonClicked()
        {
            inputButton.enabled = false;
            inputField.enabled = true;
            inputField.ActivateInputField();
        }
    }
}
