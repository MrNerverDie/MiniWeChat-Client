using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using protocol;

namespace MiniWeChat
{
    public class MainMenuPanel : BaseState
    {
        public Button _searchButton;

        public override void OnEnter(object param = null)
        {
            base.OnEnter(param);
            _searchButton.onClick.AddListener(OnClickSearchButton);
        }

        public override void OnExit()
        {
            base.OnExit();
            UIManager.GetInstance().DestroySingleUI(EUIType.MainMenuPanel);
        }

        public override void OnShow(object param = null)
        {
            base.OnShow(param);
        }

        public override void OnHide()
        {
            base.OnHide();
        }

        public void OnClickSearchButton()
        {
            StateManager.GetInstance().PushState<SearchFriendPanel>(EUIType.SearchFriendPanel);
        }
    }
}
