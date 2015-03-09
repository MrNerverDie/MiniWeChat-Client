using UnityEngine;
using System.Collections;

public class TestState : BaseState {
    public override void OnEnter(object param)
    {
        UIManager.GetInstance().GetUI("Panel2");
    }

    public override void OnExit()
    {
        UIManager.GetInstance().DestroyUI("Panel2");
    }
}
