using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using protocol;

namespace MiniWeChat
{
    public class RegisterPanel : BaseState
    {
        public InputField _inputName;
        public InputField _inputId;
        public InputField _inputPassword;
        public Button _buttonRegister;

        private string _userID;
        private string _userPassword;

        public override void OnEnter(object param = null)
        {
            base.OnEnter(param);
            UIManager.GetInstance().AddChild(gameObject, EUIType.BackButton);

            _buttonRegister.interactable = false;
            _inputName.onValueChange.AddListener(OnValueChangeRegisterInfo);
            _inputId.onValueChange.AddListener(OnValueChangeRegisterInfo);
            _inputPassword.onValueChange.AddListener(OnValueChangeRegisterInfo);
            _buttonRegister.onClick.AddListener(OnClickRegisterButton);
        }

        public override void OnExit()
        {
            base.OnExit();
            UIManager.GetInstance().DestroySingleUI(EUIType.RegisterPanel);
        }

        public override void OnShow()
        {
            base.OnShow();
            MessageDispatcher.GetInstance().RegisterMessageHandler((uint)ENetworkMessage.REGISTER_RSP, OnRegisterRsp);
            MessageDispatcher.GetInstance().RegisterMessageHandler((uint)ENetworkMessage.LOGIN_RSP, OnLoginRsp);
            MessageDispatcher.GetInstance().RegisterMessageHandler((uint)EGeneralMessage.REQ_TIMEOUT, OnReqTimeOut);
        }

        public override void OnHide()
        {
            base.OnHide();
            MessageDispatcher.GetInstance().UnRegisterMessageHandler((uint)ENetworkMessage.REGISTER_RSP, OnRegisterRsp);
            MessageDispatcher.GetInstance().UnRegisterMessageHandler((uint)ENetworkMessage.LOGIN_RSP, OnLoginRsp);
            MessageDispatcher.GetInstance().UnRegisterMessageHandler((uint)EGeneralMessage.REQ_TIMEOUT, OnReqTimeOut);
        }

        public void OnValueChangeRegisterInfo(string text = null)
        {
            if (_inputName.text == "" || _inputId.text == "" || _inputPassword.text == "")
            {
                _buttonRegister.interactable = false;
            }
            else
            {
                _buttonRegister.interactable = true;
            }
        }

        public void OnClickRegisterButton()
        {
            RegisterReq req = new RegisterReq
            {
                userId = _inputId.text,
                userName = _inputName.text,
                userPassword = _inputPassword.text,
            };
            _userID = req.userId;
            _userPassword = req.userPassword;
            NetworkManager.GetInstance().SendPacket<RegisterReq>(ENetworkMessage.REGISTER_REQ, req);
            _buttonRegister.interactable = false;
        }

        public void OnRegisterRsp(uint iMessageType, object kParam)
        {
            RegisterRsp rsp = kParam as RegisterRsp;
            Debug.Log(rsp.resultCode);
            if (rsp.resultCode == RegisterRsp.ResultCode.SUCCESS)
            {
                LoginReq req = new LoginReq
                {
                    userId = _userID,
                    userPassword = _userPassword,
                };
                NetworkManager.GetInstance().SendPacket<LoginReq>(ENetworkMessage.LOGIN_REQ, req);
            }else
            {
                DialogManager.GetInstance().CreateSingleButtonDialog(rsp.resultCode.ToString());
                OnValueChangeRegisterInfo();
            }
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
            else
            {
                DialogManager.GetInstance().CreateSingleButtonDialog(rsp.resultCode.ToString());
            }
        }

        public void OnReqTimeOut(uint iMessageType, object kParam)
        {
            OnValueChangeRegisterInfo();
        }
    }
}

