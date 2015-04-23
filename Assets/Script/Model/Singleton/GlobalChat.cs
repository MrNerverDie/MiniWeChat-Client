using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.IO;
using protocol;

namespace MiniWeChat
{
    public class GlobalChat : Singleton<GlobalChat>
    {
        private Dictionary<string, ChatLog> _chatLogDict;
        private Dictionary<string, ChatDataItem> _waitSendChatDict;
        private List<ChatLog> _sortedChatLogList = new List<ChatLog>();

        public int Count
        {
            get { return _chatLogDict.Count; }
        }

#region LifeCycle

        public override void Init()
        {
            base.Init();
            MessageDispatcher.GetInstance().RegisterMessageHandler((uint)ENetworkMessage.RECEIVE_CHAT_SYNC, OnReceiveChatSync);
            MessageDispatcher.GetInstance().RegisterMessageHandler((uint)ENetworkMessage.SEND_CHAT_RSP, OnSendChatRsp);
            MessageDispatcher.GetInstance().RegisterMessageHandler((uint)EModelMessage.SEND_CHAT_TIMEOUT, OnSendChatTimeOut);
            MessageDispatcher.GetInstance().RegisterMessageHandler((uint)ENetworkMessage.LOGIN_RSP, OnLoginRsp);
            MessageDispatcher.GetInstance().RegisterMessageHandler((uint)ENetworkMessage.LOGOUT_RSP, OnLogOutRsp);
            MessageDispatcher.GetInstance().RegisterMessageHandler((uint)ENetworkMessage.OFFLINE_SYNC, OnLogOutRsp);
            MessageDispatcher.GetInstance().RegisterMessageHandler((uint)EModelMessage.TRY_LOGIN, OnTryLogin);
            MessageDispatcher.GetInstance().RegisterMessageHandler((uint)ENetworkMessage.CHANGE_FRIEND_SYNC, OnChangeFriendSync);

            _chatLogDict = new Dictionary<string, ChatLog>();
            _waitSendChatDict = new Dictionary<string, ChatDataItem>();

            LoadLogDict();
        }

        public override void Release()
        {
            base.Release();
            MessageDispatcher.GetInstance().UnRegisterMessageHandler((uint)ENetworkMessage.RECEIVE_CHAT_SYNC, OnReceiveChatSync);
            MessageDispatcher.GetInstance().UnRegisterMessageHandler((uint)ENetworkMessage.SEND_CHAT_RSP, OnSendChatRsp);
            MessageDispatcher.GetInstance().UnRegisterMessageHandler((uint)EModelMessage.SEND_CHAT_TIMEOUT, OnSendChatTimeOut);
            MessageDispatcher.GetInstance().UnRegisterMessageHandler((uint)ENetworkMessage.LOGIN_RSP, OnLoginRsp);
            MessageDispatcher.GetInstance().UnRegisterMessageHandler((uint)ENetworkMessage.LOGOUT_RSP, OnLogOutRsp);
            MessageDispatcher.GetInstance().UnRegisterMessageHandler((uint)ENetworkMessage.OFFLINE_SYNC, OnLogOutRsp);
            MessageDispatcher.GetInstance().UnRegisterMessageHandler((uint)EModelMessage.TRY_LOGIN, OnTryLogin);
            MessageDispatcher.GetInstance().UnRegisterMessageHandler((uint)ENetworkMessage.CHANGE_FRIEND_SYNC, OnChangeFriendSync);

            SaveAndClearLogDict();
        }

        public void OnApplicationPause(bool pause)
        {
            if (pause)
            {
                SaveLogDict();                
            }
        }

#endregion

        /// <summary>
        /// 通过chatID来获取ChatLog
        /// </summary>
        /// <param name="chatID">当chatLog为群聊的时候，chatID为群ID，否则为userID</param>
        /// <returns></returns>
        public ChatLog GetChatLog(string chatID)
        {
            if (!_chatLogDict.ContainsKey(chatID))
            {
                ChatLog chatLog = new ChatLog
                {
                    chatID = chatID,
                };

                _chatLogDict.Add(chatID, chatLog);
            }

            return _chatLogDict[chatID];
        }

        public void RemoveChatLog(string chatID)
        {
            _chatLogDict.Remove(chatID);
        }

        /// <summary>
        /// 通过chatID和index来获取ChatDataItem
        /// </summary>
        /// <param name="chatID">当chatLog为群聊的时候，chatID为群ID，否则为userID</param>
        /// <param name="index">chatDataItem在ChatLog中的index</param>
        /// <returns></returns>
        public ChatDataItem GetChatDataItem(string chatID, int index)
        {
            return GetChatLog(chatID).itemList[index];
        }

        private static int SortChatLogByDate(ChatLog c1, ChatLog c2)
        {
            return -(int)(c1.date - c2.date);
        }

        public void SendChatReq(ChatDataItem chatDataItem)
        {
            SendChatReq req = new SendChatReq
            {
                chatData = MiniConverter.ChatDataItemToItem(chatDataItem),
            };

            AddChatDataItem(chatDataItem);

            string msgID = NetworkManager.GetInstance().SendPacket<SendChatReq>(ENetworkMessage.SEND_CHAT_REQ, req, (uint)EModelMessage.SEND_CHAT_TIMEOUT);
            _waitSendChatDict.Add(msgID, chatDataItem);
        }

        private void AddChatDataItem(ChatDataItem chatDataItem)
        {
            string chatID = null;

            if (chatDataItem.targetType == ChatDataItem.TargetType.INDIVIDUAL)
	        {
                if (chatDataItem.sendUserId == GlobalUser.GetInstance().UserId)
                {
                    chatID = chatDataItem.receiveUserId;
                }
                else
                {
                    chatID = chatDataItem.sendUserId;
                }
            }
            else
            {
                chatID = chatDataItem.receiveUserId;
            }


            if (!_chatLogDict.ContainsKey(chatID))
            {
                ChatLog chatLog = new ChatLog
                {
                    chatID = chatID,
                };

                _chatLogDict.Add(chatID, chatLog);
            }

            _chatLogDict[chatID].itemList.Remove(chatDataItem);

            _chatLogDict[chatID].date = chatDataItem.date;
            _chatLogDict[chatID].targetType = chatDataItem.targetType;
            _chatLogDict[chatID].itemList.Add(chatDataItem);
        }

        public void SortChatLog()
        {
            _sortedChatLogList.Clear();
            foreach (var chatLog in _chatLogDict.Values)
            {
                _sortedChatLogList.Add(chatLog);
            }
            _sortedChatLogList.Sort(SortChatLogByDate);
        }

        public List<ChatLog>.Enumerator GetEnumerator()
        {
            return _sortedChatLogList.GetEnumerator();
        }

        public ChatDataItem GetLastChat(string userId)
        {
            List<ChatDataItem> itemList = GetChatLog(userId).itemList;
            if (itemList.Count > 0)
            {
                return itemList[itemList.Count - 1];
            }
            else
            {
                return null;
            }
        }

        #region MessageHandler

        public void OnReceiveChatSync(uint iMessageType, object kParam)
        {
            ReceiveChatSync rsp = kParam as ReceiveChatSync;
            foreach (var chatItem in rsp.chatData)
            {
                AddChatDataItem(MiniConverter.ChatItemToDataItem(chatItem));
            }
            MessageDispatcher.GetInstance().DispatchMessage((uint)EUIMessage.UPDATE_CHAT_LIST, null);
            MessageDispatcher.GetInstance().DispatchMessage((uint)EUIMessage.UPDATE_RECEIVE_CHAT, null);
        }

        public void OnSendChatRsp(uint iMessageType, object kParam)
        {
            NetworkMessageParam param = kParam as NetworkMessageParam;
            SendChatRsp rsp = param.rsp as SendChatRsp;
            if (rsp.resultCode == SendChatRsp.ResultCode.SUCCESS)
            {
                int index = -1;
                if (_waitSendChatDict.ContainsKey(param.msgID))
                {
                    SendChatReq req = param.req as SendChatReq;
                    index = _chatLogDict[req.chatData.receiveUserId].itemList.LastIndexOf(_waitSendChatDict[param.msgID]);
                    _waitSendChatDict[param.msgID].isSend = true;
                    _waitSendChatDict.Remove(param.msgID);                
                }
                MessageDispatcher.GetInstance().DispatchMessage((uint)EUIMessage.UPDATE_SEND_CHAT, index);
            }
        }

        public void OnSendChatTimeOut(uint iMessageType, object kParam)
        {
            NetworkMessageParam param = kParam as NetworkMessageParam;
            int index = -1;
            if (_waitSendChatDict.ContainsKey(param.msgID))
            {
                SendChatReq req = param.req as SendChatReq;
                index = _chatLogDict[req.chatData.receiveUserId].itemList.LastIndexOf(_waitSendChatDict[param.msgID]);
                _waitSendChatDict[param.msgID].isSend = false;
                _waitSendChatDict.Remove(param.msgID);
            }
            MessageDispatcher.GetInstance().DispatchMessage((uint)EUIMessage.UPDATE_SEND_CHAT, index);
        }

        public void OnLoginRsp(uint iMessageType, object kParam)
        {
            LoginRsp rsp = kParam as LoginRsp;
            if (rsp.resultCode == LoginRsp.ResultCode.SUCCESS)
            {
                LoadLogDict();
            }
        }

        public void OnLogOutRsp(uint iMessageType, object kParam)
        {
            SaveAndClearLogDict();
        }

        public void OnTryLogin(uint iMessageType, object kParam)
        {
            LoadLogDict();
        }

        public void OnChangeFriendSync(uint iMessageType, object kParam)
        {
            ChangeFriendSync rsp = kParam as ChangeFriendSync;
            string userID = rsp.userItem.userId;
            if (rsp.changeType == ChangeFriendSync.ChangeType.ADD)
            {
                if (!_chatLogDict.ContainsKey(userID))
                {
                    ChatLog chatLog = new ChatLog
                    {
                        chatID = userID,
                    };

                    _chatLogDict.Add(userID, chatLog);
                }
            }
            else if (rsp.changeType == ChangeFriendSync.ChangeType.DELETE)
            {
                if (_chatLogDict.ContainsKey(userID))
                {
                    _chatLogDict.Remove(userID);
                }
            }
        }

        #endregion

        #region LocalData

        private string GetChatDirPath()
        {
            return GlobalUser.GetInstance().GetUserDir() + "/Chat";
        }

        private void SaveLogDict()
        {
            foreach (var userID in _chatLogDict.Keys)
            {
                string filePath = GetChatDirPath() + "/" + userID;
                IOTool.SerializeToFile<ChatLog>(filePath, _chatLogDict[userID]);
            }
        }

        private void SaveAndClearLogDict()
        {
            SaveLogDict();
            ClearLogDict();
        }

        private void LoadLogDict()
        {
            if (IOTool.IsDirExist(GetChatDirPath()))
            {
                foreach (var file in IOTool.GetFiles(GetChatDirPath()))
                {
                    ChatLog chatLog = IOTool.DeserializeFromFile<ChatLog>(file.FullName);
                    if (chatLog != null)
                    {
                        _chatLogDict[chatLog.chatID] = chatLog;
                        Log4U.LogDebug("chatID", chatLog.chatID);
                    }
                }
            }
        }

        public void ClearLogDict()
        {
            _chatLogDict.Clear();
        }

        #endregion
    }
}

