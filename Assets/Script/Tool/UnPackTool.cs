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

            try
            {
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
                        case ENetworkMessage.CREATE_GROUP_CHAT_RSP:
                            packet = Serializer.Deserialize<CreateGroupChatRsp>(streamForProto);
                            break;
                        case ENetworkMessage.CHANGE_GROUP_CHAT_MEMBER_RSP:
                            packet = Serializer.Deserialize<ChangeGroupChatMemberRsq>(streamForProto);
                            break;
                        case ENetworkMessage.CHANGE_GROUP_CHAT_MEMBER_SYNC:
                            packet = Serializer.Deserialize<ChangeGroupChatMemberSync>(streamForProto);
                            break;
                        case ENetworkMessage.GET_GROUP_INFO_RSP:
                            packet = Serializer.Deserialize<GetGroupInfoRsp>(streamForProto);
                            break;
                        default:
                            Log4U.LogInfo("No Such Packet, packet type is " + networkMessage);
                            break;
                    }
                }
            }
            catch (System.Exception ex)
            {
                Log4U.LogError(ex.Message + "\n " + ex.StackTrace + "\n" + ex.Source);
            }

            return packet;
        }
    }
}

