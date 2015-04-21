using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

using protocol;

namespace MiniWeChat
{
    public class GroupDetailPanel : BaseState
    {
        public Button _buttonSetGroupNmae;
        public Button _buttonAddGroupMember;
        public Button _buttonExitGroup;

        public GridLayoutGroup _gridMemberHead;
        private const int MEMBER_ONE_ROW = 4;

        public RectTransform _gridGroupDetail;
        private const float GRID_GROUP_DETAIL_BASE = 1000f;

        public LayoutElement _groupInfoBar;
        private const float GROUP_INFO_BAR_BASE = 100f;
        private const float GROUP_INFO_BAR_INC = 350f;

        private GroupItem _groupItem;

        public override void OnEnter(object param)
        {
            base.OnEnter(param);
            UIManager.GetInstance().AddChild(transform.Find("TopBar").gameObject, EUIType.BackButton);

            _groupItem = param as GroupItem;

            InitMemberHeadFrames();

            InitButtons();
        }

        public override void OnExit()
        {
            base.OnExit();
            UIManager.GetInstance().DestroySingleUI(EUIType.GroupDetailPanel);
        }

        public override void OnShow(object param = null)
        {
            base.OnShow(param);
        }

        public override void OnHide()
        {
            base.OnHide();
        }

        private void InitMemberHeadFrames()
        {
            int memberRow = _groupItem.memberUserId.Count / MEMBER_ONE_ROW + 1;

            _gridGroupDetail.sizeDelta = new Vector2(
                GlobalVars.DEFAULT_SCREEN_WIDTH, GRID_GROUP_DETAIL_BASE + memberRow * GROUP_INFO_BAR_INC);
            _groupInfoBar.preferredHeight = GROUP_INFO_BAR_BASE + memberRow * GROUP_INFO_BAR_INC;

            foreach (var userID in _groupItem.memberUserId)
            {
                UserItem userItem = GlobalGroup.GetInstance().GetGroupMember(userID);
                if (userItem == null)
                {
                    userItem = new UserItem();
                }

                UIManager.GetInstance().AddChild(_gridMemberHead.gameObject, EUIType.GroupMemberHeadFrame);
            }
        }

        private void InitButtons()
        {
            _buttonAddGroupMember.onClick.AddListener(OnClickAddGroupMember);
            _buttonExitGroup.onClick.AddListener(OnClickExitGroup);
            _buttonSetGroupNmae.onClick.AddListener(OnClickSetGroupName);
        }

        #region EventListener

        public void OnClickSetGroupName()
        {
            DialogManager.GetInstance().CreateDoubleButtonInputDialog();
        }

        public void OnClickAddGroupMember()
        {
            StateManager.GetInstance().PushState<CreateGroupPanel>(EUIType.CreateGroupPanel, _groupItem);
        }

        public void OnClickExitGroup()
        {
            ChangeGroupReq req = new ChangeGroupReq
            {
                changeType = ChangeGroupReq.ChangeType.DELETE,
                groupId = _groupItem.groupId,
            };

            req.userId.Add(GlobalUser.GetInstance().UserId);

            NetworkManager.GetInstance().SendPacket<ChangeGroupReq>(ENetworkMessage.CHANGE_GROUP_REQ, req);
        }

        #endregion
    }
}

