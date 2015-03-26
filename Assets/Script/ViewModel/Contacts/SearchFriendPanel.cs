using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using protocol;

namespace MiniWeChat
{
    public class SearchFriendPanel : BaseState
    {
        public InputField _inputUserID;
        public Button _buttonSearchFriend;

        public override void OnEnter(object param = null)
        {
            base.OnEnter(param);
            _inputUserID.onValueChange.AddListener(OnValueChangeUserID);
            _buttonSearchFriend.onClick.AddListener(OnClickSearchFriendButton);

            UIManager.GetInstance().AddChild(gameObject, EUIType.BackButton);
        }

        public override void OnExit()
        {
            base.OnExit();
            UIManager.GetInstance().DestroySingleUI(EUIType.SearchFriendPanel);
        }

        public override void OnShow(object param = null)
        {
            base.OnShow(param);

            _inputUserID.text = "";
            _buttonSearchFriend.interactable = false;
            MessageDispatcher.GetInstance().RegisterMessageHandler((uint)ENetworkMessage.GET_USERINFO_RSP, OnGetUserInfoRsp);

        }

        public override void OnHide()
        {
            base.OnHide();
            MessageDispatcher.GetInstance().UnRegisterMessageHandler((uint)ENetworkMessage.GET_USERINFO_RSP, OnGetUserInfoRsp);
        }

        public void OnValueChangeUserID(string text)
        {
            _buttonSearchFriend.interactable = (text != "");
        }

        public void OnClickSearchFriendButton()
        {
            GetUserInfoReq req = new GetUserInfoReq
            {
                targetUserId = _inputUserID.text,
            };

            NetworkManager.GetInstance().SendPacket<GetUserInfoReq>(ENetworkMessage.GET_USERINFO_REQ, req);
        }

        public void OnGetUserInfoRsp(uint iMessageType, object kParam)
        {
            GetUserInfoRsp rsp = kParam as GetUserInfoRsp;
            if (rsp.resultCode == GetUserInfoRsp.ResultCode.SUCCESS)
            {
                StateManager.GetInstance().PushState<FriendDetailPanel>(EUIType.FriendDetailPanel, rsp.userItem);
            }
            else if(rsp.resultCode == GetUserInfoRsp.ResultCode.FAIL)
            {
                
            }
        }
    }
}

