using UnityEngine;
using System;
using UnityEngine.UI;
using System.Collections;
using protocol;

namespace MiniWeChat
{
    public class ChatFrame : MonoBehaviour
    {
        [SerializeField]
        public Image _imageHead;
        [SerializeField]
        public Text _labelUserName;
        [SerializeField]
        public Text _labelLastChat;
        [SerializeField]
        public Text _labelDate;
        [SerializeField]
        public Button _buttonChatFrame;

        protected ChatLog _chatLog;

        // Use this for initialization
        public virtual void Start()
        {
            _buttonChatFrame.onClick.AddListener(OnClickChatFrameButton);
        }

        public virtual void Show(ChatLog chatLog)
        {
            _chatLog = chatLog;

            // Set UserItem //

            UserItem userItem = GlobalContacts.GetInstance().GetUserItemById(chatLog.chatID);
            if (userItem != null)
            {
                if (_imageHead)
                {
                    UIManager.GetInstance().SetImage(_imageHead, EAtlasName.Head, "00" + userItem.headIndex);                    
                }

                if (_labelUserName)
                {
                    _labelUserName.text = userItem.userName;                    
                }
            }


            // Set ChatItem //
            if (_labelLastChat)
            {
                _labelLastChat.text = GlobalChat.GetInstance().GetLastChat(chatLog.chatID).chatBody;                
            }

            if (_labelDate)
            {
                _labelDate.text = new DateTime(chatLog.date).ToString("yyyy/MM/dd HH:mm");                
            }
        }

        public virtual void OnClickChatFrameButton()
        {
            StateManager.GetInstance().PushState<ChatPanel>(EUIType.ChatPanel, _chatLog);
        }

    }
}

