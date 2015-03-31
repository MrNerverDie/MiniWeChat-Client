using UnityEngine;
using System;
using UnityEngine.UI;
using System.Collections;
using protocol;

namespace MiniWeChat
{
    public class ChatFrame : MonoBehaviour
    {
        public Image _imageHead;
        public Text _labelUserName;
        public Text _labelLastChat;
        public Text _labelDate;

        public Button _buttonChatFrame;

        // Use this for initialization
        void Start()
        {
            _buttonChatFrame.onClick.AddListener(OnClickChatFrameButton);
        }

        public void Show(ChatLog chatLog)
        {
            UserItem userItem = GlobalContacts.GetInstance().GetUserItemById(chatLog.userId);
            UIManager.GetInstance().SetImage(_imageHead, EAtlasName.Head, "00" + userItem.headIndex);
            _labelLastChat.text = GlobalChat.GetInstance().GetLastChat(chatLog.userId).chatBody;
            _labelUserName.text = userItem.userName;
            _labelDate.text = new DateTime(chatLog.date).ToString();
        }

        public void OnClickChatFrameButton()
        {
            GameObject chatPanel = UIManager.GetInstance().GetSingleUI(EUIType.ChatPanel);
            StateManager.GetInstance().PushState<ChatPanel>(chatPanel);
        }

    }
}

