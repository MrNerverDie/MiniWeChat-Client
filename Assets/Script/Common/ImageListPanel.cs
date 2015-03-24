using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace MiniWeChat
{
    public class ImageListPanel : BaseState
    {

        public GridLayoutGroup _gridImage;

        public List<Button> _imageList;

        private CallBackWithString _callbackWrapper;

        public override void OnEnter(object param = null)
        {
            base.OnEnter(param);
            UIManager.GetInstance().AddChild(gameObject, EUIType.BackButton);
            _callbackWrapper = param as CallBackWithString;
        }

        public override void OnExit()
        {
            base.OnExit();
            UIManager.GetInstance().DestroySingleUI(EUIType.ImageListPanel);
        }

        public override void OnShow(object param = null)
        {
            base.OnShow(param);
        }

        public override void OnHide()
        {
            base.OnHide();
        }

        public void OnSelectImage(int index)
        {
            StateManager.GetInstance().PopState(index);
            if (_callbackWrapper != null)
            {
                _callbackWrapper.callback("00"+index);
            }
        }
    }
}

