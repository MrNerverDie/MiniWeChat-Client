using UnityEngine;
using UnityEditor;
using System.Collections;

public class MonoPanel1 : MonoBehaviour {

	// Use this for initialization
	void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Button1Callback()
    {
        StateManager.GetInstance().PushState<TestState>();
    }
}
