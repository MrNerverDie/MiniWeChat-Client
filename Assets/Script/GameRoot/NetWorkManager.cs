using UnityEngine;
using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using ProtoBuf;
using protocol;

namespace MiniWeChat
{
    public class NetworkManager : Singleton<NetworkManager>
    {
        TcpClient client;
        NetworkStream streamToServer;

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
        }

        public void Update()
        {

        }

        public override void Release()
        {
            streamToServer.Dispose();
            client.Close();
            Debug.Log("Client Close...");
        }

    }
}


