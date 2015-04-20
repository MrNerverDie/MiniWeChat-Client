using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using protocol;

namespace MiniWeChat
{
    public class ToggleGroupMemberParam
    {
        public string userID;
        public bool check;
    }


    public class GroupMemberFrame : MonoBehaviour
    {

        // Component //
        public Image _imageHead;
        public Text _labelUserName;

        public Toggle _toggleAddUser;

        // DataItem //
        private UserItem _userItem;

        public void Show(UserItem userItem)
        {
            _userItem = userItem;

            if (_userItem != null)
            {
                if (_imageHead)
                {
                    UIManager.GetInstance().SetImage(_imageHead, EAtlasName.Head, "00" + _userItem.headIndex);                                    
                }

                if (_labelUserName)
                {
                    _labelUserName.text = _userItem.userName;                    
                }
            }

            if (_toggleAddUser)
            {
                _toggleAddUser.onValueChanged.AddListener(OnClickToggleMemeber);                
            }
        }

        public void OnClickToggleMemeber(bool check)
        {
            MessageDispatcher.GetInstance().DispatchMessage((uint)EUIMessage.TOGGLE_GROUP_MEMBER,
                new ToggleGroupMemberParam { 
                    userID = _userItem.userId,
                    check = check,
                });
        }
    }
}

