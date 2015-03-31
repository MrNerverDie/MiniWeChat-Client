using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using protocol;
using ProtoBuf;
using System.IO;

namespace MiniWeChat
{
    public class UnPackTool
    {
        public static IExtensible UnPack(ENetworkMessage networkMessage, int startIndex, int length, byte[] buffer)
        {
            IExtensible packet = null;

            using (MemoryStream streamForProto = new MemoryStream(buffer, startIndex, length))
            {
                switch (networkMessage)
                {
                    case ENetworkMessage.KEEP_ALIVE_SYNC:
                        packet = Serializer.Deserialize<KeepAliveSyncPacket>(streamForProto);
                        break;
                    case ENetworkMessage.REGISTER_RSP:
                        packet = Serializer.Deserialize<RegisterRsp>(streamForProto);
                        break;
                    case ENetworkMessage.LOGIN_RSP:
                        packet = Serializer.Deserialize<LoginRsp>(streamForProto);
                        break;
                    case ENetworkMessage.GET_USERINFO_RSP:
                        packet = Serializer.Deserialize<GetUserInfoRsp>(streamForProto);
                        break;
                    case ENetworkMessage.PERSONALSETTINGS_RSP:
                        packet = Serializer.Deserialize<PersonalSettingsRsp>(streamForProto);
                        break;
                    case ENetworkMessage.LOGOUT_RSP:
                        packet = Serializer.Deserialize<LogoutRsp>(streamForProto);
                        break;
                    case ENetworkMessage.ADD_FRIEND_RSP:
                        packet = Serializer.Deserialize<AddFriendRsp>(streamForProto);
                        break;
                    case ENetworkMessage.DELETE_FRIEND_RSP:
                        packet = Serializer.Deserialize<DeleteFriendRsp>(streamForProto);
                        break;
                    case ENetworkMessage.OFFLINE_SYNC:
                        packet = Serializer.Deserialize<OffLineSync>(streamForProto);
                        break;
                    case ENetworkMessage.GET_PERSONALINFO_RSP:
                        packet = Serializer.Deserialize<GetPersonalInfoRsp>(streamForProto);
                        break;
                    case ENetworkMessage.CHANGE_FRIEND_SYNC:
                        packet = Serializer.Deserialize<ChangeFriendSync>(streamForProto);
                        break;
                    case ENetworkMessage.SEND_CHAT_RSP:
                        packet = Serializer.Deserialize<SendChatRsp>(streamForProto);
                        break;
                    case ENetworkMessage.RECEIVE_CHAT_SYNC:
                        packet = Serializer.Deserialize<ReceiveChatSync>(streamForProto);
                        break;
                    default:
                        Debug.Log("No Such Packet, packet type is " + networkMessage);
                        break;
                }
            }

            return packet;
        }
    }
}

