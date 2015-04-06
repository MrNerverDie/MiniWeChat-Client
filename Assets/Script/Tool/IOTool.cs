using UnityEngine;
using UnityEngine.UI;

using System.Collections.Generic;
using System.IO;
using System;
using System.Text;

using ProtoBuf;


namespace MiniWeChat
{
    public class IOTool
    {
		
        public static void CreateDir(string dirPath)
        {
            DirectoryInfo userDir = new DirectoryInfo(dirPath);
            if (!userDir.Exists)
            {
                userDir.Create();
            }
        }

        public static void SerializeToFile<T>(string filePath, T proto) where T : global::ProtoBuf.IExtensible
        {
            using (FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write))
            {
                Serializer.Serialize<T>(fs, proto);
            }
        }

        public static T DeserializeFromFile<T>(string filePath) where T : global::ProtoBuf.IExtensible
        {
            T item = default(T);
            using (FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Read))
            {
                try
                {
                    item = Serializer.Deserialize<T>(fs);
                }catch(Exception ex)
                {
                    Debug.Log(ex);
                }
            }
            return item;
        }

        public static string SerializeToString<T>(T proto) where T : global::ProtoBuf.IExtensible
        {
            using (MemoryStream ms = new MemoryStream())
            {
                Serializer.Serialize<T>(ms, proto);
                return Convert.ToBase64String(ms.ToArray());
            }
        }

        public static T DeserializeFromString<T>(string protoStr) where T : global::ProtoBuf.IExtensible
        {
            byte[] protoBytes = Convert.FromBase64String(protoStr);
            using (MemoryStream ms = new MemoryStream(protoBytes))
            {
                return Serializer.Deserialize<T>(ms);
            }
        }

        public static bool IsDirExist(string dirPath)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(dirPath);
            return dirInfo.Exists;
        }

        public static FileInfo[] GetFiles(string dirPath)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(dirPath);
            return dirInfo.GetFiles();
        }
    }
}

