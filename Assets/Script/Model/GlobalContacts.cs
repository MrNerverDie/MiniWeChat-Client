using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using protocol;

namespace MiniWeChat
{
    public class GlobalContacts : Singleton<GlobalContacts>
    {
        private List<UserItem> _friendList;

	    public int Count
	    {
            get { return _friendList.Count; }
	    }

        public override void Init()
        {
            base.Init();
            _friendList = new List<UserItem>();

            MessageDispatcher.GetInstance().RegisterMessageHandler((uint)ENetworkMessage.GET_PERSONALINFO_RSP, OnGetPersonalInfoRsp);
            MessageDispatcher.GetInstance().RegisterMessageHandler((uint)ENetworkMessage.CHANGE_FRIEND_SYNC, OnChangeFriendSync);
        }

        public override void Release()
        {
            base.Release();

            MessageDispatcher.GetInstance().UnRegisterMessageHandler((uint)ENetworkMessage.GET_PERSONALINFO_RSP, OnGetPersonalInfoRsp);
            MessageDispatcher.GetInstance().UnRegisterMessageHandler((uint)ENetworkMessage.CHANGE_FRIEND_SYNC, OnChangeFriendSync);

        }

        public void OnGetPersonalInfoRsp(uint iMessageType, object kParam)
        {
            GetPersonalInfoRsp rsp = kParam as GetPersonalInfoRsp;
            if (rsp.resultCode == GetPersonalInfoRsp.ResultCode.SUCCESS
                && rsp.friends != null)
            {
                _friendList = rsp.friends;
            }
        }

        public bool Contains(string userId)
        {
            foreach (var userItem in _friendList)
            {
                if (userItem.userId == userId)
                {
                    return true;
                }
            }
            return false;
        }

        public List<UserItem>.Enumerator GetEnumerator()
        {
            return _friendList.GetEnumerator();
        }

        public UserItem GetUserItemByIndex(int index)
        {
            return _friendList[index];
        }

        public void OnChangeFriendSync(uint iMessageType, object kParam)
        {
            ChangeFriendSync rsp = kParam as ChangeFriendSync;
            if (rsp.changeType == ChangeFriendSync.ChangeType.ADD)
            {
                _friendList.Add(rsp.userItem);
            }
            else if (rsp.changeType == ChangeFriendSync.ChangeType.DELETE)
            {
                for (int i = 0; i < _friendList.Count; i++)
                {
                    if (rsp.userItem.userId == _friendList[i].userId)
                    {
                        _friendList.RemoveAt(i);
                        break;
                    }
                }
            }

            MessageDispatcher.GetInstance().DispatchMessage((uint)EUIMessage.UPDATE_FRIEND_DETAIL);
        }


    }
}

