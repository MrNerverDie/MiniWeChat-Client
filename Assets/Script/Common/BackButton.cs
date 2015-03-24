using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace MiniWeChat
{
    public class BackButton : MonoBehaviour
    {
        private object _param;

        public void Start()
        {
            gameObject.GetComponent<Button>().onClick.AddListener(OnClickBackButton);
        }

        public void OnClickBackButton()
        {
            StateManager.GetInstance().PopState(_param);
        }

        public void SetBackParam(object param)
        {
            _param = param;
        }
    }
}

