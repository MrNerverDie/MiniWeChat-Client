using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

using protocol;

// ChangeGroup To Do //
namespace MiniWeChat
{
    public class CreateGroupPanel : BaseState
    {
        // Const //
        private const float GROUP_MEMBER_FRAME_HEIGHT = 150f;
        private const float GROUP_MEMBER_ICON_WIDTH = 150f;

        public VerticalLayoutGroup _gridSelectMember;
        public HorizontalLayoutGroup _gridMember;

        public Button _buttonConfirm;
        public Button _buttonSelectGroup;

        private GroupItem _groupItem;
        private HashSet<string> _selectUserIdSet = new HashSet<string>();

        public override void OnEnter(object param = null)
        {
            base.OnEnter(param);
            UIManager.GetInstance().AddChild(transform.Find("TopBar").gameObject, EUIType.BackButton);

            _groupItem = param as GroupItem;

            foreach (var friendItem in GlobalContacts.GetInstance())
            {
                GameObject go = UIManager.GetInstance().AddChild(_gridSelectMember.gameObject, EUIType.GroupMemberFrame);
                go.GetComponent<GroupMemberFrame>().Show(friendItem);
            }
            _gridSelectMember.GetComponent<RectTransform>().sizeDelta= new Vector2(
                GlobalVars.DEFAULT_SCREEN_WIDTH,
                GlobalContacts.GetInstance().Count * GROUP_MEMBER_FRAME_HEIGHT);

            _buttonConfirm.onClick.AddListener(OnClickConfirmButton);
        }

        public override void OnExit()
        {
            base.OnExit();
            UIManager.GetInstance().DestroySingleUI(EUIType.CreateGroupPanel);
        }

        public override void OnShow(object param = null)
        {
            base.OnShow(param);
            MessageDispatcher.GetInstance().RegisterMessageHandler((uint)EUIMessage.TOGGLE_GROUP_MEMBER, OnToggleGroupMember);
            MessageDispatcher.GetInstance().RegisterMessageHandler((uint)ENetworkMessage.CREATE_GROUP_CHAT_RSP, OnCreateGroupRsp);
        }

        public override void OnHide()
        {
            base.OnHide();
            MessageDispatcher.GetInstance().UnRegisterMessageHandler((uint)EUIMessage.TOGGLE_GROUP_MEMBER, OnToggleGroupMember);
            MessageDispatcher.GetInstance().UnRegisterMessageHandler((uint)ENetworkMessage.CREATE_GROUP_CHAT_RSP, OnCreateGroupRsp);
        }

        #region EventListener

        public void OnClickConfirmButton()
        {
            CreateGroupChatReq req = new CreateGroupChatReq();
            foreach (var item in _selectUserIdSet)
            {
                req.userId.Add(item);
            }

            NetworkManager.GetInstance().SendPacket<CreateGroupChatReq>(ENetworkMessage.CREATE_GROUP_CHAT_REQ, req);
        }

        #endregion

        #region MessageHandler

        public void OnToggleGroupMember(uint iMessageType, object kParam)
        {
            ToggleGroupMemberParam param = kParam as ToggleGroupMemberParam;
            if (param.check)
            {
                _selectUserIdSet.Add(param.userID);
            }else
	        {
                _selectUserIdSet.Remove(param.userID);
	        }
            RefreshGroupMember();
        }

        public void OnCreateGroupRsp(uint iMessageType, object kParam)
        {
            CreateGroupChatRsp rsp = kParam as CreateGroupChatRsp;
            if (rsp.resultCode == CreateGroupChatRsp.ResultCode.SUCCESS)
            {
                StateManager.GetInstance().PopState();
            }
        }

        #endregion


        #region Misc

        private void RefreshGroupMember()
        {
            UIManager.GetInstance().RefreshChildren(_gridMember.gameObject,
                EUIType.GroupMemberHeadIcon,
                _selectUserIdSet.Count,
                GROUP_MEMBER_ICON_WIDTH, false); ;

            List<string> userIdList = new List<string>(_selectUserIdSet);
            for (int i = 0; i < userIdList.Count; i++)
            {
                _gridMember.transform.GetChild(i).GetComponent<GroupMemberFrame>().
                    Show(GlobalContacts.GetInstance().GetUserItemById(userIdList[i]));
            }
        }

        #endregion
    }
}

