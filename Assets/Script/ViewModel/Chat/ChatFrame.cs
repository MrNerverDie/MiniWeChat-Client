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
        public MiniButton _buttonChatFrame;
        [SerializeField]
        public Image _imageUnRead;
        [SerializeField]
        public Text _labelUnReadNum;

        protected ChatLog _chatLog;

        // Use this for initialization
        public virtual void Start()
        {
            _buttonChatFrame.onClick.AddListener(OnClickChatFrameButton);
            _buttonChatFrame.onLongPress.AddListener(OnLongPressChatFrameButton);
        }

        public virtual void Show(ChatLog chatLog)
        {
            _chatLog = chatLog;

            // Set UserItem //

            UserItem userItem = GlobalContacts.GetInstance().GetUserItemById(chatLog.chatID);
            if (userItem != null && chatLog.targetType == ChatDataItem.TargetType.INDIVIDUAL)
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
                _labelLastChat.text = "";
                ChatDataItem lastChat = GlobalChat.GetInstance().GetLastChat(chatLog.chatID);
                if (lastChat != null)
                {
                    if (lastChat.chatType == ChatDataItem.ChatType.TEXT)
                    {
                        _labelLastChat.text = lastChat.chatBody;
                    }
                    else if (lastChat.chatType == ChatDataItem.ChatType.IMAGE)
                    {
                        _labelLastChat.text = "[表情]";
                    }
                }                
            }

            if (_labelDate)
            {
                _labelDate.text = DateFormatTool.GetChatFrameTimeStr(_chatLog.date);          
            }

            if (_imageUnRead)
            {
                int num = GlobalChat.GetInstance().GetUnReadNum(_chatLog.chatID);
                num = (num > 99) ? 99 : num;
                _imageUnRead.gameObject.SetActive(num != 0);
                _labelUnReadNum.text = num.ToString();
            }
        }

        public virtual void OnClickChatFrameButton()
        {
            StateManager.GetInstance().PushState<ChatPanel>(EUIType.ChatPanel, _chatLog);
        }

        public void OnLongPressChatFrameButton()
        {
            DialogManager.GetInstance().CreateDoubleButtonDialog("您确定要删除该聊天记录吗？", "警告", OnConfirmDeleteChatLog);
        }

        public void OnConfirmDeleteChatLog()
        {
            GlobalChat.GetInstance().RemoveChatLog(_chatLog.chatID);
            Destroy(gameObject);
        }

        public ChatDataItem.TargetType GetTargetType()
        {
            return _chatLog.targetType;
        }

    }
}

