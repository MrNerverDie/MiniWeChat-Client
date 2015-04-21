using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using protocol;

namespace MiniWeChat
{
    public class GroupFrame : MonoBehaviour
    {
        public Image _imageHead;
        public Text _labelUserName;

        public Button _buttonEnterGroup;

        private GroupItem _groupItem;

        public void Show(GroupItem groupItem)
        {
            _groupItem = groupItem;

            if (_groupItem != null)
            {
                _labelUserName.text = _groupItem.groupName;
            }

            _buttonEnterGroup.onClick.AddListener(OnClickEnterGroupChat);
        }

        public void OnClickEnterGroupChat()
        {
            ChatLog chatLog = GlobalChat.GetInstance().GetChatLog(_groupItem.groupId);
            StateManager.GetInstance().ClearStatesExceptBottom();
            StateManager.GetInstance().PushState<GroupChatPanel>(EUIType.GroupChatPanel, chatLog);
        }
		
    }
}

