using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using protocol;

namespace MiniWeChat
{
    public class GlobalContacts : Singleton<GlobalContacts>
    {
        private Dictionary<string, UserItem> _friendDict;
        //private List<UserItem> _friendList;

	    public int Count
	    {
            get { return _friendDict.Count; }
	    }

        public override void Init()
        {
            base.Init();
            _friendDict = new Dictionary<string, UserItem>();

            MessageDispatcher.GetInstance().RegisterMessageHandler((uint)ENetworkMessage.GET_PERSONALINFO_RSP, OnGetPersonalInfoRsp);
            MessageDispatcher.GetInstance().RegisterMessageHandler((uint)ENetworkMessage.CHANGE_FRIEND_SYNC, OnChangeFriendSync);
            MessageDispatcher.GetInstance().RegisterMessageHandler((uint)ENetworkMessage.LOGOUT_RSP, OnLogOutRsp);
            MessageDispatcher.GetInstance().RegisterMessageHandler((uint)ENetworkMessage.OFFLINE_SYNC, OnLogOutRsp);
            MessageDispatcher.GetInstance().RegisterMessageHandler((uint)EGeneralMessage.ENTER_MAINMENU, OnEnterMainMenu);


            LoadFriendDict();
        }

        public override void Release()
        {
            base.Release();

            MessageDispatcher.GetInstance().UnRegisterMessageHandler((uint)ENetworkMessage.GET_PERSONALINFO_RSP, OnGetPersonalInfoRsp);
            MessageDispatcher.GetInstance().UnRegisterMessageHandler((uint)ENetworkMessage.CHANGE_FRIEND_SYNC, OnChangeFriendSync);
            MessageDispatcher.GetInstance().UnRegisterMessageHandler((uint)ENetworkMessage.LOGOUT_RSP, OnLogOutRsp);
            MessageDispatcher.GetInstance().UnRegisterMessageHandler((uint)ENetworkMessage.OFFLINE_SYNC, OnLogOutRsp);
            MessageDispatcher.GetInstance().UnRegisterMessageHandler((uint)EGeneralMessage.ENTER_MAINMENU, OnEnterMainMenu);


            SaveFriendDict();
        }



        public bool Contains(string userId)
        {
            return _friendDict.ContainsKey(userId);
        }

        public Dictionary<string, UserItem>.ValueCollection.Enumerator GetEnumerator()
        {
            return _friendDict.Values.GetEnumerator();
        }

        public UserItem GetUserItemById(string id)
        {
            return _friendDict[id];
        }

        #region MessageHandler
        public void OnGetPersonalInfoRsp(uint iMessageType, object kParam)
        {
            GetPersonalInfoRsp rsp = kParam as GetPersonalInfoRsp;
            if (rsp.resultCode == GetPersonalInfoRsp.ResultCode.SUCCESS
                && rsp.friends != null)
            {
                _friendDict.Clear();
                foreach (UserItem friend in rsp.friends)
                {
                    _friendDict[friend.userId] = friend;
                }
            }
        }

        public void OnChangeFriendSync(uint iMessageType, object kParam)
        {
            ChangeFriendSync rsp = kParam as ChangeFriendSync;
            if (rsp.changeType == ChangeFriendSync.ChangeType.ADD)
            {
                _friendDict.Add(rsp.userItem.userId, rsp.userItem);
            }
            else if (rsp.changeType == ChangeFriendSync.ChangeType.DELETE)
            {
                _friendDict.Remove(rsp.userItem.userId);
            }

            MessageDispatcher.GetInstance().DispatchMessage((uint)EUIMessage.UPDATE_FRIEND_DETAIL);
        }

        public void OnLogOutRsp(uint iMessageType, object kParam)
        {
            SaveFriendDict();
        }

        public void OnEnterMainMenu(uint iMessageType, object kParam)
        {
            LoadFriendDict();
        }

        #endregion

        #region LocalData
        private string GetContactsDirPath()
        {
            return GlobalUser.GetInstance().GetUserDir() + "/Contacts";
        }

        private void SaveFriendDict()
        {
            foreach (var userID in _friendDict.Keys)
            {
                string filePath = GetContactsDirPath() + "/" + userID;
                IOTool.SerializeToFile<UserItem>(filePath, _friendDict[userID]);
            }
            _friendDict.Clear();            
        }

        private void LoadFriendDict()
        {
            if (_friendDict.Count == 0 && IOTool.IsDirExist(GetContactsDirPath()))
            {
                foreach (var file in IOTool.GetFiles(GetContactsDirPath()))
                {
                    UserItem userItem = IOTool.DeserializeFromFile<UserItem>(file.FullName);
                    _friendDict[userItem.userId] = userItem;
                }
            }
        }
        #endregion

    }
}

