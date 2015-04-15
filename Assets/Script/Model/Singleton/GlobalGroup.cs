using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace MiniWeChat
{
    public class GlobalGroup : Singleton<GlobalGroup>
    {
#region LifeCycle
        public override void Init()
        {
            base.Init();
        }

        public override void Release()
        {
            base.Release();
        }
#endregion

        #region LocalData


        #endregion
    }
}

