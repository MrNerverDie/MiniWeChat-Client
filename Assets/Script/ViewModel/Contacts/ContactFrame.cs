using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using protocol;

namespace MiniWeChat
{
    public class ContactFrame : MonoBehaviour
    {
        public Image _imageHead;
        public Text _labelName;

        private UserItem _userItem;

        public void Start()
        {
            GetComponent<Button>().onClick.AddListener(OnClickContactFrame);
        }

        public void Show(UserItem userItem)
        {
            _userItem = userItem ;
            if (_userItem != null)
            {
                UIManager.GetInstance().SetImage(_imageHead, EAtlasName.Head, "00" + _userItem.headIndex);
                _labelName.text = _userItem.userName;
            }
        }

        public void Hide()
        {

        }

        public void OnClickContactFrame()
        {
            StateManager.GetInstance().PushState<FriendDetailPanel>(EUIType.FriendDetailPanel, _userItem);
        }


    }
}

