using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using protocol;

namespace MiniWeChat
{
    public class GroupMemberFrame : MonoBehaviour
    {
        public Image _imageHead;
        public Text _labelUserName;

        private UserItem _userItem;

        public void Show(UserItem userItem)
        {
            _userItem = userItem;

            if (_userItem != null)
            {
                UIManager.GetInstance().SetImage(_imageHead, EAtlasName.Head, "00" + _userItem.headIndex);
                _labelUserName.text = _userItem.userName;
            }
        }

        
    }
}

