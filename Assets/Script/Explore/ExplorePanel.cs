using UnityEngine;
using System.Collections;

namespace MiniWeChat
{
    public class ExplorePanel : MonoBehaviour
    {

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void Show(bool isShow)
        {
            gameObject.SetActive(isShow);
        }
    }
}

