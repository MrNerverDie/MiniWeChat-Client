using UnityEngine;
using System;
using System.Collections;

public class GameRoot
{
	private static GameRoot _instance;
	GameObject _rootObj;

	MessageDispatcher _messageDispatcher;
	UIManager _uiManager;
	StateManager _stateManager;

	private GameRoot()
	{
		_rootObj = new GameObject("GameRoot");
		GameObject.DontDestroyOnLoad(_rootObj);

		_messageDispatcher = AddSingleton<MessageDispatcher>(_rootObj);
		_uiManager = AddSingleton<UIManager>(_rootObj);
		_stateManager = AddSingleton<StateManager>(_rootObj);
	}

	private static T AddSingleton<T>(GameObject go) where T : Singleton<T>
	{
		T t = go.AddComponent<T>();
		t.Init();
		return t;
	}

	public static T GetSingleton<T>() where T : Singleton<T>
	{
		if (_instance == null)
		{
			_instance = new GameRoot();
		}

		T t = _instance._rootObj.GetComponent<T>();

		if (t == null)
		{
			throw new Exception("Failed To Get Singleton : " + typeof(T));
		}

		return t;
	}
}
