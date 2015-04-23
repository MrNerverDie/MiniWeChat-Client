using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

namespace MiniWeChat
{

    public class StateManager : Singleton<StateManager>
    {
        private Stack<BaseState> _stateStack;

        public override void Init()
        {
            _stateStack = new Stack<BaseState>();

            PushFirstState();
        }

        public void PushFirstState()
        {
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
        }

        /// <summary>
        /// 对界面进行压栈
        /// </summary>
        /// <typeparam name="T">界面的类型</typeparam>
        /// <param name="uiType">界面对应的uiType</param>
        /// <param name="param">界面对应的压栈参数</param>
        public void PushState<T>(EUIType uiType, object param = null) where T : BaseState
        {
            GameObject go = UIManager.GetInstance().GetSingleUI(uiType);
            PushState<T>(go, param);
        }

        /// <summary>
        /// 对界面进行压栈
        /// </summary>
        /// <typeparam name="T">下一个界面的类型</typeparam>
        /// <param name="go">下一个界面对应的GameObject</param>
        /// <param name="param">需要传给下一个界面对应的压栈参数</param>
        public void PushState<T>(GameObject go, object param = null) where T : BaseState
        {
            T nextState = go.GetComponent<T>();
            nextState.OnEnter(param);

            if (_stateStack.Count != 0)
            {
                BaseState curState = _stateStack.Peek();
                curState.DisableTouch();
                Tweener tweener = nextState.BeginEnterTween();
                tweener.OnComplete(delegate()
                {
                    curState.OnHide();
                    curState.EnabelTouch();
                });
            }
            _stateStack.Push(nextState);
        }

        /// <summary>
        /// 对界面进行出栈
        /// </summary>
        /// <param name="param">这个界面需要传给上一个界面的参数</param>
        public void PopState(object param = null)
        {
            if (_stateStack.Count != 0)
            {
                BaseState curState = _stateStack.Pop();
                Tweener tweener = curState.BeginExitTween();
                tweener.OnComplete(delegate()
                {
                    curState.OnExit();
                });
            }

            if (_stateStack.Count != 0)
            {
                BaseState lastState = _stateStack.Peek();
                lastState.OnShow();
            }


        }

        /// <summary>
        /// 对界面进行压栈
        /// </summary>
        /// <typeparam name="T">界面的类型</typeparam>
        /// <param name="uiType">界面对应的uiType</param>
        /// <param name="param">界面对应的压栈参数</param>
        public void ReplaceState<T>(EUIType uiType, object param = null) where T : BaseState
        {
            GameObject go = UIManager.GetInstance().GetSingleUI(uiType);
            ReplaceState<T>(go, param);
        }

        /// <summary>
        /// 对栈顶的界面进行Replace
        /// </summary>
        /// <typeparam name="T">下一个界面的类型</typeparam>
        /// <param name="go">下一个界面</param>
        /// <param name="param">还给下一个界面的参数</param>
        public void ReplaceState<T>(GameObject go, object param = null) where T : BaseState
        {
            T nextState = go.GetComponent<T>();

            if (_stateStack.Count != 0)
            {
                BaseState curState = _stateStack.Pop();
                curState.OnExit();
            }
            nextState.OnEnter(param);
            _stateStack.Push(nextState);
        }

        /// <summary>
        /// 
        /// </summary>
        public void ClearStates()
        {
            while (_stateStack.Count != 0)
            {
                BaseState curState = _stateStack.Pop();
                curState.OnExit();
            }
        }

        public void ClearStatesExceptBottom(bool isShowBottom = false)
        {
            while (_stateStack.Count != 1)
            {
                BaseState curState = _stateStack.Pop();
                curState.OnExit();
            }

            if (isShowBottom)
            {
                if (_stateStack.Count == 1)
                {
                    BaseState lastState = _stateStack.Peek();
                    lastState.OnShow();
                }
            }
        }

        public void Update()
        {
#if UNITY_ANDROID
            if (Input.GetKeyUp(KeyCode.Escape) && _stateStack.Count == 1)
            {
                DialogManager.GetInstance().CreateDoubleButtonDialog(
                    "您确定要退出微信吗？", 
                    "警告",
                    OnClickConfirmExit);
            }
#endif
        }

        public void OnClickConfirmExit()
        {
            Application.Quit();
        }
    }
}