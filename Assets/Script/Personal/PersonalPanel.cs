using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace MiniWeChat
{
    public class PersonalPanel : MonoBehaviour
    {
        public Button _buttonSetName;
        public Button _buttonSetPassword;
        public Button _buttonSetHead;
        public Button _buttonExit;

        public void Start()
        {
            
        }

        public void Show(bool isShow)
        {
            gameObject.SetActive(isShow);
        }

        public void OnClickExitButton()
        {
            StateManager.GetInstance().ClearStates();
            GameObject go = UIManager.GetInstance().GetSingleUI(EUIType.LoginPanel);
            StateManager.GetInstance().PushState<LoginPanel>(go);
        }

    }
}

