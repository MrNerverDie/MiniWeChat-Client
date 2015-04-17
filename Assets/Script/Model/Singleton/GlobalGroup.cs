using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

using protocol;

namespace MiniWeChat
{
    public class GlobalGroup : Singleton<GlobalGroup>
    {
        private Dictionary<string, GroupItem> _groupDict = new Dictionary<string,GroupItem>();

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

        private string GetContactsDirPath()
        {
            return GlobalUser.GetInstance().GetUserDir() + "/Group";
        }

        #endregion
    }
}

