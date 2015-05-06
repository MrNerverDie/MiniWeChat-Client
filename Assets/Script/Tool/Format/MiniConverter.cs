using UnityEngine;
using System;
using System.Collections;
using protocol;

namespace MiniWeChat
{
    public class MiniConverter
    {
        public static int BytesToInt(byte[] bytes, int startIndex)
        {
            Array.Reverse(bytes, startIndex, 4);
            return BitConverter.ToInt32(bytes, startIndex);
        }

        public static byte[] IntToBytes(int value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            Array.Reverse(bytes);
            return bytes;
        }

        public static ChatDataItem ChatItemToDataItem(ChatItem chatItem)
        {
            return new ChatDataItem
            {
                sendUserId = chatItem.sendUserId,
                receiveUserId = chatItem.receiveUserId,
                chatType = ((ChatDataItem.ChatType)(uint)chatItem.chatType),
                chatBody = chatItem.chatBody,
                date = TimeJavaToCSharp(chatItem.date),
                targetType = ((ChatDataItem.TargetType)(uint)chatItem.targetType),
            };
        }

        public static ChatItem ChatDataItemToItem(ChatDataItem chatDataItem)
        {
            return new ChatItem
            {
                sendUserId = chatDataItem.sendUserId,
                receiveUserId = chatDataItem.receiveUserId,
                chatType = ((ChatItem.ChatType)(uint)chatDataItem.chatType),
                chatBody = chatDataItem.chatBody,
                date = chatDataItem.date,
                targetType = (ChatItem.TargetType)(uint)chatDataItem.targetType,
            };
        }

        public static long TimeJavaToCSharp(long java_time)
        {
            DateTime dt_1970 = new DateTime(1970, 1, 1, 0, 0, 0);
            long ticks_1970 = dt_1970.Ticks;
            long time_ticks = ticks_1970 + java_time * 10000;
            DateTime dt = new DateTime(time_ticks).AddHours(8);
            return dt.Ticks;
        }
    }
}


