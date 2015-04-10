using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace MiniWeChat
{
    public class GameRoot : MonoBehaviour
    {
        private static GameObject _rootObj;

        private static List<Action> _singletonReleaseList;

        public void Awake()
        {
            _rootObj = gameObject;
            GameObject.DontDestroyOnLoad(_rootObj);

            _singletonReleaseList = new List<Action>();

            StartCoroutine(InitSingletons());
        }

        /// <summary>
        /// 在这里进行所有单例的销毁
        /// </summary>
        public void OnApplicationQuit()
        {
            for (int i = _singletonReleaseList.Count - 1; i >= 0; i--)
            {
                _singletonReleaseList[i]();
            }
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
            AddSingleton<GlobalUser>(_rootObj);
            AddSingleton<GlobalContacts>(_rootObj);
            AddSingleton<GlobalChat>(_rootObj);
            AddSingleton<UIManager>(_rootObj);
            AddSingleton<StateManager>(_rootObj);
            AddSingleton<DialogManager>(_rootObj);
            AddSingleton<UIDebugger>(_rootObj);
            AddSingleton<NetworkManager>(_rootObj);
            AddSingleton<FileNetworkManager>(_rootObj);
        }

        private static T AddSingleton<T>(GameObject go) where T : Singleton<T>
        {
            T t = go.AddComponent<T>();
            t.SetInstance(t);
            t.Init();

            _singletonReleaseList.Add(delegate()
            {
                t.Release();
            });

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