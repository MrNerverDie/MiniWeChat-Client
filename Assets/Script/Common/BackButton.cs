using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace MiniWeChat
{
    public class BackButton : MonoBehaviour
    {
        public void Start()
        {
            gameObject.GetComponent<Button>().onClick.AddListener(OnClickBackButton);
        }

        public void OnClickBackButton()
        {
            StateManager.GetInstance().PopState();
        }
    }
}

