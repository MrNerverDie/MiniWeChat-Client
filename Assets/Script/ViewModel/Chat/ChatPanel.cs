using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using protocol;

namespace MiniWeChat
{
    public class ChatPanel : BaseState
    {
        [SerializeField]
        public InputField _inputChat;
        [SerializeField]
        public ScrollRect _scrollChatLog;
        [SerializeField]
        public Button _buttonSend;
        [SerializeField]
        public Button _buttonFriendDetail;
        [SerializeField]
        public VerticalLayoutGroup _gridChatBubble;
        [SerializeField]
        public Text _labelGuestUserName;
        [SerializeField]
        public Toggle _toggleShowEmotion;
        [SerializeField]
        public GridLayoutGroup _gridEmotion;

        private UserItem _guestUserItem;
        protected ChatLog _chatLog;

        protected List<ChatBubbleFrame> _chatBubbleList = new List<ChatBubbleFrame>();

        public override void OnEnter(object param)
        {

            Init(param);

            _guestUserItem = GlobalContacts.GetInstance().GetUserItemById(_chatLog.chatID);
            if (_guestUserItem != null)
            {
                if (_labelGuestUserName)
                {
                    _labelGuestUserName.text = _guestUserItem.userName;                    
                }
            }


        }

        public override void OnShow(object param = null)
        {
            base.OnShow(param);

            if (param != null)
            {
                _chatLog = param as ChatLog;
            }

            RefreshChatLog();

            MessageDispatcher.GetInstance().RegisterMessageHandler((uint)EUIMessage.UPDATE_RECEIVE_CHAT, OnUpdateReceiveChat);
            MessageDispatcher.GetInstance().RegisterMessageHandler((uint)EUIMessage.UPDATE_SEND_CHAT, OnUpdateSendChat);
            MessageDispatcher.GetInstance().RegisterMessageHandler((uint)EUIMessage.DELETE_CHAT_ITEM, OnDeleteChatItem);
        }

        public override void OnHide()
        {
            base.OnHide();

            MessageDispatcher.GetInstance().UnRegisterMessageHandler((uint)EUIMessage.UPDATE_RECEIVE_CHAT, OnUpdateReceiveChat);
            MessageDispatcher.GetInstance().UnRegisterMessageHandler((uint)EUIMessage.UPDATE_SEND_CHAT, OnUpdateSendChat);
            MessageDispatcher.GetInstance().UnRegisterMessageHandler((uint)EUIMessage.DELETE_CHAT_ITEM, OnDeleteChatItem);
        }

        private void RefreshChatLog()
        {

            for (int i = _chatBubbleList.Count; i < _chatLog.itemList.Count; i++)
            {
                AddBubbleFrame(_chatLog.itemList[i]);
            }

            UpdateChatBubbleGrid();

            GlobalChat.GetInstance().MarkForRead(_chatLog.chatID);
        }

#region EventListener

        public virtual void OnClickSendButton()
        {
            if (_inputChat.text == "")
            {
                return;
            }

            ChatDataItem chatDataItem = new ChatDataItem
            {
                sendUserId = GlobalUser.GetInstance().UserId,
                receiveUserId = _guestUserItem.userId,
                date = System.DateTime.Now.Ticks,
                chatType = ChatDataItem.ChatType.TEXT,
                chatBody = _inputChat.text,
            };
            GlobalChat.GetInstance().SendChatReq(chatDataItem);

            AddBubbleFrame(chatDataItem);

            UpdateChatBubbleGrid();

            _inputChat.text = "";
        }

        public virtual void OnClickFriendDetailButton()
        {
            StateManager.GetInstance().PushState<FriendDetailPanel>(EUIType.FriendDetailPanel, _guestUserItem);
        }

        public void OnClickShowEmotionButton(bool check)
        {
            _gridEmotion.gameObject.SetActive(check);
            StartCoroutine(CoroutineTool.ActionNextFrame(delegate()
            {
                UpdateChatBubbleGrid();
            }));
        }

        public virtual void OnClickSendEmotionButton(int index)
        {
            ChatDataItem chatDataItem = new ChatDataItem
            {
                sendUserId = GlobalUser.GetInstance().UserId,
                receiveUserId = _guestUserItem.userId,
                date = System.DateTime.Now.Ticks,
                chatType = ChatDataItem.ChatType.IMAGE,
                chatBody = index.ToString(),
                targetType = ChatDataItem.TargetType.INDIVIDUAL
            };
            GlobalChat.GetInstance().SendChatReq(chatDataItem);

            AddBubbleFrame(chatDataItem);

            UpdateChatBubbleGrid();
        }

#endregion


        #region Messagehandler

        public void OnUpdateReceiveChat(uint iMessageType, object kParam)
        {
            RefreshChatLog();
        }

        public void OnUpdateSendChat(uint iMessageType, object kParam)
        {
            int index = (int)kParam;
            _chatBubbleList[index].Show(_chatLog.itemList[index]);
        }

        public void OnDeleteChatItem(uint iMessageType, object kParam)
        {
            DeleteChatParam param = kParam as DeleteChatParam;
            Log4U.LogDebug( _chatLog.itemList.Remove(param.chatDataItem));
            Log4U.LogDebug(_chatBubbleList.Remove(param.chatBubbleFrame));
        }

        #endregion

        protected void AddBubbleFrame(ChatDataItem chatDataItem)
        {
            EUIType uiType = (chatDataItem.sendUserId == GlobalUser.GetInstance().UserId) ? EUIType.PersonalChatBubbleFrame : EUIType.FriendChatBubbleFrame;
            GameObject bubbleFrame = UIManager.GetInstance().AddChild(_gridChatBubble.gameObject, uiType);
            bubbleFrame.GetComponent<ChatBubbleFrame>().Show(chatDataItem);
            _chatBubbleList.Add(bubbleFrame.GetComponent<ChatBubbleFrame>()); 
        }

        protected void UpdateChatBubbleGrid()
        {
            float sumHeight = 0;
            foreach (var item in _chatBubbleList)
            {
                sumHeight += item.GetHeight();
            }

            _gridChatBubble.GetComponent<RectTransform>().sizeDelta = new Vector2(GlobalVars.DEFAULT_SCREEN_WIDTH, sumHeight);
            _scrollChatLog.verticalNormalizedPosition = 0;
        }

        protected void Init(object param)
        {
            base.OnEnter(param);

            UIManager.GetInstance().AddChild(transform.Find("TopBar").gameObject, EUIType.BackButton);

            _buttonSend.onClick.AddListener(OnClickSendButton);
            _toggleShowEmotion.onValueChanged.AddListener(OnClickShowEmotionButton);
            _buttonFriendDetail.onClick.AddListener(OnClickFriendDetailButton);

        }
    }
}

