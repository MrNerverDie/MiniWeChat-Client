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
        }

        public override void Release()
        {
            base.Release();

            MessageDispatcher.GetInstance().UnRegisterMessageHandler((uint)ENetworkMessage.GET_PERSONALINFO_RSP, OnGetPersonalInfoRsp);
            MessageDispatcher.GetInstance().UnRegisterMessageHandler((uint)ENetworkMessage.CHANGE_FRIEND_SYNC, OnChangeFriendSync);

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
        #endregion

    }
}

