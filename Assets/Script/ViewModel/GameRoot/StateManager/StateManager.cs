using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace MiniWeChat
{

    public class StateManager : Singleton<StateManager>
    {
        private Stack<BaseState> _stateStack;

        public override void Init()
        {
            _stateStack = new Stack<BaseState>();

            GameObject welcomePanel = UIManager.GetInstance().GetSingleUI(EUIType.WelcomePanel);
            PushState<WelcomePanel>(welcomePanel);
        }

        public void PushState<T>(EUIType uiType, object param = null) where T : BaseState
        {
            GameObject go = UIManager.GetInstance().GetSingleUI(uiType);
            PushState<T>(go, param);
        }

        public void PushState<T>(GameObject go, object param = null) where T : BaseState
        {
            T nextState = go.GetComponent<T>();

            if (_stateStack.Count != 0)
            {
                BaseState curState = _stateStack.Peek();
                curState.OnHide();
            }
            nextState.OnEnter(param);
            _stateStack.Push(nextState);
        }

        public void PopState(object param = null)
        {
            if (_stateStack.Count != 0)
            {
                BaseState curState = _stateStack.Pop();
                curState.OnExit();
            }

            if (_stateStack.Count != 0)
            {
                BaseState lastState = _stateStack.Peek();
                lastState.OnShow();
            }
        }

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

        public void ClearStates()
        {
            while (_stateStack.Count != 0)
            {
                BaseState curState = _stateStack.Pop();
                curState.OnExit();
            }
        }

        public void ClearStatesExceptBottom()
        {
            while (_stateStack.Count != 1)
            {
                BaseState curState = _stateStack.Pop();
                curState.OnExit();
            }
        }
    }

    public class BaseState : MonoBehaviour
    {
        /// <summary>
        /// 界面栈初始化的时候被调用的方法
        /// </summary>
        /// <param name="param">需要传进来的参数</param>
        public virtual void OnEnter(object param = null)
        {
            OnShow();
        }

        /// <summary>
        /// 界面栈销毁的时候被调用的方法
        /// </summary>
        public virtual void OnExit()
        {
            OnHide();
        }

       /// <summary>
       /// 界面栈隐藏时调用的方法，当界面销毁的时候也会被调用
       /// </summary>
        public virtual void OnHide()
        {
            gameObject.SetActive(false);
        }

        /// <summary>
        /// 界面栈显示时调用的方法，当界面初始化的时候也会被调用
        /// </summary>
        /// <param name="param"></param>
        public virtual void OnShow(object param = null)
        {
            gameObject.SetActive(true);
        }
    }

    public class CallBackWithString
    {
        public UnityEngine.Events.UnityAction<string> callback;
    }
}