using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace MiniWeChat
{
    public class BasePanel : MonoBehaviour
    {
        /// <summary>
        /// 控制Panel出现
        /// </summary>
        /// <param name="param">附加参数</param>
        public virtual void Show(object param = null)
        {
            gameObject.SetActive(true);
        }

        /// <summary>
        /// 控制Panel隐藏
        /// </summary>
        public virtual void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}

