using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using protocol;

namespace MiniWeChat
{
    public class ChatPanel : BaseState
    {
        public InputField _inputChat;
        public ScrollRect _scrollChatLog;
        public Button _buttonSend;
        public VerticalLayoutGroup _gridChatBubble;

        private UserItem _guestUserItem;

        public override void OnEnter(object param = null)
        {
            _guestUserItem = param as UserItem;

            base.OnEnter(param);
            UIManager.GetInstance().AddChild(gameObject, EUIType.BackButton);
            _gridChatBubble.GetComponent<RectTransform>().sizeDelta = new Vector2(GlobalVars.DEFAULT_SCREEN_WIDTH, 0);
            _buttonSend.onClick.AddListener(OnClickSendButton);
            _scrollChatLog.verticalNormalizedPosition = 0;
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

        public void OnClickSendButton()
        {
            GameObject bubbleFrame = UIManager.GetInstance().AddChild(_gridChatBubble.gameObject, EUIType.PersonalChatBubbleFrame);
            bubbleFrame.GetComponent<ChatBubbleFrame>().Show(_inputChat.text, GlobalUser.GetInstance().Self);
            _inputChat.text = "";
            _gridChatBubble.GetComponent<RectTransform>().sizeDelta += new Vector2(0, bubbleFrame.GetComponent<RectTransform>().sizeDelta.y);
            _scrollChatLog.verticalNormalizedPosition = 0;

            ChatDataItem chatDataItem = new ChatDataItem
            {
                sendUserId = GlobalUser.GetInstance().UserId,
                receiveUserId = _guestUserItem.userId,
                date = System.DateTime.Now.Ticks,
                chatType = ChatDataItem.ChatType.TEXT,
                chatBody = _inputChat.text,
            };
            GlobalChat.GetInstance().SendChatReq(chatDataItem);

        }
    }
}

