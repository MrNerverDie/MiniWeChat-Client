using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace MiniWeChat
{
    [RequireComponent(typeof(Toggle))]
    public class MiniToggle : MonoBehaviour
    {
        public GameObject _upGameObject;
        public GameObject _downGameObject;

        public void Awake()
        {
            GetComponent<Toggle>().onValueChanged.AddListener(OnToggleValueChanged);
            OnToggleValueChanged(GetComponent<Toggle>().isOn);
        }

        public void OnToggleValueChanged(bool check)
        {
            _upGameObject.SetActive(check);
            _downGameObject.SetActive(!check);
        }
    }
}

