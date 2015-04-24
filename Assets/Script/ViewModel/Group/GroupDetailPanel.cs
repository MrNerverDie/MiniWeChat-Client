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
            int memberRow = Mathf.CeilToInt((float)_groupItem.memberUserId.Count / (float)MEMBER_ONE_ROW);

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

                GameObject go = UIManager.GetInstance().AddChild(_gridMemberHead.gameObject, EUIType.GroupMemberHeadFrame);
                go.GetComponent<GroupMemberFrame>().Show(userItem);
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
            DialogManager.GetInstance().CreateDoubleButtonInputDialog(
                title : "修改群名",
                inputHint : "群名",
                inputPlaceHolder : _groupItem.groupName,
                confirmCallback: OnConfirmSetGroupName);
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

        public void OnConfirmSetGroupName(string name)
        {
            ChangeGroupReq req = new ChangeGroupReq
            {
                changeType = ChangeGroupReq.ChangeType.UPDATE_INFO,
                groupId = _groupItem.groupId,
                groupName = name,
            };

            NetworkManager.GetInstance().SendPacket<ChangeGroupReq>(ENetworkMessage.CHANGE_GROUP_REQ, req);
        }

        #endregion
    }
}

