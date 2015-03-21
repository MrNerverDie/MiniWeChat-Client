using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace MiniWeChat
{
    public class ChatPanel : BaseState
    {
        public override void OnEnter(object param = null)
        {
            UIManager.GetInstance().AddChild(gameObject, EUIType.BackButton);
        }

        public override void OnExit()
        {
            UIManager.GetInstance().DestroySingleUI(EUIType.ChatPanel);
        }
    }
}

