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
            GameObject go = UIManager.GetInstance().GetSingleUI(EUIType.RegisterPanel);
            StateManager.GetInstance().ReplaceState<RegisterPanel>(go);
        }
    }
}

