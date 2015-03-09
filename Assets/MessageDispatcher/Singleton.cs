using System;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Singleton<T>  
{
    public static T GetInstance()
    {
		return GameRoot.GetSingleton<T>();	
    }

	public virtual void Init()
	{
		return;
	}
}
