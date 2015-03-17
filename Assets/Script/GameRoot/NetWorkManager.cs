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
        private TcpClient client;

        private const int HEAD_SIZE = 4;
        private const int HEAD_NUM = 2;

        public override void Init()
        {
            Debug.Log("Client Running...");
            try {
                client = new TcpClient();
                client.Connect("192.168.45.55", 8080); 
            } catch (Exception ex) {
                Debug.Log(ex.StackTrace);
                return;
            }

            KeepAliveSyncPacket reqPacket = new KeepAliveSyncPacket
            {
                a = 1,
                b = false,
                c = "1",
            };

            SendPacket<KeepAliveSyncPacket>(ENetworkMessage.KeepAliveSync, reqPacket);
        }

        public override void Release()
        {
            client.Close();
            Debug.Log("Client Close...");
        }

        private void ReceivePacket()
        {
            NetworkStream streamToServer = client.GetStream();
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
            Debug.Log(bufferSize);

            byte[] bufferSizeBytes = BitConverter.GetBytes(bufferSize);
            byte[] networkMessageBytes = BitConverter.GetBytes((int)networkMessage);
            Debug.Log((int)networkMessage);

            MemoryStream streamToServerBuffer = new MemoryStream();
            streamToServerBuffer.Write(bufferSizeBytes, 0, HEAD_SIZE);
            streamToServerBuffer.Write(networkMessageBytes, 0, HEAD_SIZE);
            streamForProto.WriteTo(streamToServerBuffer);

            streamToServerBuffer.WriteTo(client.GetStream());
        }

    }
}


