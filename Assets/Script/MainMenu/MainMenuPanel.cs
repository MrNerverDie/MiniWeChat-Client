using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace MiniWeChat
{
    public class MainMenuPanel : BaseState
    {
        public Button _searchButton;

        public override void OnEnter(object param = null)
        {
            base.OnEnter(param);
            
        }

        public override void OnExit()
        {
            base.OnExit();
            UIManager.GetInstance().DestroySingleUI(EUIType.MainMenuPanel);
        }


    }
}
