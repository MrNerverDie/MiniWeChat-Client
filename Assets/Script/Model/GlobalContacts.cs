using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using protocol;

namespace MiniWeChat
{
    public class GlobalContacts : Singleton<GlobalContacts>
    {
        private List<UserItem> _friendList;

        public override void Init()
        {
            base.Init();
        }

        public override void Release()
        {
            base.Release();
        }

        
    }
}

