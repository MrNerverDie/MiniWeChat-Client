using UnityEngine;
using System.Collections;

namespace MiniWeChat
{
    #region 内部事件 Internal 1000 - 2000

    public enum EGeneralMessage : uint
    {
        SOCKET_CONNECTED = 1000,
        SOCKET_DISCONNECTED,
        REQ_TIMEOUT,
    }

    public enum EUIMessage : uint
    {
        UPDATE_FRIEND_DETAIL = 2000,
    }
    #endregion
}

