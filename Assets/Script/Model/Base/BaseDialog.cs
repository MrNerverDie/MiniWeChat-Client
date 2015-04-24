using UnityEngine;
using UnityEngine.UI;

using System.Collections;
using System.Collections.Generic;

using DG.Tweening;


namespace MiniWeChat
{
    public class BaseDialog : MonoBehaviour
    {
        private const float FADE_DURATION = 0.3f;

        private CanvasGroup _canvasGroup;

        public void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        public Tweener BeginEnterTween()
        {
            Tweener tweener = DOTween.To(() => _canvasGroup.alpha, x => _canvasGroup.alpha = x, 1f, FADE_DURATION).ChangeStartValue(0f);
            StartCoroutine(BlockTouchForTween(tweener));
            return tweener;
        }

        public Tweener BeginExitTween()
        {
            Tweener tweener = DOTween.To(() => _canvasGroup.alpha, x => _canvasGroup.alpha = x, 0f, FADE_DURATION).ChangeStartValue(1f);
            StartCoroutine(BlockTouchForTween(tweener));
            return tweener;
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

        protected virtual void Hide()
        {
            BeginExitTween().OnComplete(delegate()
            {
                gameObject.SetActive(false);
            });
        }
    }
}

