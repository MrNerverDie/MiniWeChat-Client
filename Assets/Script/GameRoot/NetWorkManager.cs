using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using ProtoBuf;
using protocol;
using System.IO;

namespace MiniWeChat
{
    public class NetworkManager : Singleton<NetworkManager>
    {
        private Socket _socket;

        private byte[] _receiveBuffer;

        private const int HEAD_SIZE = 4;
        private const int HEAD_NUM = 3;

        private float CONNECT_TIME_OUT = 3.0f;
        private float REQ_TIME_OUT = 3.0f;
        private float KEEP_ALIVE_TIME_OUT = 5.0f;

        private bool _isKeepAlive = false;

        HashSet<string> _msgIDSet;

        Queue<MessageArgs> _receiveMessageQueue;
        private const float CHECK_QUEUE_DURATION = 0.1f;

        private HashSet<ENetworkMessage> _forcePushMessageType;

        #region LifeCycle
        public override void Init()
        {
            Debug.Log("Client Running...");

            _receiveMessageQueue = new Queue<MessageArgs>();
            _msgIDSet = new HashSet<string>();
            _forcePushMessageType = new HashSet<ENetworkMessage> 
            { 
                ENetworkMessage.KEEP_ALIVE_SYNC,
            };

            MessageDispatcher.GetInstance().RegisterMessageHandler((uint)EGeneralMessage.SOCKET_CONNECTED, OnSocketConnected);
            MessageDispatcher.GetInstance().RegisterMessageHandler((uint)ENetworkMessage.KEEP_ALIVE_SYNC, OnKeepAliveSync);

            StartCoroutine(BeginHandleReceiveMessageQueue());
            StartCoroutine(BeginTryConnect());
        }

        public override void Release()
        {
            MessageDispatcher.GetInstance().UnRegisterMessageHandler((uint)EGeneralMessage.SOCKET_CONNECTED, OnSocketConnected);
            MessageDispatcher.GetInstance().UnRegisterMessageHandler((uint)ENetworkMessage.KEEP_ALIVE_SYNC, OnKeepAliveSync);

            CloseConnection();
        }

        private IEnumerator BeginHandleReceiveMessageQueue()
        {
            while (true)
            {
                yield return new WaitForSeconds(CHECK_QUEUE_DURATION);
                lock (_receiveMessageQueue)
                {
                    while(_receiveMessageQueue.Count != 0)
                    {
                        MessageArgs args = _receiveMessageQueue.Dequeue();
                        MessageDispatcher.GetInstance().DispatchMessage(args);
                    }
                }
            }
        }

        private void OnSocketConnected(uint iMessageType, object kParam)
        {
            _receiveBuffer = new byte[_socket.ReceiveBufferSize];

            KeepAliveSyncPacket reqPacket = new KeepAliveSyncPacket
            {
                a = 1,
                b = false,
                c = "Hello MiniWeChat Server",
            };

            //for (int i = 0; i < 100; i++)
            //{
            //StartCoroutine(BeginSendPacket<KEEP_ALIVE_SYNCPacket>(ENetworkMessage.KEEP_ALIVE_SYNC, reqPacket));
            //}

            _isKeepAlive = true;

            BeginReceivePacket();
        }

        #endregion

        #region Connection

        private IEnumerator BeginConnection()
        {
            try
            {
                _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _socket.BeginConnect(GlobalVars.IPAddress, GlobalVars.IPPort, new AsyncCallback(FinishConnection), null);
            }
            catch (Exception ex)
            {
                Debug.Log(ex.StackTrace);
                yield break;
            }

            yield return new WaitForSeconds(CONNECT_TIME_OUT);

            if (_socket.Connected == false)
            {
                Debug.Log("Client Connect Time Out...");
                CloseConnection();
            }
        }

        private void FinishConnection(IAsyncResult ar)
        {
            _socket.EndConnect(ar);

            if (_socket.Connected)
            {
                lock(_receiveMessageQueue)
                {
                    MessageArgs args = new MessageArgs()
                    {
                        iMessageType = (uint)EGeneralMessage.SOCKET_CONNECTED,
                        kParam = null
                    };

                    _receiveMessageQueue.Enqueue(args);
                }
            }
        }

        private void CloseConnection()
        {
            if (_socket != null)
            {
                if (_socket.Connected)
                {
                    _socket.Shutdown(SocketShutdown.Both);
                    _socket.Disconnect(true);
                }
                _socket.Close();
            }
            Debug.Log("Client Close...");
        }

        private IEnumerator BeginTryConnect()
        {
            while (true)
            {
                if (_isKeepAlive == false)
                {
                    Debug.Log("Begin Reconnect...");
                    yield return StartCoroutine(BeginConnection());
                }
                _isKeepAlive = false;
                yield return new WaitForSeconds(KEEP_ALIVE_TIME_OUT);
            }
        }

        private void OnKeepAliveSync(uint iMessageType, object kParam)
        {
            _isKeepAlive = true;
        }

        #endregion

        #region ReceivePacket

        private void BeginReceivePacket()
        {
            try
            {
                lock (_socket)
                {
                    _socket.BeginReceive(_receiveBuffer, 0, _socket.ReceiveBufferSize, SocketFlags.None, new AsyncCallback(EndReceivePacket), null);
                }
            }
            catch (Exception ex)
            {
                CloseConnection();
                Debug.Log(ex.Message);
            }

        }

        private void EndReceivePacket(IAsyncResult ar)
        {
            int bytesRead = 0;
            try
            {
                lock (_socket)
                {
                    bytesRead = _socket.EndReceive(ar);
                }
                Debug.Log("Raed Data Length is : " + bytesRead);

                int position = 0;

                while (position < bytesRead)
                {
                    int bufferSize = MiniConverter.BytesToInt(_receiveBuffer, HEAD_SIZE * 0);
                    ENetworkMessage networkMessage = (ENetworkMessage)MiniConverter.BytesToInt(_receiveBuffer, HEAD_SIZE * 1);

                    byte[] msgIDBytes = new byte[HEAD_SIZE];
                    for (int i = 0; i < HEAD_SIZE; i++)
                    {
                        msgIDBytes[i] = _receiveBuffer[position + HEAD_SIZE * 2 + i];
                    }
                    string msgID = BitConverter.ToString(msgIDBytes);
                    
                    object param = UnPack(networkMessage, position + HEAD_SIZE * HEAD_NUM, bufferSize - HEAD_NUM * HEAD_SIZE);

                    MessageArgs args = new MessageArgs
                    {
                        iMessageType = (uint)networkMessage,
                        kParam = param,
                    };

                    lock (_msgIDSet)
                    {
                        if (_forcePushMessageType.Contains(networkMessage) || _msgIDSet.Contains(msgID))
                        {
                            lock (_receiveMessageQueue)
                            {
                                _receiveMessageQueue.Enqueue(args);
                            }
                        }

                        if (_msgIDSet.Contains(msgID))
                        {
                            _msgIDSet.Remove(msgID);
                        }
                    }
                    
                    if (_forcePushMessageType.Contains(networkMessage))
                    {

                        DoBeginSendPacket(networkMessage, msgIDBytes);
                    }

                    Debug.Log("bufferSize : " + bufferSize);
                    Debug.Log("networkMessage : " + networkMessage);
                    Debug.Log("msgID : " + msgID);

                    position += bufferSize;
                }
                


                Array.Clear(_receiveBuffer, 0, _socket.ReceiveBufferSize);

                BeginReceivePacket();
            }
            catch (Exception ex)
            {
                CloseConnection();
                Debug.Log(ex.Message);
            }
        }

        #endregion


        #region SendPacket

        public void SendPacket<T>(ENetworkMessage networkMessage, T packet, uint timeoutMessage = (uint)EGeneralMessage.REQ_TIMEOUT) where T : global::ProtoBuf.IExtensible
        {
            StartCoroutine(BeginSendPacket<T>(networkMessage, packet, timeoutMessage));
        }

        private IEnumerator BeginSendPacket<T>(ENetworkMessage networkMessage, T packet, uint timeoutMessage) where T : global::ProtoBuf.IExtensible
        {
            byte[] msgIDBytes = BitConverter.GetBytes(UnityEngine.Random.value);
            string msgID = BitConverter.ToString(msgIDBytes);


            lock(_msgIDSet)
            {
                _msgIDSet.Add(BitConverter.ToString(msgIDBytes));
            }

            DoBeginSendPacket<T>(networkMessage, packet, msgIDBytes);
            yield return new WaitForSeconds(REQ_TIME_OUT);

            lock (_msgIDSet)
            {
                if (_msgIDSet.Contains(msgID))
                {
                    _msgIDSet.Remove(msgID);
                    MessageDispatcher.GetInstance().DispatchMessage(timeoutMessage);
                    Debug.Log("Send Packet Type : " + networkMessage + " msgID : " + msgID + " timeout ");
                }
            }
        }

        /// <summary>
        /// 协议格式：
        /// SIZE ： 4 | TYPE ： 4 | MsgID : 4 | PACKET ： dynamic
        /// </summary>
        /// <typeparam name="T">向服务器发送的packet的类型</typeparam>
        /// <param name="networkMessage">向服务器发送的请求的类型</param>
        /// <param name="packet">向服务器发送的packet</param>
        private void DoBeginSendPacket<T>(ENetworkMessage networkMessage, T packet, byte[] msgID) where T : global::ProtoBuf.IExtensible
        {
            try
            {
                byte[] sendBuffer = new byte[_socket.SendBufferSize];

                MemoryStream streamForProto = new MemoryStream();
                Serializer.Serialize<T>(streamForProto, packet);
                int bufferSize = HEAD_SIZE * HEAD_NUM + (int)streamForProto.Length;

                byte[] bufferSizeBytes = MiniConverter.IntToBytes(bufferSize);
                byte[] networkMessageBytes = MiniConverter.IntToBytes((int)networkMessage);

                Array.Copy(bufferSizeBytes, 0, sendBuffer, HEAD_SIZE * 0, HEAD_SIZE);
                Array.Copy(networkMessageBytes, 0, sendBuffer, HEAD_SIZE * 1, HEAD_SIZE);
                Array.Copy(msgID, 0, sendBuffer, HEAD_SIZE * 2, HEAD_SIZE);
                Array.Copy(streamForProto.ToArray(), 0, sendBuffer, HEAD_SIZE * HEAD_NUM, streamForProto.Length);
                lock (_socket)
                {
                    _socket.BeginSend(sendBuffer, 0, bufferSize, SocketFlags.None, new AsyncCallback(EndSendPacket), null);
                }
                streamForProto.Dispose();

            }
            catch (Exception ex)
            {
                CloseConnection();
                Debug.Log(ex.Message);
            }
        }

        private void DoBeginSendPacket(ENetworkMessage networkMessage, byte[] msgID)
        {
            try
            {
                byte[] sendBuffer = new byte[HEAD_SIZE * HEAD_NUM];

                byte[] bufferSizeBytes = MiniConverter.IntToBytes(HEAD_SIZE * HEAD_NUM);
                byte[] networkMessageBytes = MiniConverter.IntToBytes((int)networkMessage);

                Array.Copy(bufferSizeBytes, 0, sendBuffer, HEAD_SIZE * 0, HEAD_SIZE);
                Array.Copy(networkMessageBytes, 0, sendBuffer, HEAD_SIZE * 1, HEAD_SIZE);
                Array.Copy(msgID, 0, sendBuffer, HEAD_SIZE * 2, HEAD_SIZE);

                lock (_socket)
                {
                    _socket.BeginSend(sendBuffer, 0, HEAD_SIZE * HEAD_NUM, SocketFlags.None, new AsyncCallback(EndSendPacket), null);
                }
            }
            catch (Exception ex)
            {
                CloseConnection();
                Debug.Log(ex.Message);
            }
        }

        private void EndSendPacket(IAsyncResult ar)
        {
            int bytesSend = 0;
            try
            {
                lock (_socket)
                {
                    bytesSend = _socket.EndSend(ar);
                }
            }
            catch (Exception ex)
            {
                CloseConnection();
                Debug.Log(ex.Message);
            }
        }
        #endregion

        #region UnPack
        private object UnPack(ENetworkMessage networkMessage, int startIndex, int length)
        {
            object packet = null;

            using (MemoryStream streamForProto = new MemoryStream(_receiveBuffer, startIndex, length))
            {
                switch (networkMessage)
                {
                    case ENetworkMessage.KEEP_ALIVE_SYNC:
                        packet = Serializer.Deserialize<KeepAliveSyncPacket>(streamForProto);
                        break;
                    case ENetworkMessage.REGISTER_REQ:
                        break;
                    case ENetworkMessage.REGISTER_RSP:
                        break;
                    case ENetworkMessage.LOGIN_REQ:
                        break;
                    case ENetworkMessage.LOGIN_RSP:
                        break;
                    default:
                        Debug.Log("No Such Packet, packet type is " + networkMessage);
                        break;
                }
            }

            return packet;
        }
        #endregion

    }
}


