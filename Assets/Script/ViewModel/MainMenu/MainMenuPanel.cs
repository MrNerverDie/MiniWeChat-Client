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
        public Button _createGroupButton;
        public Image _imageUnReadChat;

        public MainMenuNaviBar _mainMenuNaviBar;

        public ScrollRect _swipePanel;

        public override void OnEnter(object param = null)
        {
            base.OnEnter(param);

            _searchButton.onClick.AddListener(OnClickSearchButton);
            _createGroupButton.onClick.AddListener(OnClickCreateGroupButton);
        }

        public override void OnShow(object param = null)
        {
            base.OnShow(param);
            _mainMenuNaviBar.GetCurPanel().Show();

            _imageUnReadChat.gameObject.SetActive(GlobalChat.GetInstance().IsAnyUnReadChat());
            MessageDispatcher.GetInstance().RegisterMessageHandler((uint)EUIMessage.UPDATE_CHAT_LIST, OnUpdateChatList);
            
        }

        public override void OnHide()
        {
            base.OnHide();
            _mainMenuNaviBar.GetCurPanel().Hide();

            MessageDispatcher.GetInstance().UnRegisterMessageHandler((uint)EUIMessage.UPDATE_CHAT_LIST, OnUpdateChatList);

        }

        public void OnClickSearchButton()
        {
            StateManager.GetInstance().PushState<SearchFriendPanel>(EUIType.SearchFriendPanel);
        }

        public void OnClickCreateGroupButton()
        {
            StateManager.GetInstance().PushState<CreateGroupPanel>(EUIType.CreateGroupPanel);
        }

        public void OnUpdateChatList(uint iMessageType, object kParam)
        {
            _imageUnReadChat.gameObject.SetActive(GlobalChat.GetInstance().IsAnyUnReadChat());
        }
    }
}
