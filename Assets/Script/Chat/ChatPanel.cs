using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace MiniWeChat
{
    public class ChatPanel : BaseState
    {
        public Button _buttonBack;

        public override void OnEnter(object param = null)
        {
            base.OnEnter(param);
        }

        public override void OnExit()
        {
            UIManager.GetInstance().DestroySingleUI(EUIType.ChatPanel);
        }

        public void Start()
        {
            _buttonBack.onClick.AddListener(OnClickBackButton);
        }

        public void OnClickBackButton()
        {
            StateManager.GetInstance().PopState();
        }
    }
}

