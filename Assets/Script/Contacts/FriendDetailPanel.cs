using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using protocol;

namespace MiniWeChat
{

    public class FriendDetailPanel : BaseState
    {

        public Text _laeblName;
        public Text _labelId;
        public Image _imageHead;

        public Button _buttonAddFriend;
        public Button _buttonDeleteFriend;
        public Button _buttonBeginChat;

        private UserItem _userItem;

        public override void OnEnter(object param)
        {
            base.OnEnter(param);

            _userItem = param as UserItem;

            _laeblName.text = _userItem.userName;
            _labelId.text = _userItem.userId;
            UIManager.GetInstance().SetImage(_imageHead, EAtlasName.Head, "00" + _userItem.headIndex);

            InitButtons();
        }

        public override void OnExit()
        {
            base.OnExit();
            UIManager.GetInstance().DestroySingleUI(EUIType.FriendDetailPanel);
        }

        public override void OnShow(object param = null)
        {
            base.OnShow(param);
        }

        public override void OnHide()
        {
            base.OnHide();
        }

        private void InitButtons()
        {
            _buttonAddFriend.gameObject.SetActive(false);
            _buttonDeleteFriend.gameObject.SetActive(false);
            _buttonBeginChat.gameObject.SetActive(false);

            if (_userItem.userId == GlobalUser.GetInstance().UserId)
            {
                return;
            }
        }



    }
}

