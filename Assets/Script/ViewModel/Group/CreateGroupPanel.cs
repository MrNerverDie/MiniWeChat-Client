using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

using protocol;

namespace MiniWeChat
{
    public class CreateGroupPanel : BaseState
    {
        public VerticalLayoutGroup _gridSelectMember;
        public HorizontalLayoutGroup _gridMember;

        public Button _buttonConfirm;

        private GroupItem _groupItem;

        public override void OnEnter(object param = null)
        {
            base.OnEnter(param);
            UIManager.GetInstance().AddChild(transform.Find("TopBar").gameObject, EUIType.BackButton);

            _groupItem = param as GroupItem;

            foreach (var friendItem in GlobalContacts.GetInstance())
            {
                
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            UIManager.GetInstance().DestroySingleUI(EUIType.CreateGroupPanel);
        }

        public override void OnShow(object param = null)
        {
            base.OnShow(param);
        }

        public override void OnHide()
        {
            base.OnHide();
        }
    }
}

