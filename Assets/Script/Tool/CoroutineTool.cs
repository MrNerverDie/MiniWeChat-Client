using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;
using System.Collections.Generic;

namespace MiniWeChat
{
    public class CoroutineTool : MonoBehaviour
    {
		public static IEnumerator ActionNextFrame(Action action)
        {
            yield return null;
            action();
        }
    }
}

