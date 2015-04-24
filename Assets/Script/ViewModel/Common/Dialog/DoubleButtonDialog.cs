using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace MiniWeChat
{
    public class DoubleButtonDialog : SingleButtonDialog
    {
        [SerializeField]
        public Button _buttonCancel;

        private UnityEngine.Events.UnityAction _cancelCallback;

        public void Show(string title, string content, UnityEngine.Events.UnityAction confirmCallback = null, UnityEngine.Events.UnityAction cancelCallback = null)
        {
            base.Show(title, content, confirmCallback);

            _cancelCallback = cancelCallback;

            _buttonCancel.onClick.AddListener(DoCancelCallBack);
        }

        public void DoCancelCallBack()
        {
            if (_cancelCallback != null)
            {
                _cancelCallback();
            }

            Hide();
        }
    }
}

