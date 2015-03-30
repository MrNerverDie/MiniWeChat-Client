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
        Dictionary<string, ChatLog> _chatLogDict;
        List<ChatLog> _sortedChatLogList;

        private static readonly string _dirPath = Application.persistentDataPath + "/Chat";

        public int Count
        {
            get { return _chatLogDict.Count; }
        }

        public override void Init()
        {
            base.Init();
            MessageDispatcher.GetInstance().RegisterMessageHandler((uint)ENetworkMessage.RECEIVE_CHAT_SYNC, OnReceiveChatSync);

            _chatLogDict = new Dictionary<string, ChatLog>();
        }

        public override void Release()
        {
            base.Release();
            MessageDispatcher.GetInstance().UnRegisterMessageHandler((uint)ENetworkMessage.RECEIVE_CHAT_SYNC, OnReceiveChatSync);
        }

        public List<ChatDataItem> GetChatDataItemList(string userID)
        {
            return _chatLogDict[userID].itemList;
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

            _sortedChatLogList.Sort(SortChatLogByDate);
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

            NetworkManager.GetInstance().SendPacket<SendChatReq>(ENetworkMessage.SEND_CHAT_REQ, req);
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
                    date = chatDataItem.date,
                };

                _chatLogDict.Add(guestUserID, chatLog);
            }

            _chatLogDict[guestUserID].itemList.Add(chatDataItem);
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

        #endregion
    }
}

