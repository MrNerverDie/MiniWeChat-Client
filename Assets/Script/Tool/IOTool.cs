using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;
using ProtoBuf;
using System.Text;

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

        public static T DeserializeFromFile<T>(string filePath, T proto) where T : global::ProtoBuf.IExtensible
        {
            using (FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Read))
            {
                return Serializer.Deserialize<T>(fs);
            }
        }
    }
}

