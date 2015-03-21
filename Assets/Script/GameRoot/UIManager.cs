using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace MiniWeChat
{
    public class UIManager : Singleton<UIManager>
    {

        private Dictionary<EUIType, GameObject> _UIDict;
        private Dictionary<EUIType, string> _UIPathDict;

        private Transform _canvas;

        public override void Init()
        {
            _canvas = GameObject.Find("Canvas").transform;
            _UIDict = new Dictionary<EUIType, GameObject>();
            _UIPathDict = new Dictionary<EUIType, string>();

            InitUIPathDict();
        }

        public GameObject GetSingleUI(EUIType name)
        {
            if (_UIPathDict.ContainsKey(name) == false)
            {
                return null;
            }

            if (_UIDict.ContainsKey(name) == false)
            {
                GameObject go = GameObject.Instantiate(Resources.Load<GameObject>(_UIPathDict[name])) as GameObject;
                go.transform.SetParent(_canvas, false);
                go.name = name.ToString();
                _UIDict.Add(name, go);
                return go;
            }
            return _UIDict[name];
        }

        public void DestroySingleUI(EUIType name)
        {
            if (_UIDict.ContainsKey(name) == false)
            {
                return;
            }

            GameObject.Destroy(_UIDict[name]);
            _UIDict.Remove(name);
        }

        public GameObject AddChild(GameObject parent, EUIType name)
        {
            GameObject go = GameObject.Instantiate(Resources.Load<GameObject>(_UIPathDict[name])) as GameObject;
            go.transform.SetParent(parent.transform, false);
            return go;
        }

        private void InitUIPathDict()
        {
            _UIPathDict.Add(EUIType.MainMenuPanel, "Common/MainMenuPanel");
            _UIPathDict.Add(EUIType.ChatFrame, "Chat/ChatFrame");
            _UIPathDict.Add(EUIType.ContactFrame, "Contacts/ContactFrame");
            _UIPathDict.Add(EUIType.FriendChatBubbleFrame, "Chat/FriendChatBubbleFrame");
            _UIPathDict.Add(EUIType.PersonalChatBubbleFrame, "Chat/PersonalChatBubbleFrame");
            _UIPathDict.Add(EUIType.ChatPanel, "Chat/ChatPanel");
            _UIPathDict.Add(EUIType.WelcomePanel, "Common/WelcomePanel");
        }
    }

    public enum EUIType
    {
        MainMenuPanel = 1,
        ChatFrame,
        ContactFrame,
        FriendChatBubbleFrame,
        PersonalChatBubbleFrame,
        ChatPanel,
        WelcomePanel,
    }
}