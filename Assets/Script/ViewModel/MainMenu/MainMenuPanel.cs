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
        public MainMenuNaviBar _mainMenuNaviBar;

        public override void OnEnter(object param = null)
        {
            base.OnEnter(param);
            _searchButton.onClick.AddListener(OnClickSearchButton);

            _mainMenuNaviBar.SwitchToTab((int)MainMenuNaviBar.MainMenuTab.CHAT_LIST);
        }

        public override void OnExit()
        {
            base.OnExit();
            UIManager.GetInstance().DestroySingleUI(EUIType.MainMenuPanel);
        }

        public override void OnShow(object param = null)
        {
            base.OnShow(param);
            _mainMenuNaviBar.GetCurPanel().Show();
        }

        public override void OnHide()
        {
            base.OnHide();
            _mainMenuNaviBar.GetCurPanel().Hide();
        }

        public void OnClickSearchButton()
        {
            StateManager.GetInstance().PushState<SearchFriendPanel>(EUIType.SearchFriendPanel);
        }
    }
}
