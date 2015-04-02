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
        private Dictionary<EAtlasName, string> _atlasPathDict;

        private Transform _canvas;

        public override void Init()
        {
            _canvas = GameObject.Find("Canvas").transform;
            _UIDict = new Dictionary<EUIType, GameObject>();
            _UIPathDict = new Dictionary<EUIType, string>();
            _atlasPathDict = new Dictionary<EAtlasName, string>();

            InitUIPathDict();
            InitAtlasPathDict();
        }

        public GameObject GetSingleUI(EUIType name)
        {
            if (_UIPathDict.ContainsKey(name) == false)
            {
                return null;
            }

            if (_UIDict.ContainsKey(name) == false)
            {
                GameObject go = GameObject.Instantiate(Resources.Load<GameObject>("View/" + _UIPathDict[name])) as GameObject;
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
            GameObject go = GameObject.Instantiate(Resources.Load<GameObject>("View/" + _UIPathDict[name])) as GameObject;
            go.transform.SetParent(parent.transform, false);
            return go;
        }

        public void SetSiblingToTop(GameObject child)
        {
            child.transform.SetSiblingIndex(child.transform.parent.childCount - 1);
        }

        public void SetImage(Image image, EAtlasName eAtlasName, string spriteName)
        {
            GameObject go = Resources.Load<GameObject>("Raw/Image/" + _atlasPathDict[eAtlasName] + "/" + spriteName);
            if (go != null)
	        {
                Sprite sprite = go.GetComponent<SpriteRenderer>().sprite;
                image.sprite = sprite; 
	        }
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
            _UIPathDict.Add(EUIType.BackButton, "Common/BackButton");
            _UIPathDict.Add(EUIType.RegisterPanel, "Personal/RegisterPanel");
            _UIPathDict.Add(EUIType.LoginPanel, "Personal/LoginPanel");
            _UIPathDict.Add(EUIType.SingleButtonDialog, "Common/Dialog/SingleButtonDialog");
            _UIPathDict.Add(EUIType.DoubleButtonInputDialog, "Common/Dialog/DoubleButtonInputDialog");
            _UIPathDict.Add(EUIType.ImageListPanel, "Common/ImageListPanel");
            _UIPathDict.Add(EUIType.SearchFriendPanel, "Contacts/SearchFriendPanel");
            _UIPathDict.Add(EUIType.FriendDetailPanel, "Contacts/FriendDetailPanel");
            _UIPathDict.Add(EUIType.InputDebug, "Common/InputDebug");
            _UIPathDict.Add(EUIType.StatusLabel, "Common/StatusLabel");
        }

        private void InitAtlasPathDict()
        {
            _atlasPathDict.Add(EAtlasName.Chat, "Chat");
            _atlasPathDict.Add(EAtlasName.Common, "Common");
            _atlasPathDict.Add(EAtlasName.Head, "Head");
            _atlasPathDict.Add(EAtlasName.MainMenu, "MainMenu");
            _atlasPathDict.Add(EAtlasName.Single, "Single");
        }
    }

    public enum EUIType
    {
        MainMenuPanel = 0,
        ChatFrame,
        ContactFrame,
        FriendChatBubbleFrame,
        PersonalChatBubbleFrame,
        ChatPanel,
        WelcomePanel,
        BackButton,
        RegisterPanel,
        LoginPanel,
        SingleButtonDialog,
        DoubleButtonInputDialog,
        ImageListPanel,
        SearchFriendPanel,
        FriendDetailPanel,
        InputDebug,
        StatusLabel,
    }

    public enum EAtlasName
    {
        Chat = 0,
        Common,
        Head,
        MainMenu,
        Single,
    }



}