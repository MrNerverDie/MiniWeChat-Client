using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace MiniWeChat
{
    public class GlobalChat : Singleton<GlobalChat>
    {
        Dictionary<string, List<ChatDataItem>> _chatDict;

        public int Count
        {
            get { return _chatDict.Count; }
        }

        public override void Init()
        {
            base.Init();

            _chatDict = new Dictionary<string, List<ChatDataItem>>();
        }

        public override void Release()
        {
            base.Release();
        }

        public List<ChatDataItem> GetChatDataItemList(string userID)
        {
            return _chatDict[userID];
        }

        
        

    }
}

