using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using protocol;

namespace MiniWeChat
{
    public class GlobalUser : Singleton<GlobalUser>
    {
        #region Prop
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

        private bool _isLogin = false;
        public bool IsLogin
        {
            get { return _isLogin; }
        }
        #endregion

        #region LifeCycle
        public override void Init()
        {
            MessageDispatcher.GetInstance().RegisterMessageHandler((uint)ENetworkMessage.GETUSERINFO_RSP, OnGetUserInfoRsp);
            MessageDispatcher.GetInstance().RegisterMessageHandler((uint)ENetworkMessage.LOGIN_RSP, OnLoginRsp);
            MessageDispatcher.GetInstance().RegisterMessageHandler((uint)ENetworkMessage.PERSONALSETTINGS_RSP, OnPersonalSetRsp);
            MessageDispatcher.GetInstance().RegisterMessageHandler((uint)ENetworkMessage.LOGOUT_RSP, OnLogOutRsp);
            MessageDispatcher.GetInstance().RegisterMessageHandler((uint)EGeneralMessage.SOCKET_CONNECTED, TryLoginWithPref);

        }

        public override void Release()
        {
            MessageDispatcher.GetInstance().UnRegisterMessageHandler((uint)ENetworkMessage.GETUSERINFO_RSP, OnGetUserInfoRsp);
            MessageDispatcher.GetInstance().UnRegisterMessageHandler((uint)ENetworkMessage.LOGIN_RSP, OnGetUserInfoRsp);
            MessageDispatcher.GetInstance().UnRegisterMessageHandler((uint)ENetworkMessage.PERSONALSETTINGS_RSP, OnPersonalSetRsp);
            MessageDispatcher.GetInstance().UnRegisterMessageHandler((uint)ENetworkMessage.PERSONALSETTINGS_RSP, OnLogOutRsp);
            MessageDispatcher.GetInstance().UnRegisterMessageHandler((uint)EGeneralMessage.SOCKET_CONNECTED, TryLoginWithPref);

        }
        #endregion

        #region Login
        private void TryLoginWithPref(uint iMessageType, object kParam)
        {
            if (PlayerPrefs.HasKey(GlobalVars.PREF_USER_ID) && PlayerPrefs.HasKey(GlobalVars.PREF_USER_PASSWORD))
            {
                TryLogin(PlayerPrefs.GetString(GlobalVars.PREF_USER_ID), PlayerPrefs.GetString(GlobalVars.PREF_USER_PASSWORD));                
            }
        }

        public void TryLogin(string id, string password)
        {
            LoginReq req = new LoginReq
            {
                userId = id,
                userPassword = password
            };
            _userId = id;
            _userPassword = password;
            NetworkManager.GetInstance().SendPacket<LoginReq>(ENetworkMessage.LOGIN_REQ, req);
        }

        #endregion

        #region MessageHandler
        public void OnGetUserInfoRsp(uint iMessageType, object kParam)
        {
            GetUserInfoRsp rsp = kParam as GetUserInfoRsp;
            if (rsp.resultCode == GetUserInfoRsp.ResultCode.SUCCESS)
            {
                _userId = rsp.userItem.userId;
                _userName = rsp.userItem.userName;
            }
        }

        public void OnLoginRsp(uint iMessageType, object kParam)
        {
            LoginRsp rsp = kParam as LoginRsp;
            if (rsp.resultCode == LoginRsp.ResultCode.SUCCESS)
            {
                _isLogin = true;

                GetUserInfoReq req = new GetUserInfoReq
                {
                    targetUserId = _userId,
                };
                NetworkManager.GetInstance().SendPacket<GetUserInfoReq>(ENetworkMessage.GETUSERINFO_REQ, req);

                PlayerPrefs.SetString(GlobalVars.PREF_USER_ID, _userId);
                PlayerPrefs.SetString(GlobalVars.PREF_USER_PASSWORD, _userPassword);
            }
        }

        public void OnPersonalSetRsp(uint iMessageType, object kParam)
        {
            PersonalSettingsRsp rsp = kParam as PersonalSettingsRsp;

            if (rsp.resultCode == PersonalSettingsRsp.ResultCode.SUCCESS)
            {
                GetUserInfoReq req = new GetUserInfoReq
                {
                    targetUserId = _userId,
                };

                NetworkManager.GetInstance().SendPacket<GetUserInfoReq>(ENetworkMessage.GETUSERINFO_REQ, req);
            }
        }

        public void OnLogOutRsp(uint iMessageType, object kParam)
        {
            LogoutRsp rsp = kParam as LogoutRsp;

            if (rsp.resultCode == LogoutRsp.ResultCode.SUCCESS)
            {
                PlayerPrefs.DeleteKey(GlobalVars.PREF_USER_PASSWORD);
                _isLogin = false;
            }
        }

        #endregion
    }
}

