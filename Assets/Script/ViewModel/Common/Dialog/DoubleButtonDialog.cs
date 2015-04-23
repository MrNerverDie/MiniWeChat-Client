using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace MiniWeChat
{
    public class DoubleButtonDialog : SingleButtonDialog
    {
        [SerializeField]
        public Button _buttonCancel;

        public void Show(string title, string content, UnityEngine.Events.UnityAction confirmCallback = null, UnityEngine.Events.UnityAction cancelCallback = null)
        {
            base.Show(title, content, confirmCallback);

            if (cancelCallback == null)
            {
                cancelCallback = Hide;
            }

            _buttonCancel.onClick.AddListener(cancelCallback);
        }
    }
}

