using UnityEngine;
using UnityEngine.UI;
using System.Collections;

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

        // Use this for initialization
        void Start()
        {

        }

        public void Show(string text)
        {
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
        }

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


