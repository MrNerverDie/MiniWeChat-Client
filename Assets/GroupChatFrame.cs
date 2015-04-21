using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

using protocol;

namespace MiniWeChat
{
    public class GroupChatFrame : ChatFrame
    {

        public override void Show(ChatLog chatLog)
        {
            _chatLog = chatLog;

            // Set GroupItem  //
            GroupItem groupItem = GlobalGroup.GetInstance().GetGroup(chatLog.chatID);
            if (groupItem != null)
            {
                if (_labelUserName)
                {
                    _labelUserName.text = groupItem.groupId;
                }
            }

            if (_labelLastChat)
            {
                _labelLastChat.text = GlobalChat.GetInstance().GetLastChat(chatLog.chatID).chatBody;
            }

            if (_labelDate)
            {
                _labelDate.text = new System.DateTime(chatLog.date).ToString("yyyy/MM/dd HH:mm");
            }
        }

        public override void OnClickChatFrameButton()
        {
            StateManager.GetInstance().PushState<GroupChatPanel>(EUIType.GroupChatPanel, _chatLog);
        }
    }
}

