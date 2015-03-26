using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using protocol;

namespace MiniWeChat
{
    public class ContactsPanel : BaseWidget
    {
        private const int CONTACT_FRAME_HEIGHT = 150;

        public VerticalLayoutGroup _contactsGrid;

        private List<ContactFrame> _contactFrameList = new List<ContactFrame>();

        public override void Show(object param = null)
        {
            base.Show(param);
            MessageDispatcher.GetInstance().RegisterMessageHandler((uint)EUIMessage.UPDATE_FRIEND_DETAIL, OnUpdateFriendDetail);
            RefreshContactsFrames();
        }

        public override void Hide()
        {
            base.Hide();
            MessageDispatcher.GetInstance().UnRegisterMessageHandler((uint)EUIMessage.UPDATE_FRIEND_DETAIL, OnUpdateFriendDetail);
        }

        public void OnUpdateFriendDetail(uint iMessageType, object kParam)
        {
            RefreshContactsFrames();
        }

        public void RefreshContactsFrames()
        {
            _contactsGrid.GetComponent<RectTransform>().sizeDelta = new Vector2(GlobalVars.DEFAULT_SCREEN_WIDTH, CONTACT_FRAME_HEIGHT * GlobalContacts.GetInstance().Count);

            for (int i = 0; i < GlobalContacts.GetInstance().Count; i++)
            {
                if (i >= _contactFrameList.Count)
                {
                    GameObject go = UIManager.GetInstance().AddChild(_contactsGrid.gameObject, EUIType.ContactFrame);
                    _contactFrameList.Add(go.GetComponent<ContactFrame>());
                }
                _contactFrameList[i].Show(GlobalContacts.GetInstance().GetUserItemByIndex(i));
            }

            if (_contactFrameList.Count >  GlobalContacts.GetInstance().Count)
            {
                for (int i = GlobalContacts.GetInstance().Count; i < _contactFrameList.Count; i++)
                {
                    GameObject.Destroy(_contactFrameList[i].gameObject);
                }
                _contactFrameList = _contactFrameList.GetRange(0, GlobalContacts.GetInstance().Count);
            }
        }
    }
}

