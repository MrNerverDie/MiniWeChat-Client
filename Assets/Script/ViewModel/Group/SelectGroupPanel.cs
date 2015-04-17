using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace MiniWeChat
{
    public class SelectGroupPanel : BaseState
    {
        public override void OnEnter(object param = null)
        {
            base.OnEnter(param);
            UIManager.GetInstance().AddChild(transform.Find("TopBar").gameObject, EUIType.BackButton);
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

