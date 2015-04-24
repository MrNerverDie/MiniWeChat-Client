using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using protocol;

namespace MiniWeChat
{

    public class FriendDetailPanel : BaseState
    {

        public Text _laeblName;
        public Text _labelId;
        public Image _imageHead;

        public Button _buttonAddFriend;
        public Button _buttonDeleteFriend;
        public Button _buttonBeginChat;

        private UserItem _userItem;

        public override void OnEnter(object param)
        {
            base.OnEnter(param);

            _userItem = param as UserItem;

            _laeblName.text = _userItem.userName;
            _labelId.text = _userItem.userId;
            UIManager.GetInstance().SetImage(_imageHead, EAtlasName.Head, "00" + _userItem.headIndex);
            UIManager.GetInstance().AddChild(gameObject, EUIType.BackButton);

            InitButtons();
        }

        public override void OnShow(object param = null)
        {
            base.OnShow(param);
            MessageDispatcher.GetInstance().RegisterMessageHandler((uint)EUIMessage.UPDATE_FRIEND_DETAIL, OnUpdateFriendDetail);
        }

        public override void OnHide()
        {
            base.OnHide();
            MessageDispatcher.GetInstance().UnRegisterMessageHandler((uint)EUIMessage.UPDATE_FRIEND_DETAIL, OnUpdateFriendDetail);

        }

        private void InitButtons()
        {
            _buttonAddFriend.gameObject.SetActive(false);
            _buttonDeleteFriend.gameObject.SetActive(false);
            _buttonBeginChat.gameObject.SetActive(false);
            _buttonAddFriend.onClick.RemoveAllListeners();
            _buttonDeleteFriend.onClick.RemoveAllListeners();
            _buttonBeginChat.onClick.RemoveAllListeners();


            if (_userItem.userId == GlobalUser.GetInstance().UserId)
            {
                return;
            }
            else if (GlobalContacts.GetInstance().Contains(_userItem.userId))
            {
                _buttonBeginChat.gameObject.SetActive(true);
                _buttonDeleteFriend.gameObject.SetActive(true);

                _buttonBeginChat.onClick.AddListener(OnClickBeginChatButton);
                _buttonDeleteFriend.onClick.AddListener(OnClickDeleteFriendButton);
                
            }
            else if (!GlobalContacts.GetInstance().Contains(_userItem.userId))
            {
                _buttonAddFriend.gameObject.SetActive(true);

                _buttonAddFriend.onClick.AddListener(OnClickAddFriendButton);
            }
        }

        public void OnClickBeginChatButton()
        {
            StateManager.GetInstance().ClearStatesExceptBottom(true);
            ChatLog chatLog = GlobalChat.GetInstance().GetChatLog(_userItem.userId);
            StateManager.GetInstance().PushState<ChatPanel>(EUIType.ChatPanel, chatLog);
        }

        public void OnClickAddFriendButton()
        {
            AddFriendReq req = new AddFriendReq
            {
                friendUserId = _userItem.userId,
            };

            NetworkManager.GetInstance().SendPacket<AddFriendReq>(ENetworkMessage.ADD_FRIEND_REQ, req);
        }

        public void OnClickDeleteFriendButton()
        {
            DeleteFriendReq req = new DeleteFriendReq
            {
                friendUserId = _userItem.userId,
            };

            NetworkManager.GetInstance().SendPacket<DeleteFriendReq>(ENetworkMessage.DELETE_FRIEND_REQ, req); 
        }

        public void OnUpdateFriendDetail(uint iMessageType, object kParam)
        {
            InitButtons();
        }
    }
}

