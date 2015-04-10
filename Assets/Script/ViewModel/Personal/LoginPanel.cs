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

        public override void OnShow(object param = null)
        {
            base.OnShow(param);
            MessageDispatcher.GetInstance().RegisterMessageHandler((uint)ENetworkMessage.LOGIN_RSP, OnLoginRsp);
            MessageDispatcher.GetInstance().RegisterMessageHandler((uint)EModelMessage.REQ_TIMEOUT, OnReqTimeOut);

            _inputId.text = PlayerPrefs.GetString(GlobalVars.PREF_USER_ID);
        }

        public override void OnHide()
        {
            base.OnHide();
            MessageDispatcher.GetInstance().UnRegisterMessageHandler((uint)ENetworkMessage.LOGIN_RSP, OnLoginRsp);
            MessageDispatcher.GetInstance().UnRegisterMessageHandler((uint)EModelMessage.REQ_TIMEOUT, OnReqTimeOut);

        }

        public void OnValueChangeLoginInfo(string text = null)
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
            GlobalUser.GetInstance().TryLogin(_inputId.text, _inputPassword.text);
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
                StateManager.GetInstance().ClearStates();
                StateManager.GetInstance().PushState<MainMenuPanel>(go);
            }
            else
            {
                DialogManager.GetInstance().CreateSingleButtonDialog(rsp.resultCode.ToString());
            }
        }

        public void OnReqTimeOut(uint iMessageType, object kParam)
        {
            OnValueChangeLoginInfo();
        }
    }
}

