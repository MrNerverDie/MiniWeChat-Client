using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace MiniWeChat
{
    public class BaseState : MonoBehaviour
    {
        /// <summary>
        /// 界面栈初始化的时候被调用的方法
        /// </summary>
        /// <param name="param">需要传进来的参数</param>
        public virtual void OnEnter(object param = null)
        {
            OnShow();
        }

        /// <summary>
        /// 界面栈销毁的时候被调用的方法
        /// </summary>
        public virtual void OnExit()
        {
            OnHide();
        }

        /// <summary>
        /// 界面栈隐藏时调用的方法，当界面销毁的时候也会被调用
        /// </summary>
        public virtual void OnHide()
        {
            gameObject.SetActive(false);
        }

        /// <summary>
        /// 界面栈显示时调用的方法，当界面初始化的时候也会被调用
        /// </summary>
        /// <param name="param"></param>
        public virtual void OnShow(object param = null)
        {
            gameObject.SetActive(true);
        }
    }
}

