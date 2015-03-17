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
        private TcpClient _client;
        private NetworkStream _streamToServer;

        private byte[] _buffer;
        private const int BUFFER_SIZE = 8192;

        private const int HEAD_SIZE = 4;
        private const int HEAD_NUM = 2;

        public override void Init()
        {
            Debug.Log("Client Running...");
            try {
                _client = new TcpClient();
                _client.Connect("192.168.45.55", 8080); 
            } catch (Exception ex) {
                Debug.Log(ex.StackTrace);
                return;
            }

            _buffer = new byte[BUFFER_SIZE];
            _streamToServer = _client.GetStream();

            KeepAliveSyncPacket reqPacket = new KeepAliveSyncPacket
            {
                a = 1,
                b = false,
                c = "Hello MiniWeChat Server",
            };

            //for (int i = 0; i < 100; i++)
            //{
                SendPacket<KeepAliveSyncPacket>(ENetworkMessage.KeepAliveSync, reqPacket);                
            //}

            BeginReceivePacket();
        }

        public override void Release()
        {
            _client.Close();
            Debug.Log("Client Close...");
        }

        private void BeginReceivePacket()
        {
            lock(_streamToServer)
            {
                AsyncCallback callBack = new AsyncCallback(EndReceivePacket);
                _streamToServer.BeginRead(_buffer, 0, BUFFER_SIZE, callBack, null);
            }
        }

        private void EndReceivePacket(IAsyncResult ar)
        {
            int bytesRead = 0;
            try
            {
                lock (_streamToServer)
                {
                    bytesRead = _streamToServer.EndRead(ar);
                }
                Debug.Log("Raed Data Length is : " + bytesRead);

                int bufferSize = MiniConverter.BytesToInt(_buffer, HEAD_SIZE * 0);
                ENetworkMessage networkMessage = (ENetworkMessage)MiniConverter.BytesToInt(_buffer, HEAD_SIZE * 1);

                Debug.Log("bufferSize : " + bufferSize);
                Debug.Log("networkMessage : " + networkMessage);
                
                using(MemoryStream streamForProto = new MemoryStream(_buffer, HEAD_SIZE * HEAD_NUM, bufferSize - HEAD_SIZE * HEAD_NUM))
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

                Array.Clear(_buffer, 0, BUFFER_SIZE);

                BeginReceivePacket();
            }
            catch (Exception ex)
            {
                if (_streamToServer != null)
                    _streamToServer.Dispose();
                _client.Close();
                Debug.Log(ex.Message);
            }
        }

        /// <summary>
        /// 协议格式：
        /// SIZE ： 4 | TYPE ： 4 | PACKET ： dynamic
        /// </summary>
        /// <typeparam name="T">向服务器发送的packet的类型</typeparam>
        /// <param name="networkMessage">向服务器发送的请求的类型</param>
        /// <param name="packet">向服务器发送的packet</param>
        private void SendPacket<T>(ENetworkMessage networkMessage, T packet) where T : global::ProtoBuf.IExtensible
        {
            MemoryStream streamForProto = new MemoryStream();
            Serializer.Serialize<T>(streamForProto, packet);
            int bufferSize = HEAD_SIZE * HEAD_NUM + (int)streamForProto.Length;

            byte[] bufferSizeBytes = MiniConverter.IntToBytes(bufferSize);
            byte[] networkMessageBytes = MiniConverter.IntToBytes((int)networkMessage);

            MemoryStream streamToServerBuffer = new MemoryStream();
            streamToServerBuffer.Write(bufferSizeBytes, 0, HEAD_SIZE);
            streamToServerBuffer.Write(networkMessageBytes, 0, HEAD_SIZE);
            streamForProto.WriteTo(streamToServerBuffer);

            streamToServerBuffer.WriteTo(_streamToServer);
        }

    }
}


