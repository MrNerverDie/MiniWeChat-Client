using UnityEngine;
using System.Collections;

namespace MiniWeChat
{
    #region 主要事件 General 1000 - 2000

    public enum EModelMessage : uint
    {
        SOCKET_CONNECTED = 1000,
        SOCKET_DISCONNECTED,
        TRY_LOGIN,
        REQ_FINISH,
        REQ_TIMEOUT,
        SEND_CHAT_TIMEOUT,
        UPLOAD_FINISH,
        DOWNLOAD_FINISH,
    }

    #endregion

    #region 界面事件 General 1000 - 2000

    public enum EUIMessage : uint
    {
        UPDATE_FRIEND_DETAIL = 2000,
        UPDATE_SEND_CHAT,
        UPDATE_RECEIVE_CHAT,
        UPDATE_PERSONAL_DETAIL,
        UPDATE_CHAT_LIST,
    }

    #endregion
}

