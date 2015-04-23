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
            base.Show(chatLog);

            // Set GroupItem  //
            GroupItem groupItem = GlobalGroup.GetInstance().GetGroup(chatLog.chatID);
            if (groupItem != null)
            {
                if (_labelUserName)
                {
                    _labelUserName.text = groupItem.groupName;
                }
                Log4U.LogDebug(groupItem.groupName);
            }
        }

        public override void OnClickChatFrameButton()
        {
            StateManager.GetInstance().PushState<GroupChatPanel>(EUIType.GroupChatPanel, _chatLog);
        }
    }
}

