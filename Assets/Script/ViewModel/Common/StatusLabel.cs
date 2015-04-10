using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using DG.Tweening;

namespace MiniWeChat
{
    [RequireComponent(typeof(Text))]
    public class StatusLabel : MonoBehaviour
    {
        private const float FADE_DURATION = 0.3f;
        private Text _labelStatus;

		public void Show()
        {
            _labelStatus = GetComponent<Text>();
            _labelStatus.gameObject.SetActive(false);
            if (!NetworkManager.GetInstance().IsConncted)
            {
                _labelStatus.gameObject.SetActive(true);
                DOTween.ToAlpha(() => _labelStatus.color, x => _labelStatus.color = x, 0f, FADE_DURATION).From();
                UIManager.GetInstance().SetSiblingToTop(gameObject);
            }
        }

        public void Hide()
        {
            DOTween.ToAlpha(() => _labelStatus.color, x => _labelStatus.color = x, 0f, FADE_DURATION).OnComplete(delegate()
            {
                gameObject.SetActive(false);
            });
        }

    }
}

