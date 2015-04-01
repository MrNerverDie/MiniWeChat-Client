using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using protocol;

namespace MiniWeChat
{
    public class ChatBubbleFrame : MonoBehaviour
    {
        private const float IMAGE_BUBBLE_WIDTH_BASE = 150f;
        private const float IMAGE_BUBBLE_HEIGHT_BASE = 125f;

        private const float FRAME_BUBBLE_HEIGHT_BASE = 200f;

        private const float WIDTH_INCREMENT = 50f;
        private const float HEIGHT_INCREMENT = 55f;

        private const float MAX_CHAR_NUM_ONE_LINE = 13;

        public RectTransform _imageChatBubble;
        public RectTransform _frameChatBubble;
        public Text _textChat;
        public Image _imageHead;

        /// <summary>
        /// 在一个聊天窗口中显示一个字符串
        /// </summary>
        /// <param name="text">字符串</param>
        public void Show(ChatDataItem chatDataItem)
        {
            string text = chatDataItem.chatBody;
            UserItem userItem = GlobalUser.GetInstance().Self;
            if (chatDataItem.sendUserId != GlobalUser.GetInstance().UserId)
            {
                userItem = GlobalContacts.GetInstance().GetUserItemById(chatDataItem.sendUserId);
            }

            int lines = 0;
            float maxCharNumInOneLine = 0;

            SetTextParam(ref text,ref lines,ref maxCharNumInOneLine);

            _textChat.text = text;
            _imageChatBubble.sizeDelta = new Vector2(
                IMAGE_BUBBLE_WIDTH_BASE + WIDTH_INCREMENT * (maxCharNumInOneLine - 1),
                IMAGE_BUBBLE_HEIGHT_BASE + HEIGHT_INCREMENT * (lines - 1)
                );
            _frameChatBubble.sizeDelta = new Vector2(
                GlobalVars.DEFAULT_SCREEN_WIDTH,
                FRAME_BUBBLE_HEIGHT_BASE + HEIGHT_INCREMENT * (lines - 1) 
                );
            _frameChatBubble.GetComponent<LayoutElement>().preferredHeight = _frameChatBubble.sizeDelta.y;

            UIManager.GetInstance().SetImage(_imageHead, EAtlasName.Head, "00" + userItem.headIndex);
        }

        /// <summary>
        /// 对传入的字符串进行排版处理
        /// </summary>
        /// <param name="text">聊天的字符串</param>
        /// <param name="lines">需要计算传出的行数</param>
        /// <param name="maxCharNumInOneLine">最终进行排版之后的字符串中一行中最多的字符数</param>
        private void SetTextParam(ref string text, ref int lines, ref float maxCharNumInOneLine)
        {
            float curCharNumInOneLine = 0;

            for (int i = 0; i < text.Length; i++)
            {
                char character = text[i];

                if (character == '\n')
                {
                    lines++;
                    if (maxCharNumInOneLine < MAX_CHAR_NUM_ONE_LINE && curCharNumInOneLine > maxCharNumInOneLine)
                    {
                        maxCharNumInOneLine = curCharNumInOneLine;
                    }
                    curCharNumInOneLine = 0;
                    continue;
                }

                if (curCharNumInOneLine >= MAX_CHAR_NUM_ONE_LINE)
                {
                    text = text.Insert(i, "\n");
                    i--;
                    continue;
                }

                if ((int)character > 160)
                {
                    curCharNumInOneLine++;
                }
                else
                {
                    curCharNumInOneLine += 0.5f;
                }
            }

            lines++;
            if (maxCharNumInOneLine < MAX_CHAR_NUM_ONE_LINE && curCharNumInOneLine > maxCharNumInOneLine)
            {
                maxCharNumInOneLine = (int)Mathf.Round(curCharNumInOneLine);
            }
        }
        
    }
}


