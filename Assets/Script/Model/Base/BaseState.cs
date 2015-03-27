using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using DG.Tweening;

namespace MiniWeChat
{
    [RequireComponent(typeof(CanvasGroup))]
    public class BaseState : MonoBehaviour
    {

        private CanvasGroup _canvasGroup;

        /// <summary>
        /// 界面栈初始化的时候被调用的方法
        /// </summary>
        /// <param name="param">需要传进来的参数</param>
        public virtual void OnEnter(object param = null)
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            OnShow();
            transform.localScale = Vector3.one * 0.8f;
            transform.DOScale(Vector3.one, 0.3f);
            _canvasGroup.alpha = 0f;
            DOTween.To(() => _canvasGroup.alpha, x => _canvasGroup.alpha = x, 1.0f, 0.3f);
        }

        /// <summary>
        /// 界面栈销毁的时候被调用的方法
        /// </summary>
        public virtual void OnExit()
        {
            OnHide();
            transform.localScale = Vector3.one;
            transform.DOScale(Vector3.zero, 0.3f);
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

