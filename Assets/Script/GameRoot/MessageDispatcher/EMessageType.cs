using UnityEngine;
using System.Collections;

namespace MiniWeChat
{
    #region 界面事件 EUIMessage 0 - 999
    public enum EUIMessage : uint
    {
        UITest0 = 0,
        UITest1 = 1,
        UITest2 = 2
    }
    #endregion

    #region 战斗事件 EBattleMessage 1000 - 1999
    public enum EBattleMessage : uint
    {
        BattleTest = 1000,
    }
    #endregion

    #region 网络事件 ENetMessage 2000 - 2999
    public enum ENetMessage : uint
    {
        NetTest = 2000,
    }
    #endregion
}

