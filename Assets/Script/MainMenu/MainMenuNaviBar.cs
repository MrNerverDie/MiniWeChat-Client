using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace MiniWeChat
{

    public class MainMenuNaviBar : MonoBehaviour
    {

        public ChatListPanel _chatListPanel;
        public ContactsPanel _contactsPanel;
        public Toggle _toggleChatList, _toggleContacts, _toggleExplore, togglePersonal;

        public void Start()
        {
            _toggleChatList.onValueChanged.AddListener(SwitchToChatList);
            _toggleContacts.onValueChanged.AddListener(SwitchToContacts);

            _toggleChatList.Select();
            SwitchToChatList(true);
            SwitchToContacts(false);
            SwitchToExplore(false);
            SwitchToPersonal(false);
        }

        public void SwitchToChatList(bool check)
        {
            _chatListPanel.Show(check);
        }

        public void SwitchToContacts(bool check)
        {
            _contactsPanel.Show(check);
        }

        public void SwitchToExplore(bool check)
        {

        }

        public void SwitchToPersonal(bool check)
        {

        }
    }
}


