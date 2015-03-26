using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using protocol;

namespace MiniWeChat
{
    public class MainMenuPanel : BaseState
    {
        public Button _searchButton;

        public override void OnEnter(object param = null)
        {
            base.OnEnter(param);
            _searchButton.onClick.AddListener(OnClickSearchButton);
        }

        public override void OnExit()
        {
            base.OnExit();
            UIManager.GetInstance().DestroySingleUI(EUIType.MainMenuPanel);
        }

        public override void OnShow(object param = null)
        {
            base.OnShow(param);
            //MessageDispatcher.GetInstance().RegisterMessageHandler((uint))
        }

        public override void OnHide()
        {
            base.OnHide();
        }

        public void OnClickSearchButton()
        {
            DialogManager.GetInstance().CreateDoubleButtonInputDialog("搜索用户", "搜索", "长度不能超过20", "", InputField.ContentType.Alphanumeric, OnSubmitSearchReq);
        }

        public void OnSubmitSearchReq(string text)
        {
            GetUserInfoReq req = new GetUserInfoReq
            {
                targetUserId = text,
            };

            NetworkManager.GetInstance().SendPacket<GetUserInfoReq>(ENetworkMessage.GETUSERINFO_REQ, req);
        }
    }
}
