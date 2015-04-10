using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections.Generic;

namespace MiniWeChat
{
    public class Log4U : MonoBehaviour
    {

        public enum LogLevel
        {
            DEBUG = 0, 
            INFO,
            WARNING,
            ERROR,
            NOLOG,
        }

        private static LogLevel _currentLevel = LogLevel.NOLOG;

        public static void LogDebug(object msg)
        {
            if (_currentLevel <= LogLevel.DEBUG)
            {
                Debug.Log(GetLogStr(LogLevel.DEBUG, msg));
            }
        }

        public static void LogInfo(object msg)
        {
            if (_currentLevel <= LogLevel.INFO)
            {
                Debug.Log(GetLogStr(LogLevel.INFO, msg));
            }
        }

        public static void LogWarning(object msg)
        {
            if (_currentLevel <= LogLevel.WARNING)
            {
                Debug.LogWarning(GetLogStr(LogLevel.WARNING, msg));
            }
        }

        public static void LogError(object msg)
        {
            if (_currentLevel <= LogLevel.ERROR)
            {
                Debug.LogError(GetLogStr(LogLevel.ERROR, msg));
            }
        }

        public static string GetLogStr(LogLevel level, object msg)
        {
            return String.Format("[{0}][{1}]{2}", DateTime.Now.ToShortTimeString(), level, msg); 
        }
    }
}

