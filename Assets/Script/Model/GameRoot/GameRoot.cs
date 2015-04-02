using UnityEngine;
using System;
using System.Collections;

namespace MiniWeChat
{
    public class GameRoot : MonoBehaviour
    {
        private static GameObject _rootObj;

        public void Awake()
        {
            _rootObj = gameObject;
            GameObject.DontDestroyOnLoad(_rootObj);

            StartCoroutine(InitSingletons());
        }

        /// <summary>
        /// 在这里进行所有单例的销毁
        /// </summary>
        public void OnDestroy()
        {
            MessageDispatcher.GetInstance().Release();
            UIManager.GetInstance().Release();
            StateManager.GetInstance().Release();
            DialogManager.GetInstance().Release();
            NetworkManager.GetInstance().Release();
            GlobalContacts.GetInstance().Release();
            GlobalChat.GetInstance().Release();
            GlobalUser.GetInstance().Release();
            UIDebugger.GetInstance().Release();
        }

        /// <summary>
        /// 在这里进行所有丹利的初始化
        /// </summary>
        /// <returns></returns>
        private IEnumerator InitSingletons()
        {
            ClearCanvas();
            yield return null;
            AddSingleton<MessageDispatcher>(_rootObj);
            AddSingleton<UIManager>(_rootObj);
            AddSingleton<StateManager>(_rootObj);
            AddSingleton<NetworkManager>(_rootObj);
            AddSingleton<DialogManager>(_rootObj);
            AddSingleton<GlobalUser>(_rootObj);
            AddSingleton<GlobalContacts>(_rootObj);
            AddSingleton<GlobalChat>(_rootObj);
            AddSingleton<UIDebugger>(_rootObj);
        }

        private static T AddSingleton<T>(GameObject go) where T : Singleton<T>
        {
            T t = go.AddComponent<T>();
            t.Init();
            return t;
        }

        public static T GetSingleton<T>() where T : Singleton<T>
        {
            T t = _rootObj.GetComponent<T>();

            if (t == null)
            {
                throw new Exception("Failed To Get Singleton : " + typeof(T));
            }

            return t;
        }

        public void ClearCanvas()
        {
            Transform canvas = GameObject.Find("Canvas").transform;
            foreach (Transform panel in canvas)
            {
                GameObject.Destroy(panel.gameObject);
            }
        }
    }
}