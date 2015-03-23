using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace MiniWeChat
{
    public class DialogManager : Singleton<DialogManager>
    {
        public override void Init()
        {
            base.Init();
        }

        public GameObject CreateSingleButtonDialog(string content, string title = null, UnityEngine.Events.UnityAction confirmCallback = null)
        {
            GameObject go = UIManager.GetInstance().GetSingleUI(EUIType.SingleButtonDialog);
            go.GetComponent<SingleButtonDialog>().Show(title, content, confirmCallback);
            return go;
        }
    }
}

