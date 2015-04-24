using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

using System.Collections.Generic;

namespace MiniWeChat
{
    [RequireComponent(typeof(Button))]
    public class MiniButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        [Tooltip("长按判定时间")]
        public float _durationThreshold = 0.75f;

        public UnityEvent onLongPress = new UnityEvent();
        public Button.ButtonClickedEvent onClick;
 
        private bool isPointerDown = false;
        private bool longPressTriggered = false;
        private float timePressStarted;
 
        public void Awake()
        {
            onClick = GetComponent<Button>().onClick;
        }

        private void Update( ) {
            if ( isPointerDown && !longPressTriggered ) {
                if ( Time.time - timePressStarted > _durationThreshold ) {
                    longPressTriggered = true;
                    onLongPress.Invoke();
                }
            }
        }
 
        public void OnPointerDown( PointerEventData eventData ) {
            timePressStarted = Time.time;
            isPointerDown = true;
            longPressTriggered = false;
        }
 
        public void OnPointerUp( PointerEventData eventData ) {
            isPointerDown = false;
        }
 
 
        public void OnPointerExit( PointerEventData eventData ) {
            isPointerDown = false;
        }
    }
}

