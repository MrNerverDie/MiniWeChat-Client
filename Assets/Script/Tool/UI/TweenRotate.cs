using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using DG.Tweening;

namespace MiniWeChat
{
    public class TweenRotate : MonoBehaviour
    {
        public void Start()
        {
            transform.DORotate(new Vector3(0, 0, 360), 1.0f, RotateMode.FastBeyond360)
                .SetLoops(-1, LoopType.Restart)
                .SetEase(Ease.Linear)
                .From();
        }
    }
}

