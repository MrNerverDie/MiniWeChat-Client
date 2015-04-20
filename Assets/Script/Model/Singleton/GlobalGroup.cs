using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

using protocol;

namespace MiniWeChat
{
    public class GlobalGroup : Singleton<GlobalGroup>
    {
        private Dictionary<string, GroupItem> _groupDict = new Dictionary<string,GroupItem>();
        private Dictionary<string, UserItem> _groupMemberDict = new Dictionary<string, UserItem>();

        #region LifeCycle
        
        public override void Init()
        {
            MessageDispatcher.GetInstance().RegisterMessageHandler((uint)ENetworkMessage.GET_PERSONALINFO_RSP, OnGetPersonalInfoRsp);

            base.Init();
        }

        public override void Release()
        {
            MessageDispatcher.GetInstance().UnRegisterMessageHandler((uint)ENetworkMessage.GET_PERSONALINFO_RSP, OnGetPersonalInfoRsp);

            base.Release();
        }

        #endregion

        #region QueryData

        public UserItem GetGroupMember(string userID)
        {
            if (!_groupMemberDict.ContainsKey(userID))
            {
                return _groupMemberDict[userID];
            }
            else 
	        {
                UserItem userItem = GlobalContacts.GetInstance().GetUserItemById(userID);

                if (userItem == null)
                {
                    //NetworkManager.GetInstance().SendPacket<Get>
                }

                return userItem;
            }
        }

        public bool ContainsMember(string groupID, string userID)
        {
            GroupItem group = GetGroup(groupID);
            return false;
        }

        public GroupItem GetGroup(string groupID)
        {
            if (!_groupDict.ContainsKey(groupID))
            {
                return _groupDict[groupID];
            }
            else
            {
                throw new UnityException("No such group : " + groupID);
            }
        }

        #endregion

        #region MessageHandler

        public void OnGetPersonalInfoRsp(uint iMessageType, object kParam)
        {
            NetworkMessageParam param = kParam as NetworkMessageParam;
            GetPersonalInfoRsp rsp = param.rsp as GetPersonalInfoRsp;
            GetPersonalInfoReq req = param.req as GetPersonalInfoReq;
            if (rsp.resultCode == GetPersonalInfoRsp.ResultCode.SUCCESS
                && req.friendInfo)
            {
                _groupDict.Clear();
                //foreach (UserItem friend in rsp.friends)
                //{
                //    _groupDict[friend.userId] = friend;
                //}
            }
        }

        #endregion

        #region LocalData

        private string GetGroupDirPath()
        {
            return GlobalUser.GetInstance().GetUserDir() + "/Group";
        }

        private void SaveGroupData()
        {
            foreach (var groupID in _groupDict.Keys)
            {
                string filePath = GetGroupDirPath() + "/" + groupID;
                IOTool.SerializeToFile<GroupItem>(filePath, _groupDict[groupID]);
            }
        }

        private void SaveAndClearFriendDict()
        {
            SaveGroupData();
            ClearGroupData();
        }

        private void LoadGroupData()
        {
            if (_groupDict.Count == 0 && IOTool.IsDirExist(GetGroupDirPath()))
            {
                foreach (var file in IOTool.GetFiles(GetGroupDirPath()))
                {
                    GroupItem groupItem = IOTool.DeserializeFromFile<GroupItem>(file.FullName);
                    if (groupItem != null)
                    {
                        _groupDict[groupItem.groupId] = groupItem;
                    }
                }
            }
        }

        public void ClearGroupData()
        {
            _groupDict.Clear();
        }

        #endregion
    }
}

