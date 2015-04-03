using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace MiniWeChat
{
    /// <summary>
    /// 所有消息回调函数必须遵守的委托定义
    /// </summary>
    /// <param name="iMessageType"></param>
    /// <param name="kParam"></param>
    public delegate void MessageHandler(uint iMessageType, object kParam);
    public class MessageArgs
    {
        public uint iMessageType;
        public object kParam;
    }

    public class MessageDispatcher : Singleton<MessageDispatcher>
    {
        private const float CHECK_QUEUE_DURATION = 0.1f;

        Dictionary<uint, List<MessageHandler>> m_kMessageTable;
        Queue<MessageArgs> _receiveMessageQueue;

        /// <summary>
        /// 进行单例的初始化 //
        /// </summary>
        public override void Init()
        {
            m_kMessageTable = new Dictionary<uint, List<MessageHandler>>();
            _receiveMessageQueue = new Queue<MessageArgs>();

            StartCoroutine(BeginHandleReceiveMessageQueue());
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

        /// <summary>
        /// 分发消息，同步
        /// </summary>
        /// <param name="iMessageType">消息类型</param>
        /// <param name="kParam">附加参数</param>
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

        /// <summary>
        /// 分发消息，异步，会在协程BeginHandleReceiveMessageQueue的下一次检查中进行真正的消息分发
        /// </summary>
        /// <param name="iMessageType">消息类型</param>
        /// <param name="kParam">附加参数</param>
        public void DispatchMessageAsync(uint iMessageType, object kParam = null)
        {
            lock (_receiveMessageQueue)
            {
                MessageArgs args = new MessageArgs()
                {
                    iMessageType = iMessageType,
                    kParam = kParam,
                };

                _receiveMessageQueue.Enqueue(args);
            }
        }

        private IEnumerator BeginHandleReceiveMessageQueue()
        {
            while (true)
            {
                yield return new WaitForSeconds(CHECK_QUEUE_DURATION);
                lock (_receiveMessageQueue)
                {
                    while (_receiveMessageQueue.Count != 0)
                    {
                        MessageArgs args = _receiveMessageQueue.Dequeue();
                        DispatchMessage(args.iMessageType, args.kParam);
                    }
                }
            }
        }

    }
}
