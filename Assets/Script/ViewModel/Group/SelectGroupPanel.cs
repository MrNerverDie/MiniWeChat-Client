using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

using protocol;

namespace MiniWeChat
{
    public class SelectGroupPanel : BaseState
    {
        public RectTransform _gridSelectGroup;
        private const float GROUP_FRAME_HEIGHT = 150f;

        public override void OnEnter(object param = null)
        {
            base.OnEnter(param);
            UIManager.GetInstance().AddChild(transform.Find("TopBar").gameObject, EUIType.BackButton);

            InitGroupFrames();
        }

        public override void OnShow(object param = null)
        {
            base.OnShow(param);
        }

        public override void OnHide()
        {
            base.OnHide();
        }

        private void InitGroupFrames()
        {
            foreach (GroupItem groupItem in GlobalGroup.GetInstance())
            {
                GameObject go = UIManager.GetInstance().AddChild(_gridSelectGroup.gameObject, EUIType.GroupFrame);
                go.GetComponent<GroupFrame>().Show(groupItem);
            }

            _gridSelectGroup.sizeDelta = new Vector2(GlobalVars.DEFAULT_SCREEN_WIDTH, 
                GlobalGroup.GetInstance().Count * GROUP_FRAME_HEIGHT);  
        }
    }
}

