using UnityEngine;
using System.Collections;

public class MonoPanel2 : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Button2Callback()
    {
        StateManager.GetInstance().PopState();
    }
}
