using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace MiniWeChat
{
    public class DoubleButtonInputDialog : MonoBehaviour
    {
        public Text _labelTitle;
        public Text _labelInputHint;
        public InputField _inputContent;
        public Button _buttonConfirm;
        public Button _buttonCancel;

        private UnityEngine.Events.UnityAction<string> _confirmCallback;

        public void Show(string title, string inputHint, string inputPlaceHolder, string inputContent, InputField.ContentType contentType,
            UnityEngine.Events.UnityAction<string> confirmCallback, UnityEngine.Events.UnityAction cancelCallback)
        {
            gameObject.SetActive(true);
            UIManager.GetInstance().SetSiblingToTop(gameObject);

            if (title != null)
            {
                _labelTitle.text = title;
            }

            if (inputHint != null)
            {
                _labelInputHint.text = inputHint;
            }

            if (inputPlaceHolder != null)
            {
                _inputContent.placeholder.GetComponent<Text>().text = inputPlaceHolder;
            }

            if (inputContent != null)
            {
                _inputContent.text = inputContent;
            }

            _inputContent.contentType = contentType;

            _confirmCallback = confirmCallback;
            _buttonConfirm.onClick.AddListener(OnClickConfirmButton);


            if (cancelCallback != null)
            {
                _buttonCancel.onClick.AddListener(cancelCallback);
            }
            _buttonCancel.onClick.AddListener(Hide);

        }

        public void Hide()
        {
            gameObject.SetActive(false);
            _buttonConfirm.onClick.RemoveAllListeners();
            _buttonCancel.onClick.RemoveAllListeners();
        }

        public void OnClickConfirmButton()
        {
            if (_confirmCallback != null)
            {
                _confirmCallback(_inputContent.text);
            }
            Hide();
        }
    }
}

