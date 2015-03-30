using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace MiniWeChat
{
    public class ChatPanel : BaseState
    {
        public InputField _inputChat;

        public override void OnEnter(object param = null)
        {
            base.OnEnter(param);
            UIManager.GetInstance().AddChild(gameObject, EUIType.BackButton);
            _inputChat.MoveTextEnd(true);
        }

        public override void OnExit()
        {
            base.OnExit();
            UIManager.GetInstance().DestroySingleUI(EUIType.ChatPanel);
        }
    }
}

