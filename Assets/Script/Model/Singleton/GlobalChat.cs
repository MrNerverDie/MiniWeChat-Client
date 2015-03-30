using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;
using ProtoBuf;

namespace MiniWeChat
{
    public class GlobalChat : Singleton<GlobalChat>
    {
        Dictionary<string, ChatLog> _chatLogDict;

        private static readonly string _dirPath = Application.persistentDataPath + "/Chat";

        public int Count
        {
            get { return _chatLogDict.Count; }
        }

        public override void Init()
        {
            base.Init();

            _chatLogDict = new Dictionary<string, ChatLog>();
        }

        public override void Release()
        {
            base.Release();
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
                
            }
        }
    }
}

