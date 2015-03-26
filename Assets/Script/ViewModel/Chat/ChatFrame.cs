using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace MiniWeChat
{
    public class ChatFrame : BaseWidget
    {

        public Button _buttonChatFrame;

        // Use this for initialization
        void Start()
        {
            _buttonChatFrame.onClick.AddListener(OnClickChatFrameButton);
        }

        public void OnClickChatFrameButton()
        {
            GameObject chatPanel = UIManager.GetInstance().GetSingleUI(EUIType.ChatPanel);
            StateManager.GetInstance().PushState<ChatPanel>(chatPanel);
        }

    }
}

