using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace MiniWeChat
{
    public delegate void MessageHandler(uint iMessageType, object kParam);
    public class MessageArgs
    {
        public uint iMessageType;
        public object kParam;
    }

    public class MessageDispatcher : Singleton<MessageDispatcher>
    {

        Dictionary<uint, List<MessageHandler>> m_kMessageTable;

        /// <summary>
        /// 进行单例的初始化 //
        /// </summary>
        public override void Init()
        {
            m_kMessageTable = new Dictionary<uint, List<MessageHandler>>();
        }

        /// <summary>
        /// 对一个消息注册一个新的回调函数，如果这个消息
        /// 已经有该回调函数，则不会注册第二次
        /// </summary>
        /// <param name="iMessageType"></param>
        /// <param name="kHandler"></param>
        public void RegisterMessageHandler(uint iMessageType, MessageHandler kHandler)
        {
            if (!m_kMessageTable.ContainsKey(iMessageType))
            {
                m_kMessageTable.Add(iMessageType, new List<MessageHandler>());
            }
            List<MessageHandler> kHandlerList = m_kMessageTable[iMessageType];
            if (!kHandlerList.Contains(kHandler))
            {
                kHandlerList.Add(kHandler);                
            }
        }

        /// <summary>
        /// 对一个消息取消注册一个回调函数
        /// </summary>
        /// <param name="iMessageType"></param>
        /// <param name="kHandler"></param>
        public void UnRegisterMessageHandler(uint iMessageType, MessageHandler kHandler)
        {
            if (m_kMessageTable.ContainsKey(iMessageType))
            {
                List<MessageHandler> kHandlerList = m_kMessageTable[iMessageType];
                kHandlerList.Remove(kHandler);
            }
        }

        public void DispatchMessage(uint iMessageType, object kParam = null)
        {
            if (m_kMessageTable.ContainsKey(iMessageType))
            {
                List<MessageHandler> kHandlerList = m_kMessageTable[iMessageType];
                for (int i = 0; i < kHandlerList.Count; i++)
                {
                    ((MessageHandler)kHandlerList[i])(iMessageType, kParam);
                }
            }
        }

        public void DispatchMessage(MessageArgs kMessageArgs)
        {
            DispatchMessage(kMessageArgs.iMessageType, kMessageArgs.kParam);
        }
    }
}
