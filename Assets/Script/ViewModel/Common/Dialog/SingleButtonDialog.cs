using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace MiniWeChat
{
    public class SingleButtonDialog : MonoBehaviour
    {
        public Text _labelTitle;
        public Text _labelContent;
        public Button _buttonConfirm;

        public void Show(string title, string content, UnityEngine.Events.UnityAction confirmCallback = null)
        {
            gameObject.SetActive(true);
            UIManager.GetInstance().SetSiblingToTop(gameObject);
            
            if (title != null)
            {
                _labelTitle.text = title;                
            }

            if (content != null)
            {
                _labelContent.text = content;                
            }

            if (confirmCallback == null)
            {
                confirmCallback = Hide;
            }

            _buttonConfirm.onClick.AddListener(confirmCallback);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
            _buttonConfirm.onClick.RemoveAllListeners();
        }
    }
}

