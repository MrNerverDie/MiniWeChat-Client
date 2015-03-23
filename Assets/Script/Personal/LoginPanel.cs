using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using protocol;

namespace MiniWeChat
{
    public class LoginPanel : BaseState
    {
        public InputField _inputId;
        public InputField _inputPassword;
        public Button _buttonLogin;
        public Button _buttonRegister;

        private string _userID;
        private string _userPassword;

        public override void OnEnter(object param = null)
        {
            base.OnEnter(param);
            _inputId.onValueChange.AddListener(OnValueChangeLoginInfo);
            _inputPassword.onValueChange.AddListener(OnValueChangeLoginInfo);
            _buttonLogin.onClick.AddListener(OnClickLoginButton);
            _buttonRegister.onClick.AddListener(OnCLickRegisterButton);
            _buttonLogin.interactable = false;
        }

        public override void OnExit()
        {
            base.OnExit();
            UIManager.GetInstance().DestroySingleUI(EUIType.LoginPanel);
        }

        public override void OnShow()
        {
            base.OnShow();
            MessageDispatcher.GetInstance().RegisterMessageHandler((uint)ENetworkMessage.LOGIN_RSP, OnLoginRsp);
        }

        public override void OnHide()
        {
            base.OnHide();
            MessageDispatcher.GetInstance().UnRegisterMessageHandler((uint)ENetworkMessage.LOGIN_RSP, OnLoginRsp);
        }

        public void OnValueChangeLoginInfo(string text)
        {
            if (_inputId.text == "" || _inputPassword.text == "")
            {
                _buttonLogin.interactable = false;
            }
            else
            {
                _buttonLogin.interactable = true;
            }
        }

        public void OnClickLoginButton()
        {
            LoginReq req = new LoginReq
            {
                userId = _inputId.text,
                userPassword = _inputPassword.text,
            };
            _userID = req.userId;
            _userPassword = req.userPassword;
            NetworkManager.GetInstance().SendPacket<LoginReq>(ENetworkMessage.LOGIN_REQ, req);
            _buttonLogin.interactable = false;
        }

        public void OnCLickRegisterButton()
        {
            GameObject go = UIManager.GetInstance().GetSingleUI(EUIType.RegisterPanel);
            StateManager.GetInstance().PushState<RegisterPanel>(go);
        }

        public void OnLoginRsp(uint iMessageType, object kParam)
        {
            LoginRsp rsp = kParam as LoginRsp;
            Debug.Log(rsp.resultCode);
            if (rsp.resultCode == LoginRsp.ResultCode.SUCCESS)
            {
                GameObject go = UIManager.GetInstance().GetSingleUI(EUIType.MainMenuPanel);
                StateManager.GetInstance().PushState<MainMenuPanel>(go);
            }
        }
    }
}

