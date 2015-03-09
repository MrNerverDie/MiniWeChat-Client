using UnityEngine;
using System;
using System.Collections;

namespace MiniWeChat
{
    public class GameRoot : MonoBehaviour
    {
        private static GameRoot _instance;
        GameObject _rootObj;

        public void Awake()
        {
            _rootObj = gameObject;
            GameObject.DontDestroyOnLoad(_rootObj);

            AddSingleton<StateManager>(_rootObj);
            AddSingleton<MessageDispatcher>(_rootObj);
            AddSingleton<UIManager>(_rootObj);
        }

        private static T AddSingleton<T>(GameObject go) where T : Singleton<T>
        {
            T t = go.AddComponent<T>();
            t.Init();
            return t;
        }

        public static T GetSingleton<T>() where T : Singleton<T>
        {
            T t = _instance._rootObj.GetComponent<T>();

            if (t == null)
            {
                throw new Exception("Failed To Get Singleton : " + typeof(T));
            }

            return t;
        }
    }
}