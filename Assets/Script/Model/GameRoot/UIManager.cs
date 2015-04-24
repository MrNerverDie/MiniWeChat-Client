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
            InitGifRes();
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

        public void RefreshChildren(GameObject parent, EUIType name, int count)
        {
            if (parent.transform.childCount > count)
            {
                for (int i = count; i < parent.transform.childCount; i++)
                {
                    GameObject.Destroy(parent.transform.GetChild(i).gameObject); 
                }
            }
            else
            {
                for (int i = parent.transform.childCount; i < count; i++)
                {
                    AddChild(parent, name);
                }
            }
        }

        public void RefreshChildren(GameObject parent, EUIType name, int count, float childHeight, bool isVertical)
        {
            RefreshChildren(parent, name, count);

            Vector2 originalSize = parent.GetComponent<RectTransform>().sizeDelta;

            if (isVertical)
            {
                parent.GetComponent<RectTransform>().sizeDelta = new Vector2(originalSize.x, childHeight * count);
            }
            else
            {
                parent.GetComponent<RectTransform>().sizeDelta = new Vector2(childHeight * count, originalSize.y);
            }
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
                try
                {
                    Sprite sprite = go.GetComponent<SpriteRenderer>().sprite;
                    image.sprite = sprite; 
                }catch(System.NullReferenceException)
                {
                    Log4U.LogError(eAtlasName, spriteName);
                }
	        }
        }

        private void InitGifRes()
        {
            StartCoroutine(BeginInitRes());
        }

        private IEnumerator BeginInitRes()
        {
            TextAsset[] gifTextAssets = Resources.LoadAll<TextAsset>("Raw/Gif");
            foreach (var gifTextAsset in gifTextAssets)
            {
                yield return StartCoroutine(UniGif.GetTextureListCoroutine(this, gifTextAsset.bytes, gifTextAsset.GetInstanceID(), null,
                    FilterMode.Trilinear, TextureWrapMode.Clamp, false));
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
            _UIPathDict.Add(EUIType.LoadingDialog, "Common/Dialog/LoadingDialog");
            _UIPathDict.Add(EUIType.CreateGroupPanel, "Group/CreateGroupPanel");
            _UIPathDict.Add(EUIType.GroupDetailPanel, "Group/GroupDetailPanel");
            _UIPathDict.Add(EUIType.SelectGroupPanel, "Group/SelectGroupPanel");
            _UIPathDict.Add(EUIType.GroupMemberFrame, "Group/GroupMemberFrame");
            _UIPathDict.Add(EUIType.GroupMemberHeadFrame, "Group/GroupMemberHeadFrame");
            _UIPathDict.Add(EUIType.GroupMemberHeadIcon, "Group/GroupMemberHeadIcon");
            _UIPathDict.Add(EUIType.GroupFrame, "Group/GroupFrame");
            _UIPathDict.Add(EUIType.GroupChatFrame, "Chat/GroupChatFrame");
            _UIPathDict.Add(EUIType.GroupChatPanel, "Chat/GroupChatPanel");
            _UIPathDict.Add(EUIType.DoubleButtonDialog, "Common/Dialog/DoubleButtonDialog");
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
        LoadingDialog,
        CreateGroupPanel,
        GroupDetailPanel,
        GroupMemberFrame,
        GroupMemberHeadFrame,
        GroupMemberHeadIcon,
        SelectGroupPanel,
        GroupChatFrame,
        GroupChatPanel,
        GroupFrame,
        DoubleButtonDialog,
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