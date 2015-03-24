using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using protocol;

namespace MiniWeChat
{
    public class GlobalUser : Singleton<GlobalUser>
    {
        private string _userId;
        public string UserId
        {
            get { return _userId; }
        }

        private string _userPassword;
        public string UserPassword
        {
            get { return _userPassword; }
        }

        private string _userName;

        public string UserName
        {
            get { return _userName; }
        }

        public override void Init()
        {
            MessageDispatcher.GetInstance().RegisterMessageHandler((uint)ENetworkMessage.GETUSERINFO_RSP, OnGetUserInfo);
        }

        public override void Release()
        {
            MessageDispatcher.GetInstance().UnRegisterMessageHandler((uint)ENetworkMessage.GETUSERINFO_RSP, OnGetUserInfo);
        }

        public void OnGetUserInfo(uint iMessageType, object kParam)
        {
            GetUserInfoRsp rsp = kParam as GetUserInfoRsp;
            _userId = rsp.userItem.userId;
            _userName = rsp.userItem.userName;
            if (rsp.resultCode == GetUserInfoRsp.ResultCode.SUCCESS)
            {
                GameObject go = UIManager.GetInstance().GetSingleUI(EUIType.MainMenuPanel);
                StateManager.GetInstance().ClearStates();
                StateManager.GetInstance().PushState<MainMenuPanel>(go);
            }
        }

        
    }
}

