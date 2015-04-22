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
            AddSingleton<MessageDispatcher>();
            AddSingleton<GlobalUser>();
            AddSingleton<GlobalContacts>();
            AddSingleton<GlobalChat>();
            AddSingleton<GlobalGroup>();
            AddSingleton<UIManager>();
            AddSingleton<StateManager>();
            AddSingleton<DialogManager>();
            AddSingleton<UIDebugger>();
            AddSingleton<NetworkManager>();
            AddSingleton<FileNetworkManager>();
        }

        private static void AddSingleton<T>() where T : Singleton<T>
        {
            if (_rootObj.GetComponent<T>() == null)
            {
                T t = _rootObj.AddComponent<T>();
                t.SetInstance(t);
                t.Init();

                _singletonReleaseList.Add(delegate()
                {
                    t.Release();
                });
            }
        }

        public static T GetSingleton<T>() where T : Singleton<T>
        {
            T t = _rootObj.GetComponent<T>();

            if (t == null)
            {
                AddSingleton<T>();
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