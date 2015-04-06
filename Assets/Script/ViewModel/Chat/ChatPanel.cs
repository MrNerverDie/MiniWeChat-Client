using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using protocol;

namespace MiniWeChat
{
    public class ChatPanel : BaseState
    {
        public InputField _inputChat;
        public ScrollRect _scrollChatLog;
        public Button _buttonSend;
        public Button _buttonFriendDetail;
        public VerticalLayoutGroup _gridChatBubble;
        public Text _labelGuestUserName;

        private UserItem _guestUserItem;
        private ChatLog _chatLog;

        private List<ChatBubbleFrame> _chatBubbleList;

        public override void OnEnter(object param = null)
        {
            base.OnEnter(param);
            _guestUserItem = param as UserItem;            
            UIManager.GetInstance().AddChild(gameObject, EUIType.BackButton);
            _gridChatBubble.GetComponent<RectTransform>().sizeDelta = new Vector2(GlobalVars.DEFAULT_SCREEN_WIDTH, 0);
            _buttonSend.onClick.AddListener(OnClickSendButton);
            _buttonFriendDetail.onClick.AddListener(OnClickFriendDetailButton);
            _scrollChatLog.verticalNormalizedPosition = 0;
            _labelGuestUserName.text = _guestUserItem.userName;

            _chatLog = GlobalChat.GetInstance().GetChatLog(_guestUserItem.userId);
            _chatBubbleList = new List<ChatBubbleFrame>();
            RefreshChatLog();

        }

        public override void OnExit()
        {
            base.OnExit();
            UIManager.GetInstance().DestroySingleUI(EUIType.ChatPanel);
        }

        public override void OnShow(object param = null)
        {
            base.OnShow(param);
        }

        public override void OnHide()
        {
            base.OnHide();
        }

        private void RefreshChatLog()
        {

            foreach (var chatDataItem in _chatLog.itemList)
            {
                EUIType uiType = (chatDataItem.sendUserId == GlobalUser.GetInstance().UserId) ? EUIType.PersonalChatBubbleFrame : EUIType.FriendChatBubbleFrame;
                GameObject bubbleFrame = UIManager.GetInstance().AddChild(_gridChatBubble.gameObject, uiType);
                bubbleFrame.GetComponent<ChatBubbleFrame>().Show(chatDataItem);
                _chatBubbleList.Add(bubbleFrame.GetComponent<ChatBubbleFrame>());
            }

            _gridChatBubble.GetComponent<RectTransform>().sizeDelta = new Vector2(GlobalVars.DEFAULT_SCREEN_WIDTH, ChatBubbleFrame.FRAME_BUBBLE_HEIGHT_BASE * _chatLog.itemList.Count);

            _scrollChatLog.verticalNormalizedPosition = 0;
        }

        public void OnClickSendButton()
        {
            ChatDataItem chatDataItem = new ChatDataItem
            {
                sendUserId = GlobalUser.GetInstance().UserId,
                receiveUserId = _guestUserItem.userId,
                date = System.DateTime.Now.Ticks,
                chatType = ChatDataItem.ChatType.TEXT,
                chatBody = _inputChat.text,
            };
            GlobalChat.GetInstance().SendChatReq(chatDataItem);

            GameObject bubbleFrame = UIManager.GetInstance().AddChild(_gridChatBubble.gameObject, EUIType.PersonalChatBubbleFrame);
            _gridChatBubble.GetComponent<RectTransform>().sizeDelta += new Vector2(0, bubbleFrame.GetComponent<LayoutElement>().preferredHeight);
            bubbleFrame.GetComponent<ChatBubbleFrame>().Show(chatDataItem);
            _chatBubbleList.Add(bubbleFrame.GetComponent<ChatBubbleFrame>());

            _inputChat.text = "";
            _scrollChatLog.verticalNormalizedPosition = 0;
        }

        public void OnClickFriendDetailButton()
        {
            StateManager.GetInstance().PushState<FriendDetailPanel>(EUIType.FriendDetailPanel, _guestUserItem);
        }
    }
}

