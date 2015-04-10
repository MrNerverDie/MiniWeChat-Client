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
    public class NetworkMessageParam
    {
        public IExtensible req;
        public IExtensible rsp;
        public string msgID;
    }

    public class NetworkManager : Singleton<NetworkManager>
    {
        private Socket _socket;

        private byte[] _receiveBuffer;

        private const int HEAD_SIZE = 4;
        private const int HEAD_NUM = 3;

        private float CONNECT_TIME_OUT = 3.0f;
        private float REQ_TIME_OUT = 4.0f;
        private float KEEP_ALIVE_TIME_OUT = 6.0f;

        private bool _isKeepAlive = false;

        Dictionary<string, IExtensible> _msgIDDict;

        private HashSet<ENetworkMessage> _forcePushMessageType;
        private HashSet<ENetworkMessage> _needReqMessageType;

        public bool IsConncted
        {
            get { return _socket != null && _socket.Connected; }
        }

        #region LifeCycle
        public override void Init()
        {
            Debug.Log("Client Running...");

            _msgIDDict = new Dictionary<string, IExtensible>();
            InitForcePushMessageType();
            InitNeedReqMessageType();

            MessageDispatcher.GetInstance().RegisterMessageHandler((uint)EModelMessage.SOCKET_CONNECTED, OnSocketConnected);
            MessageDispatcher.GetInstance().RegisterMessageHandler((uint)EModelMessage.SOCKET_DISCONNECTED, OnSocketDisConnected);
            MessageDispatcher.GetInstance().RegisterMessageHandler((uint)ENetworkMessage.KEEP_ALIVE_SYNC, OnKeepAliveSync);

            MessageDispatcher.GetInstance().DispatchMessageAsync((uint)EModelMessage.SOCKET_DISCONNECTED, null);
        }

        public override void Release()
        {
            MessageDispatcher.GetInstance().UnRegisterMessageHandler((uint)EModelMessage.SOCKET_CONNECTED, OnSocketConnected);
            MessageDispatcher.GetInstance().UnRegisterMessageHandler((uint)EModelMessage.SOCKET_DISCONNECTED, OnSocketDisConnected);
            MessageDispatcher.GetInstance().UnRegisterMessageHandler((uint)ENetworkMessage.KEEP_ALIVE_SYNC, OnKeepAliveSync);

            CloseConnection();
        }



        public void OnSocketConnected(uint iMessageType, object kParam)
        {
            if (_receiveBuffer == null)
            {
                _receiveBuffer = new byte[_socket.ReceiveBufferSize];                
            }

            _isKeepAlive = true;

            BeginReceivePacket();
        }

        public void OnSocketDisConnected(uint iMessageType, object kParam)
        {
            MessageDispatcher.GetInstance().DispatchMessage((uint)ENetworkMessage.OFFLINE_SYNC, new OffLineSync { causeCode = OffLineSync.CauseCode.ANOTHER_LOGIN });

            StartCoroutine(BeginTryConnect());
        }

        #endregion

        #region Connection

        private IEnumerator BeginConnection()
        {
            try
            {
                _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
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

            _isKeepAlive = _socket.Connected;
        }

        private void FinishConnection(IAsyncResult ar)
        {
            _socket.EndConnect(ar);

            if (_socket.Connected)
            {
                MessageDispatcher.GetInstance().DispatchMessageAsync((uint)EModelMessage.SOCKET_CONNECTED, null);
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
                Debug.Log("Client Close...");
            }
        }

        /// <summary>
        /// 当无法接收到心跳包的时候尝试重新连接服务器
        /// </summary>
        /// <returns></returns>
        private IEnumerator BeginTryConnect()
        {
            yield return null;
            while (_socket == null || !_socket.Connected || !_isKeepAlive)
            {
                Debug.Log("Begin Connect...");
                CloseConnection();
                yield return StartCoroutine(BeginConnection());
            }

            while (_isKeepAlive)
            {
                _isKeepAlive = false;
                yield return new WaitForSeconds(KEEP_ALIVE_TIME_OUT);
            }

            MessageDispatcher.GetInstance().DispatchMessageAsync((uint)EModelMessage.SOCKET_DISCONNECTED, null);
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
                Debug.Log(ex.Message);
            }

        }

        private void EndReceivePacket(IAsyncResult ar)
        {
            int bytesRead = -1;
            try
            {
                if (IsConncted)
                {
                    lock (_socket)
                    {
                        bytesRead = _socket.EndReceive(ar);                              
                    }
                }

                if (bytesRead == -1)
                {
                    CloseConnection();
                    return;
                }
            }
            catch (ObjectDisposedException  ex)
            {
                Debug.Log("Receive Closed");
            }
            catch (Exception ex)
            {
                Debug.Log(ex.Message);
                Debug.Log(ex.StackTrace);
                Debug.Log(ex.Source);
            }

            // Begin Read //
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

                if (networkMessage != ENetworkMessage.KEEP_ALIVE_SYNC)
                {
                    Debug.Log("networkMessage : " + networkMessage + "msgID : " + msgID);
                }

                if (position + bufferSize > bytesRead)
                {
                    Debug.Log("Error receive packet, packet is too long : " + bufferSize);
                    break;
                }

                IExtensible rspPacket = UnPackTool.UnPack(networkMessage, position + HEAD_SIZE * HEAD_NUM, bufferSize - HEAD_NUM * HEAD_SIZE, _receiveBuffer);
                if (rspPacket == null)
                {
                    continue;
                }

                MessageArgs args = new MessageArgs
                {
                    iMessageType = (uint)networkMessage,
                    kParam = rspPacket,
                };

                NetworkMessageParam networkParam = new NetworkMessageParam
                {
                    rsp = rspPacket,
                    msgID = msgID,
                };

                lock (_msgIDDict)
                {
                    if (_msgIDDict.ContainsKey(msgID))
                    {
                        networkParam.req = _msgIDDict[msgID];
                    }

                    if (_needReqMessageType.Contains(networkMessage))
                    {
                        args.kParam = networkParam;
                    }

                    if (_forcePushMessageType.Contains(networkMessage) || _msgIDDict.ContainsKey(msgID))
                    {
                        MessageDispatcher.GetInstance().DispatchMessageAsync(args.iMessageType, args.kParam);
                    }

                    if (_msgIDDict.ContainsKey(msgID))
                    {
                        RemoveMsgID(msgID);
                    }
                }

                if (_forcePushMessageType.Contains(networkMessage))
                {

                    DoBeginSendPacket(networkMessage, msgIDBytes);
                }

                position += bufferSize;
            }

            Array.Clear(_receiveBuffer, 0, _socket.ReceiveBufferSize);

            BeginReceivePacket();
        }

        /// <summary>
        /// 配置需要回复服务器的消息类型
        /// </summary>
        private void InitForcePushMessageType()
        {
            _forcePushMessageType = new HashSet<ENetworkMessage> 
            { 
                ENetworkMessage.KEEP_ALIVE_SYNC,
                ENetworkMessage.OFFLINE_SYNC,
                ENetworkMessage.CHANGE_FRIEND_SYNC,
                ENetworkMessage.RECEIVE_CHAT_SYNC,
            };
        }

        /// <summary>
        /// 配置在Rsp包中同时需要Req包信息的消息类型
        /// </summary>
        private void InitNeedReqMessageType()
        {
            _needReqMessageType = new HashSet<ENetworkMessage>
            {
                ENetworkMessage.SEND_CHAT_RSP,
                ENetworkMessage.GET_PERSONALINFO_RSP,
            };
        }

        #endregion


        #region SendPacket

        public string SendPacket<T>(ENetworkMessage networkMessage, T packet, uint timeoutMessage = (uint)EModelMessage.REQ_TIMEOUT) where T : global::ProtoBuf.IExtensible
        {
            byte[] msgIDBytes = BitConverter.GetBytes(UnityEngine.Random.value);

            if (timeoutMessage == (uint)EModelMessage.REQ_TIMEOUT)
            {
                DialogManager.GetInstance().ShowLoadingDialog();
            }

            StartCoroutine(BeginSendPacket<T>(networkMessage, packet, timeoutMessage, msgIDBytes));

            return BitConverter.ToString(msgIDBytes);
        }

        private IEnumerator BeginSendPacket<T>(ENetworkMessage networkMessage, T packet, uint timeoutMessage, byte[] msgIDBytes) where T : global::ProtoBuf.IExtensible
        {
            string msgID = BitConverter.ToString(msgIDBytes);

            lock(_msgIDDict)
            {
                _msgIDDict.Add(BitConverter.ToString(msgIDBytes), packet);
            }

            Debug.Log("Send : " + networkMessage + " msgID : " + msgID);

            DoBeginSendPacket<T>(networkMessage, packet, msgIDBytes);
            yield return new WaitForSeconds(REQ_TIME_OUT);

            lock (_msgIDDict)
            {
                if (_msgIDDict.ContainsKey(msgID))
                {
                    RemoveMsgID(msgID);

                    NetworkMessageParam param = new NetworkMessageParam
                    {
                        msgID = msgID,
                        req = packet,
                    };
                    MessageDispatcher.GetInstance().DispatchMessage(timeoutMessage, param);
                    DialogManager.GetInstance().CreateSingleButtonDialog("Send Packet Type : " + networkMessage + " msgID : " + msgID + " timeout ");
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
                    if (_socket != null && _socket.Connected)
                    {
                        _socket.BeginSend(sendBuffer, 0, bufferSize, SocketFlags.None, new AsyncCallback(EndSendPacket), null);                        
                    }
                }
                streamForProto.Dispose();

            }
            catch (Exception ex)
            {
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
                Debug.Log(ex.Message);
            }
        }
        #endregion

        #region Misc

        private void RemoveMsgID(string msgID)
        {
            _msgIDDict.Remove(msgID);
            DialogManager.GetInstance().HideLoadingDialog();
        }

        #endregion
    }
}


