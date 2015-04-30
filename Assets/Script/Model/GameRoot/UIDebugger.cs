using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;

namespace MiniWeChat
{
    public class UIDebugger : Singleton<UIDebugger>
    {
        private InputField _inputDebug;

        private Dictionary<string, System.Action<string[]>> _debugActionDict;

        public override void Init()
        {
            base.Init();
            _inputDebug = UIManager.GetInstance().GetSingleUI(EUIType.InputDebug).GetComponent<InputField>();
            _inputDebug.onEndEdit.AddListener(OnInputDebugEndEdit);
            _inputDebug.gameObject.SetActive(false);

            _debugActionDict = new Dictionary<string, System.Action<string[]>>
            {
                {"CLEARALL", OnClearAll},
                {"UPLOADFILE", OnUploadFile},
                {"CHANGEIP", OnChangeIP},
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

#if UNITY_ANDROID
            if (Input.touchCount == 4)
            {
                _inputDebug.gameObject.SetActive(true);
                UIManager.GetInstance().SetSiblingToTop(_inputDebug.gameObject);
            }
#endif
        }

        public void OnInputDebugEndEdit(string text)
        {
            string[] cmd = text.Split(' ');
            if (_debugActionDict.ContainsKey(cmd[0]))
            {
                _debugActionDict[cmd[0]](cmd);
            }

            _inputDebug.gameObject.SetActive(false);
        }

        public void OnClearAll(string[] args)
        {
            PlayerPrefs.DeleteKey(GlobalVars.PREF_USER_ID);
            PlayerPrefs.DeleteKey(GlobalVars.PREF_USER_PASSWORD);

            GlobalChat.GetInstance().ClearLogDict();
            GlobalContacts.GetInstance().ClearFriendDict();
            GlobalGroup.GetInstance().ClearGroupDict();

            DirectoryInfo dirInfo = new DirectoryInfo(Application.persistentDataPath);
            foreach (var item in dirInfo.GetDirectories())
            {
                item.Delete(true);
            }
        }

        public void OnUploadFile(string[] args)
        {
            GameObject go = Resources.Load<GameObject>("Raw/Image/Head/001");
            if (go != null)
            {
                Sprite sprite = go.GetComponent<SpriteRenderer>().sprite;
                FileNetworkManager.GetInstance().UploadFile("https://www.baidu.com/", UnityEngine.Random.value.ToString(), sprite);
            }
        }

        public void OnChangeIP(string[] args)
        {
            if (args.Length >= 2)
            {
                GlobalVars.IPAddress = args[1];
                MessageDispatcher.GetInstance().DispatchMessageAsync((uint)EModelMessage.SOCKET_DISCONNECTED);
            }
        }
    }
}

