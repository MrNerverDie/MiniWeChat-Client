using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace MiniWeChat
{
    public enum EUIType
    {
        MainMenuPanel = 0,
        ChatFrame,
        ContactFrame,
        FriendChatBubbleFrame,
        PersonalChatBubbleFrame,
        ChatPanel,
        BackButton,
        RegisterPanel,
        LoginPanel,
        SingleButtonDialog,
        DoubleButtonInputDialog,
        ImageListPanel,
        SearchFriendPanel,
        FriendDetailPanel,
        InputDebug,
        StatusLabel,
        LoadingDialog,
        CreateGroupPanel,
        GroupDetailPanel,
        GroupMemberFrame,
        GroupMemberHeadFrame,
        GroupMemberHeadIcon,
        SelectGroupPanel,
        GroupChatFrame,
        GroupChatPanel,
        GroupFrame,
        DoubleButtonDialog,
    }

    public enum EAtlasName
    {
        Chat = 0,
        Common,
        Head,
        MainMenu,
        Single,
    }
}

