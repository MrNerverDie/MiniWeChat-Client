using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace MiniWeChat
{
    public class ChatListPanel : MonoBehaviour
    {
        private const int CHAT_FRAME_HEIGHT = 200;

        public VerticalLayoutGroup _chatGrid;

        public void Start()
        {
            int chatNum = 10;
            _chatGrid.GetComponent<RectTransform>().sizeDelta = new Vector2(1080, CHAT_FRAME_HEIGHT * chatNum);
            for (int i = 0; i < chatNum; i++)
            {
                GameObject go = UIManager.GetInstance().AddChild(_chatGrid.gameObject, EUIType.ChatFrame);
            }
        }

        public void Show(bool isShow)
        {
            gameObject.SetActive(isShow);
        }

    }
}

