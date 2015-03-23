using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using protocol;

namespace MiniWeChat
{
    public class GlobalUser : Singleton<GlobalUser>
    {
        public string _userId;
        public string _userPassword;
        public string _userName;

        public override void Init()
        {
            MessageDispatcher.GetInstance().RegisterMessageHandler((uint)ENetworkMessage.LOGIN_RSP, OnLoginRsp);
            MessageDispatcher.GetInstance().RegisterMessageHandler((uint)ENetworkMessage.LOGIN_RSP, OnLoginRsp);
        }

        public override void Release()
        {
            MessageDispatcher.GetInstance().UnRegisterMessageHandler((uint)ENetworkMessage.LOGIN_RSP, OnLoginRsp);
            MessageDispatcher.GetInstance().UnRegisterMessageHandler((uint)ENetworkMessage.LOGIN_RSP, OnLoginRsp);
        }

        public void OnLoginRsp(uint iMessageType, object kParam)
        {
            
        }

        
    }
}

