using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace MiniWeChat
{
    public class ChatInputBar : MonoBehaviour
    {
        public Button _buttonSend;
        public InputField _inputFieldChat;
        public VerticalLayoutGroup _gridChatBubble;

        // Use this for initialization
        void Start()
        {
            _buttonSend.onClick.AddListener(OnClickSendButton);
        }

        public void OnClickSendButton()
        {
            GameObject bubbleFrame = UIManager.GetInstance().AddChild(_gridChatBubble.gameObject, EUIType.PersonalChatBubbleFrame);
            bubbleFrame.GetComponent<ChatBubbleFrame>().Show(_inputFieldChat.text);
            _inputFieldChat.text = "";
        }

    }
}

