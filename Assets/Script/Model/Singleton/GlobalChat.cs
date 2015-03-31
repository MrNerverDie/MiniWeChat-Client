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
        private List<ChatLog> _sortedChatLogList;

        private static readonly string _dirPath = Application.persistentDataPath + "/Chat";

        public int Count
        {
            get { return _chatLogDict.Count; }
        }

        public override void Init()
        {
            base.Init();
            MessageDispatcher.GetInstance().RegisterMessageHandler((uint)ENetworkMessage.RECEIVE_CHAT_SYNC, OnReceiveChatSync);
            MessageDispatcher.GetInstance().RegisterMessageHandler((uint)ENetworkMessage.SEND_CHAT_RSP, OnSendChatRsp);

            _chatLogDict = new Dictionary<string, ChatLog>();
            _waitSendChatDict = new Dictionary<string, ChatDataItem>();
            _sortedChatLogList = new List<ChatLog>();
        }

        public override void Release()
        {
            base.Release();
            MessageDispatcher.GetInstance().UnRegisterMessageHandler((uint)ENetworkMessage.RECEIVE_CHAT_SYNC, OnReceiveChatSync);
            MessageDispatcher.GetInstance().UnRegisterMessageHandler((uint)ENetworkMessage.SEND_CHAT_RSP, OnSendChatRsp);
        }

        public ChatLog GetChatLog(string userID)
        {
            return _chatLogDict[userID];
        }

        private void SaveLogDict()
        {
            foreach (var userID in _chatLogDict.Keys)
            {
                string filePath = _dirPath + "/" + userID;
                IOTool.SerializeToFile<ChatLog>(filePath, _chatLogDict[userID]);
            }
        }

        private void InitLogDict()
        {
            DirectoryInfo dirInfo = new DirectoryInfo(_dirPath);
            foreach (var item in dirInfo.GetFiles())
            {
                ChatLog chatLog = IOTool.DeserializeFromFile<ChatLog>(item.FullName);
                _sortedChatLogList.Add(chatLog);
                _chatLogDict[chatLog.userId] = chatLog;
            }
        }

        private static int SortChatLogByDate(ChatLog c1, ChatLog c2)
        {
            return - (int)(c1.date - c2.date);
        }

        public void SendChatReq(ChatDataItem chatDataItem)
        {
            SendChatReq req = new SendChatReq
            {
                chatData = MiniConverter.ChatDataItemToItem(chatDataItem),
            };

            AddChatDataItem(chatDataItem);

            string msgID = NetworkManager.GetInstance().SendPacket<SendChatReq>(ENetworkMessage.SEND_CHAT_REQ, req);
            _waitSendChatDict.Add(msgID, chatDataItem);
        }

        private void AddChatDataItem(ChatDataItem chatDataItem)
        {
            string guestUserID = chatDataItem.sendUserId;

            if (chatDataItem.sendUserId == GlobalUser.GetInstance().UserId)
            {
                guestUserID = chatDataItem.receiveUserId;
            }

            if (!_chatLogDict.ContainsKey(guestUserID))
            {
                ChatLog chatLog = new ChatLog
                {
                    userId = guestUserID,
                };

                _chatLogDict.Add(guestUserID, chatLog);
            }

            _chatLogDict[guestUserID].itemList.Remove(chatDataItem);

            _chatLogDict[guestUserID].date = chatDataItem.date;
            _chatLogDict[guestUserID].itemList.Add(chatDataItem);
        }

        public List<ChatLog>.Enumerator GetEnumerator()
        {
            _sortedChatLogList.Sort(SortChatLogByDate);
            return _sortedChatLogList.GetEnumerator();
        }

        public ChatDataItem GetLastChat(string userId)
        {
            List<ChatDataItem> itemList = GetChatLog(userId).itemList;
            return itemList[itemList.Count - 1];
        }

        #region MessageHandler

        public void OnReceiveChatSync(uint iMessageType, object kParam)
        {
            ReceiveChatSync rsp = kParam as ReceiveChatSync;
            foreach (var chatItem in rsp.chatData)
            {
                AddChatDataItem(MiniConverter.ChatItemToDataItem(chatItem));
            }
        }

        public void OnSendChatRsp(uint iMessageType, object kParam)
        {
            NetworkMessageParam param = kParam as NetworkMessageParam;
            SendChatRsp rsp = param.rsp as SendChatRsp;
            if (rsp.resultCode == SendChatRsp.ResultCode.SUCCESS)
            {
                if (_waitSendChatDict.ContainsKey(param.msgID))
                {
                    _waitSendChatDict[param.msgID].isSend = true;
                    _waitSendChatDict.Remove(param.msgID);                    
                }
            }
        }

        #endregion
    }
}

