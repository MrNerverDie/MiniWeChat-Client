using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using protocol;

namespace MiniWeChat
{
    public class ContactFrame : BaseWidget
    {
        public Image _imageHead;
        public Text _labelName;

        private UserItem _userItem;

        public void Start()
        {
            GetComponent<Button>().onClick.AddListener(OnClickContactFrame);
        }

        public override void Show(object param = null)
        {
            base.Show(param);
            _userItem = param as UserItem;
            if (_userItem != null)
            {
                UIManager.GetInstance().SetImage(_imageHead, EAtlasName.Head, "00" + _userItem.headIndex);
                _labelName.text = _userItem.userName;
            }
        }

        public override void Hide()
        {
            base.Hide();
        }

        public void OnClickContactFrame()
        {
            StateManager.GetInstance().PushState<FriendDetailPanel>(EUIType.FriendDetailPanel, _userItem);
        }


    }
}

