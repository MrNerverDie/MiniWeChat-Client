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

        private int _headIndex;
        public int HeadIndex
        {
            get { return _headIndex; }
        }

        public bool IsEnterMainMenu
        {
            get { return PlayerPrefs.HasKey(GlobalVars.PREF_USER_PASSWORD); }
        }

        public UserItem Self
        {
            get { return new UserItem{ userName = _userName, userId = _userId, headIndex = _headIndex}; }
        }

        #endregion

        #region LifeCycle
        public override void Init()
        {
            MessageDispatcher.GetInstance().RegisterMessageHandler((uint)ENetworkMessage.GET_PERSONALINFO_RSP, OnGetPersonalInfoRsp);
            MessageDispatcher.GetInstance().RegisterMessageHandler((uint)ENetworkMessage.LOGIN_RSP, OnLoginRsp);
            MessageDispatcher.GetInstance().RegisterMessageHandler((uint)ENetworkMessage.PERSONALSETTINGS_RSP, OnPersonalSetRsp);
            MessageDispatcher.GetInstance().RegisterMessageHandler((uint)ENetworkMessage.LOGOUT_RSP, OnLogOutRsp);
            MessageDispatcher.GetInstance().RegisterMessageHandler((uint)EModelMessage.SOCKET_CONNECTED, TryLoginWithPref);
            MessageDispatcher.GetInstance().RegisterMessageHandler((uint)ENetworkMessage.OFFLINE_SYNC, OnOffLineSync);


            if (PlayerPrefs.HasKey(GlobalVars.PREF_USER_ID))
            {
                _userId = PlayerPrefs.GetString(GlobalVars.PREF_USER_ID);                
            }
            LoadUserInfo();
        }

        public override void Release()
        {
            MessageDispatcher.GetInstance().UnRegisterMessageHandler((uint)ENetworkMessage.GET_PERSONALINFO_RSP, OnGetPersonalInfoRsp);
            MessageDispatcher.GetInstance().UnRegisterMessageHandler((uint)ENetworkMessage.LOGIN_RSP, OnLoginRsp);
            MessageDispatcher.GetInstance().UnRegisterMessageHandler((uint)ENetworkMessage.PERSONALSETTINGS_RSP, OnPersonalSetRsp);
            MessageDispatcher.GetInstance().UnRegisterMessageHandler((uint)ENetworkMessage.PERSONALSETTINGS_RSP, OnLogOutRsp);
            MessageDispatcher.GetInstance().UnRegisterMessageHandler((uint)EModelMessage.SOCKET_CONNECTED, TryLoginWithPref);
            MessageDispatcher.GetInstance().UnRegisterMessageHandler((uint)ENetworkMessage.OFFLINE_SYNC, OnOffLineSync);

            SaveAndClearUserInfo();
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
            MessageDispatcher.GetInstance().DispatchMessage((uint)EModelMessage.TRY_LOGIN, null);
            NetworkManager.GetInstance().SendPacket<LoginReq>(ENetworkMessage.LOGIN_REQ, req);
        }

        private void DoLogOut()
        {
            PlayerPrefs.DeleteKey(GlobalVars.PREF_USER_PASSWORD);
            _isLogin = false;

            StateManager.GetInstance().ClearStates();
            StateManager.GetInstance().PushFirstState();
        }

        #endregion

        #region MessageHandler
        public void OnGetPersonalInfoRsp(uint iMessageType, object kParam)
        {
            NetworkMessageParam param = kParam as NetworkMessageParam;
            GetPersonalInfoRsp rsp = param.rsp as GetPersonalInfoRsp;
            GetPersonalInfoReq req = param.req as GetPersonalInfoReq;
            if (rsp.resultCode == GetPersonalInfoRsp.ResultCode.SUCCESS
                && req.userInfo)
            {
                _userName = rsp.userInfo.userName;
                _headIndex = rsp.userInfo.headIndex;
                MessageDispatcher.GetInstance().DispatchMessage((uint)EUIMessage.UPDATE_PERSONAL_DETAIL, null);
            }
        }

        public void OnLoginRsp(uint iMessageType, object kParam)
        {
            LoginRsp rsp = kParam as LoginRsp;
            if (rsp.resultCode == LoginRsp.ResultCode.SUCCESS)
            {
                _isLogin = true;

                GetPersonalInfoReq req = new GetPersonalInfoReq
                {
                    friendInfo  = true,
                    userInfo = true,
                };
                NetworkManager.GetInstance().SendPacket<GetPersonalInfoReq>(ENetworkMessage.GET_PERSONALINFO_REQ, req);

                PlayerPrefs.SetString(GlobalVars.PREF_USER_ID, _userId);
                PlayerPrefs.SetString(GlobalVars.PREF_USER_PASSWORD, _userPassword);

                CreateDir();
            }
        }

        public void OnPersonalSetRsp(uint iMessageType, object kParam)
        {
            PersonalSettingsRsp rsp = kParam as PersonalSettingsRsp;

            Log4U.LogInfo(rsp.resultCode);

            if (rsp.resultCode == PersonalSettingsRsp.ResultCode.SUCCESS)
            {
                GetPersonalInfoReq req = new GetPersonalInfoReq
                {
                    userInfo = true,
                };

                NetworkManager.GetInstance().SendPacket<GetPersonalInfoReq>(ENetworkMessage.GET_PERSONALINFO_REQ, req);
            }
        }

        public void OnLogOutRsp(uint iMessageType, object kParam)
        {
            LogoutRsp rsp = kParam as LogoutRsp;

            if (rsp.resultCode == LogoutRsp.ResultCode.SUCCESS)
            {
                SaveAndClearUserInfo();
                DoLogOut();
            }
        }

        public void OnOffLineSync(uint iMessageType, object kParam)
        {
            OffLineSync rsp = kParam as OffLineSync;

            if (rsp.causeCode == OffLineSync.CauseCode.CHANGE_PASSWORD ||
                rsp.causeCode == OffLineSync.CauseCode.ANOTHER_LOGIN)
            {
                SaveAndClearUserInfo();
                DoLogOut();
            }
        }

        #endregion

        #region LocalData
        
        public void CreateDir()
        {
            string[] dirPathList = new string[]
            {
                Application.persistentDataPath + "/" + _userId,
                Application.persistentDataPath + "/" + _userId + "/Chat",
                Application.persistentDataPath + "/" + _userId + "/Contacts",
                Application.persistentDataPath + "/" + _userId + "/Head",
                Application.persistentDataPath + "/" + _userId + "/Image",
            };

            foreach (var dirPath in dirPathList)
            {
                IOTool.CreateDir(dirPath);
            }
        }

        public string GetUserDir()
        {
            return Application.persistentDataPath + "/" + _userId;
        }

        public void LoadUserInfo()
        {
            UserItem userItem = IOTool.DeserializeFromString<UserItem>(PlayerPrefs.GetString(GlobalVars.PREF_USER_ITEM));
            _headIndex = userItem.headIndex;
            _userName = userItem.userName;
        }

        public void SaveAndClearUserInfo()
        {
            PlayerPrefs.SetString(GlobalVars.PREF_USER_ITEM, IOTool.SerializeToString<UserItem>(Self));
            _headIndex = 0;
            _userName = "";
        }


        #endregion
    }
}

