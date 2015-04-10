using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace MiniWeChat
{
    [RequireComponent(typeof(ScrollRect))]
    public class Swipe : MonoBehaviour
    {
		public void Start()
        {
            GetComponent<ScrollRect>().onValueChanged.AddListener(OnDragScrollRect);
        }

        public void OnDragScrollRect(Vector2 dragVec)
        {
            Debug.Log(dragVec);
        }
    }
}

