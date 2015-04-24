using UnityEngine;
using UnityEngine.UI;

using System.Collections.Generic;

using DG.Tweening;

namespace MiniWeChat
{
    public class SingleButtonDialog : BaseDialog
    {
        [SerializeField]
        public Text _labelTitle;
        [SerializeField]
        public Text _labelContent;
        [SerializeField]
        public Button _buttonConfirm;

        private UnityEngine.Events.UnityAction _confirmCallback;

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

            _confirmCallback = confirmCallback;

            _buttonConfirm.onClick.AddListener(DoConfirmCallBack);

            BeginEnterTween();
        }

        public void DoConfirmCallBack()
        {
            if (_confirmCallback != null)
            {
                _confirmCallback();
            }

            Hide();
        }

        protected override void Hide()
        {
            base.Hide();
            _buttonConfirm.onClick.RemoveAllListeners();
        }
    }
}

