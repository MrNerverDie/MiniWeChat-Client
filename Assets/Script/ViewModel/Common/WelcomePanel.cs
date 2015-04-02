using UnityEngine;
using System.Collections;

namespace MiniWeChat
{
    public class WelcomePanel : BaseState 
    {
        private const float WELCOME_LOAD_DURATION = 1.0f;

        public override void OnEnter(object param = null)
        {
            base.OnEnter(param);
            StartCoroutine(BeginWelcomeLoad());
        }

        public override void OnExit()
        {
            base.OnExit();
            UIManager.GetInstance().DestroySingleUI(EUIType.WelcomePanel);
        }

        private IEnumerator BeginWelcomeLoad()
        {
            yield return new WaitForSeconds(WELCOME_LOAD_DURATION);
            if (GlobalUser.GetInstance().IsEnterMainMenu == false)
            {
                GameObject go = UIManager.GetInstance().GetSingleUI(EUIType.LoginPanel);
                StateManager.GetInstance().ReplaceState<LoginPanel>(go);
            }
            else
            {
                GameObject go = UIManager.GetInstance().GetSingleUI(EUIType.MainMenuPanel);
                StateManager.GetInstance().ReplaceState<MainMenuPanel>(go);
            }

            GameObject label = UIManager.GetInstance().GetSingleUI(EUIType.StatusLabel);
            label.GetComponent<StatusLabel>().Show();
        }
    }
}

