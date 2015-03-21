using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace MiniWeChat
{
    public class RegisterPanel : BaseState
    {
        public override void OnEnter(object param = null)
        {
            UIManager.GetInstance().AddChild(gameObject, EUIType.BackButton);
        }

        public override void OnExit()
        {
            UIManager.GetInstance().DestroySingleUI(EUIType.RegisterPanel);
        }
    }
}

