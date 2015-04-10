using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;

namespace MiniWeChat
{
    public class UIDebugger : Singleton<UIDebugger>
    {
        private InputField _inputDebug;

        private Dictionary<string, System.Action> _debugActionDict;

        public override void Init()
        {
            base.Init();
            _inputDebug = UIManager.GetInstance().GetSingleUI(EUIType.InputDebug).GetComponent<InputField>();
            _inputDebug.onEndEdit.AddListener(OnInputDebugEndEdit);
            _inputDebug.gameObject.SetActive(false);

            _debugActionDict = new Dictionary<string, System.Action>
            {
                {"CLEARALL", OnClearAll},
                {"UPLOADFILE", OnUploadFile},
            };
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
            if (_debugActionDict.ContainsKey(text))
            {
                _debugActionDict[text]();
            }

            _inputDebug.gameObject.SetActive(false);
        }

        public void OnClearAll()
        {
            PlayerPrefs.DeleteKey(GlobalVars.PREF_USER_ID);
            PlayerPrefs.DeleteKey(GlobalVars.PREF_USER_PASSWORD);

            GlobalChat.GetInstance().ClearLogDict();
            GlobalContacts.GetInstance().ClearFriendDict();

            DirectoryInfo dirInfo = new DirectoryInfo(Application.persistentDataPath);
            foreach (var item in dirInfo.GetDirectories())
            {
                item.Delete(true);
            }
        }

        public void OnUploadFile()
        {
            GameObject go = Resources.Load<GameObject>("Raw/Image/Head/001");
            if (go != null)
            {
                Sprite sprite = go.GetComponent<SpriteRenderer>().sprite;
                FileNetworkManager.GetInstance().UploadFile("https://www.baidu.com/", UnityEngine.Random.value.ToString(), sprite);
            }
        }
    }
}

