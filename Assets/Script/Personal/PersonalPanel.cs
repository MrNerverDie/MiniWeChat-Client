using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace MiniWeChat
{
    public class PersonalPanel : BasePanel
    {
        public Text _laeblName;
        public Text _labelId;

        public Button _buttonSetName;
        public Button _buttonSetPassword;
        public Button _buttonSetHead;
        public Button _buttonExit;

        public override void Show(object param = null)
        {
            base.Show(param);
            _laeblName.text = GlobalUser.GetInstance().UserName;
            _labelId.text = GlobalUser.GetInstance().UserId;
        }

        public override void Hide()
        {
            base.Hide();
        }

        public void OnClickExitButton()
        {
            StateManager.GetInstance().ClearStates();
            GameObject go = UIManager.GetInstance().GetSingleUI(EUIType.LoginPanel);
            StateManager.GetInstance().PushState<LoginPanel>(go);
        }

    }
}

