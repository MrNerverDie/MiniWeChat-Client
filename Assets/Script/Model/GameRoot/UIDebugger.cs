using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;

namespace MiniWeChat
{
    public class UIDebugger : Singleton<UIDebugger>
    {
        private InputField _inputDebug;

        public override void Init()
        {
            base.Init();
            _inputDebug = UIManager.GetInstance().GetSingleUI(EUIType.InputDebug).GetComponent<InputField>();
            _inputDebug.onEndEdit.AddListener(OnInputDebugEndEdit);
            _inputDebug.gameObject.SetActive(false);
        }

        public override void Release()
        {
            base.Release();
            UIManager.GetInstance().DestroySingleUI(EUIType.InputDebug);
        }

        public void Update()
        {
#if UNITY_EDITOR
            if (Input.GetKeyUp(KeyCode.D))
            {
                _inputDebug.gameObject.SetActive(true);
                UIManager.GetInstance().SetSiblingToTop(_inputDebug.gameObject);
            }
#endif
        }

        public void OnInputDebugEndEdit(string text)
        {
            if (text == "CLEARALL")
            {
                PlayerPrefs.DeleteKey(GlobalVars.PREF_USER_ID);
                PlayerPrefs.DeleteKey(GlobalVars.PREF_USER_PASSWORD);

                DirectoryInfo dirInfo = new DirectoryInfo(GlobalUser.GetInstance().GetUserDir());
                dirInfo.Delete(true);
            }

            _inputDebug.gameObject.SetActive(false);
        }
    }
}

