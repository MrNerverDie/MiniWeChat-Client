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

            _buttonEnterGroup.onClick.AddListener(OnClickEnterGroup);
        }

        public void OnClickEnterGroup()
        {

        }
		
    }
}

