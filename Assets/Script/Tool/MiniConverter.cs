using UnityEngine;
using System;
using System.Collections;

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
    }
}


