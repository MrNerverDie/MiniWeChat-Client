using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using DG.Tweening;

namespace MiniWeChat
{
    [RequireComponent(typeof(CanvasGroup))]
    public class BaseState : MonoBehaviour
    {

        private CanvasGroup _canvasGroup;
        private EUIType _uiType;

        private const float FADE_DURATION = 0.3f;
        private const float ORIGINAL_SCALE = 0.8f;

        /// <summary>
        /// 界面栈初始化的时候被调用的方法
        /// </summary>
        /// <param name="param">需要传进来的参数</param>
        public virtual void OnEnter(object param = null)
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            OnShow(param);

        }

        /// <summary>
        /// 界面栈销毁的时候被调用的方法
        /// </summary>
        public virtual void OnExit()
        {
            OnHide();
            DestroySelf();
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

        public Tweener BeginEnterTween()
        {
            Tweener tweener = transform.DOScale(Vector3.one * ORIGINAL_SCALE, FADE_DURATION).From();
            StartCoroutine(BlockTouchForTween(tweener));
            return DOTween.To(() => _canvasGroup.alpha, x => _canvasGroup.alpha = x, 0f, FADE_DURATION).From();
        }

        public Tweener BeginExitTween()
        {
            Tweener tweener = transform.DOScale(Vector3.one * ORIGINAL_SCALE, FADE_DURATION);
            StartCoroutine(BlockTouchForTween(tweener));
            return DOTween.To(() => _canvasGroup.alpha, x => _canvasGroup.alpha = x, 0f, FADE_DURATION);
        }

        private IEnumerator BlockTouchForTween(Tweener tweener)
        {
            DisableTouch();
            yield return tweener.WaitForCompletion();
            EnabelTouch();
        }

        public void DisableTouch()
        {
            _canvasGroup.blocksRaycasts = false;
        }

        public void EnabelTouch()
        {
            _canvasGroup.blocksRaycasts = true;
        }

        public void SetUIType(EUIType uiType)
        {
            _uiType = uiType;
        }

        public void DestroySelf()
        {
            UIManager.GetInstance().DestroySingleUI(_uiType);
        }

    }
}

