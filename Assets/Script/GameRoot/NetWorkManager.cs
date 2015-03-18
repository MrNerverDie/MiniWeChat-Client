using UnityEngine;
using System;
using System.Collections;
using System.Net;
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
        private byte[] _sendBuffer;

        private const int HEAD_SIZE = 4;
        private const int HEAD_NUM = 2;

        private float CONNECT_TIME_OUT = 3.0f;

        #region LifeCycle
        public override void Init()
        {
            Debug.Log("Client Running...");

            BeginConnection();

        }

        private void DoInitNetWork()
        {
            _receiveBuffer = new byte[_socket.ReceiveBufferSize];
            _sendBuffer = new byte[_socket.SendBufferSize];

            KeepAliveSyncPacket reqPacket = new KeepAliveSyncPacket
            {
                a = 1,
                b = false,
                c = "Hello MiniWeChat Server",
            };

            //for (int i = 0; i < 100; i++)
            //{
            BeginSendPacket<KeepAliveSyncPacket>(ENetworkMessage.KeepAliveSync, reqPacket);
            //}

            BeginReceivePacket();
        }

        public override void Release()
        {
            if (_socket.Connected)
            {
                _socket.Shutdown(SocketShutdown.Both);
                _socket.Disconnect(true);
            }
            _socket.Close();
            Debug.Log("Client Close...");
        }

        #endregion

        #region Connection

        private void BeginConnection()
        {
            try
            {
                _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _socket.BeginConnect("192.168.45.38", 8080, new AsyncCallback(FinishConnection), null);
                Invoke("CheckConnectTimeOut", CONNECT_TIME_OUT);
                
            }
            catch (Exception ex)
            {
                Debug.Log(ex.StackTrace);
                return;
            }
        }

        private void FinishConnection(IAsyncResult ar)
        {
            _socket.EndConnect(ar);

            if (_socket.Connected)
            {
                DoInitNetWork();
            }
        }

        private void CheckConnectTimeOut()
        {
            if (_socket.Connected == false)
            {
                Debug.Log("Client Connect Time Out...");
                Release();
            }
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
                Release();
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

                int bufferSize = MiniConverter.BytesToInt(_receiveBuffer, HEAD_SIZE * 0);
                ENetworkMessage networkMessage = (ENetworkMessage)MiniConverter.BytesToInt(_receiveBuffer, HEAD_SIZE * 1);

                Debug.Log("bufferSize : " + bufferSize);
                Debug.Log("networkMessage : " + networkMessage);
                
                using(MemoryStream streamForProto = new MemoryStream(_receiveBuffer, HEAD_SIZE * HEAD_NUM, bufferSize - HEAD_SIZE * HEAD_NUM))
                {
                    switch (networkMessage)
                    {
                        case ENetworkMessage.KeepAliveSync:
                            KeepAliveSyncPacket packet = Serializer.Deserialize<KeepAliveSyncPacket>(streamForProto);
                            Debug.Log(packet.a + " " + packet.b + " " + packet.c);
                            break;
                        case ENetworkMessage.RegisterReq:
                            break;
                        case ENetworkMessage.RegisterRsp:
                            break;
                        case ENetworkMessage.LoginReq:
                            break;
                        case ENetworkMessage.LoginRsp:
                            break;
                        default:
                            break;
                    }
                }

                Array.Clear(_receiveBuffer, 0, _socket.ReceiveBufferSize);

                BeginReceivePacket();
            }
            catch (Exception ex)
            {
                Release();
                Debug.Log(ex.Message);
            }
        }

        #endregion


        #region SendPacket
        /// <summary>
        /// 协议格式：
        /// SIZE ： 4 | TYPE ： 4 | PACKET ： dynamic
        /// </summary>
        /// <typeparam name="T">向服务器发送的packet的类型</typeparam>
        /// <param name="networkMessage">向服务器发送的请求的类型</param>
        /// <param name="packet">向服务器发送的packet</param>
        private void BeginSendPacket<T>(ENetworkMessage networkMessage, T packet) where T : global::ProtoBuf.IExtensible
        {
            try
            {
                lock (_socket)
                {
                    MemoryStream streamForProto = new MemoryStream();
                    Serializer.Serialize<T>(streamForProto, packet);
                    int bufferSize = HEAD_SIZE * HEAD_NUM + (int)streamForProto.Length;

                    byte[] bufferSizeBytes = MiniConverter.IntToBytes(bufferSize);
                    byte[] networkMessageBytes = MiniConverter.IntToBytes((int)networkMessage);

                    Array.Copy(bufferSizeBytes, 0, _sendBuffer, HEAD_SIZE * 0, HEAD_SIZE);
                    Array.Copy(networkMessageBytes, 0, _sendBuffer, HEAD_SIZE * 1, HEAD_SIZE);
                    Array.Copy(streamForProto.ToArray(), 0, _sendBuffer, HEAD_SIZE * HEAD_NUM, streamForProto.Length);

                    _socket.BeginSend(_sendBuffer, 0, bufferSize, SocketFlags.None, new AsyncCallback(EndSendPacket), null);

                    streamForProto.Dispose();
                }
            }
            catch (Exception ex)
            {
                Release();
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

                if (bytesSend > 0)
                {
                    Array.Clear(_sendBuffer, 0, _socket.SendBufferSize);
                }
            }
            catch (Exception ex)
            {
                Release();
                Debug.Log(ex.Message);
            }
        }
        #endregion
    }
}


