using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using protocol;

namespace MiniWeChat
{
    public class PersonalPanel : BasePanel
    {
        public enum PersonalSetType
        {
            NAME = 0,
            PASSWORD,
            HEAD,
        }

        public Text _laeblName;
        public Text _labelId;
        public Image _imageHead;

        public Button _buttonSetName;
        public Button _buttonSetPassword;
        public Button _buttonSetHead;
        public Button _buttonExit;

        private PersonalSetType _personalSetType;

        public override void Show(object param = null)
        {
            base.Show(param);

            _buttonExit.onClick.AddListener(OnClickExitButton);
            _buttonSetPassword.onClick.AddListener(OnClickSetPassword);
            _buttonSetName.onClick.AddListener(OnClickSetName);
            _buttonSetHead.onClick.AddListener(OnClickSetHead);
            MessageDispatcher.GetInstance().RegisterMessageHandler((uint)ENetworkMessage.GETUSERINFO_RSP, OnGetUserInfoRsp);

            _laeblName.text = GlobalUser.GetInstance().UserName;
            _labelId.text = GlobalUser.GetInstance().UserId;
            UIManager.GetInstance().SetImage(_imageHead, EAtlasName.Head, "00" + GlobalUser.GetInstance().HeadIndex);
        }

        public override void Hide()
        {
            base.Hide();

            _buttonExit.onClick.RemoveAllListeners();
            _buttonSetName.onClick.RemoveAllListeners();
            _buttonSetHead.onClick.RemoveAllListeners();
            _buttonSetPassword.onClick.RemoveAllListeners();
            MessageDispatcher.GetInstance().UnRegisterMessageHandler((uint)ENetworkMessage.GETUSERINFO_RSP, OnGetUserInfoRsp);


        }

        public void OnClickExitButton()
        {
            NetworkManager.GetInstance().SendPacket<LogoutReq>(ENetworkMessage.LOGOUT_REQ, new LogoutReq());
        }

        public void OnClickSetName()
        {
            DialogManager.GetInstance().CreateDoubleButtonInputDialog("修改昵称", "昵称", "长度不能超过6", GlobalUser.GetInstance().UserName, InputField.ContentType.Standard, OnConfirmChange);
            _personalSetType = PersonalSetType.NAME;
        }

        public void OnClickSetPassword()
        {
            DialogManager.GetInstance().CreateDoubleButtonInputDialog("修改密码", "密码", "长度不能超过20", GlobalUser.GetInstance().UserPassword, InputField.ContentType.Password, OnConfirmChange);
            _personalSetType = PersonalSetType.PASSWORD;
        }

        public void OnClickSetHead()
        {
            GameObject go = UIManager.GetInstance().GetSingleUI(EUIType.ImageListPanel);
            StateManager.GetInstance().PushState<ImageListPanel>(go, new CallBackWithString { callback = OnConfirmChange });
            _personalSetType = PersonalSetType.HEAD;
        }

        public void OnConfirmChange(string text)
        {
            PersonalSettingsReq req = new PersonalSettingsReq();

            if (_personalSetType == PersonalSetType.PASSWORD)
            {
                req.userPassword = text;
            }
            else if (_personalSetType == PersonalSetType.NAME)
            {
                req.userName = text;
            }
            else if (_personalSetType == PersonalSetType.HEAD)
            {
                req.headIndex = int.Parse(text);
            }

            NetworkManager.GetInstance().SendPacket<PersonalSettingsReq>(ENetworkMessage.PERSONALSETTINGS_REQ, req);
        }

        public void OnGetUserInfoRsp(uint iMessageType, object kParam)
        {
            GetUserInfoRsp rsp = kParam as GetUserInfoRsp;
            if (rsp.resultCode == GetUserInfoRsp.ResultCode.SUCCESS)
            {
                _laeblName.text = rsp.userItem.userName;
                UIManager.GetInstance().SetImage(_imageHead, EAtlasName.Head, "00" + rsp.userItem.headIndex);
            }
        }


    }
}

