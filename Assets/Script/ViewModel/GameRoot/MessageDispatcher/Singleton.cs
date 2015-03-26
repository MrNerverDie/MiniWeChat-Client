using System;
using UnityEngine;

namespace MiniWeChat
{
    [RequireComponent(typeof(GameRoot))]
    public class Singleton<T> : MonoBehaviour where T : Singleton<T>
    {
        public static T GetInstance()
        {
            return GameRoot.GetSingleton<T>();
        }

        public virtual void Init()
        {
            return;
        }

        public virtual void Release()
        {
            return;
        }
    }
}

