using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace MiniWeChat
{

    public class MainMenuNaviBar : MonoBehaviour
    {
        public enum MainMenuTab
        {
            CHAT_LIST = 0,
            CONTACTS,
            EXPLORE,
            PERSONAL,
        }

        private const int TAB_NUM = 4;

        public List<BasePanel> _panelList;
        public List<Toggle> _toggleList;

        public void Start()
        {
            foreach (var toggle in _toggleList)
            {
                toggle.onValueChanged.AddListener(SwitchToTab);
            }

            SwitchToTab((int)MainMenuTab.CHAT_LIST);
        }

        public void SwitchToTab(bool check)
        {
            if (check)
            {
                foreach (var toggle in GetComponent<ToggleGroup>().ActiveToggles())
                {
                    int index = _toggleList.IndexOf(toggle);
                    SwitchToTab(index);
                }
            }
        }

        private void SwitchToTab(int index)
        {
            _toggleList[index].Select();

            for (int i = 0; i < TAB_NUM; i++)
            {
                if (i == index)
                {
                    _panelList[i].Show();
                }
                else
                {
                    _panelList[i].Hide();
                }
            }
            
        }

    }
}


