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
            }

            if (_imageHead != null)
            {
                int headCount = groupItem.memberUserId.Count > 9 ? 9 : groupItem.memberUserId.Count;
                UIManager.GetInstance().RefreshChildren(_imageHead.gameObject, EUIType.GroupMemberHeadIcon, headCount);

                for (int i = 0; i < headCount; i++)
                {
                    string userID = groupItem.memberUserId[i];
                    UserItem userItem = GlobalGroup.GetInstance().GetGroupMember(userID);
                    if (userItem == null)
                    {
                        userItem = new UserItem();
                    }

                    GameObject go = _imageHead.transform.GetChild(i).gameObject;
                    go.GetComponent<GroupMemberFrame>().Show(userItem);
                }
            }
        }

        public override void OnClickChatFrameButton()
        {
            StateManager.GetInstance().PushState<GroupChatPanel>(EUIType.GroupChatPanel, _chatLog);
        }
    }
}

