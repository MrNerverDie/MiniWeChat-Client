using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public abstract class BaseState
{
    public abstract void OnEnter(object param = null);

    public abstract void OnExit();

}

public class StateManager : Singleton<StateManager>{

    private Stack<BaseState> _stateStack;

	public override void Init()
	{
        _stateStack = new Stack<BaseState>();
    }

    public void PushState<T>(object param = null) where T : BaseState
    {
        T nextState = Activator.CreateInstance<T>();

        if (_stateStack.Count != 0)
        {
            BaseState curState = _stateStack.Peek();
            curState.OnExit();            
        }
        nextState.OnEnter(param);
        _stateStack.Push(nextState);
    }

    public void PopState()
    {
        if (_stateStack.Count != 0)
        {
            BaseState curState = _stateStack.Pop();
            curState.OnExit();
        }

        if (_stateStack.Count != 0)
        {
            BaseState lastState = _stateStack.Peek();
            lastState.OnEnter();
        }
    }

    public void ReplaceState<T>(object param = null) where T : BaseState
    {
        T nextState = Activator.CreateInstance<T>();

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
}
