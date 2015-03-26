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

        public List<BaseWidget> _panelList;
        public List<Toggle> _toggleList;

        private int _curIndex;

        public void Start()
        {
            foreach (var toggle in _toggleList)
            {
                toggle.onValueChanged.AddListener(SwitchToTab);
            }
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

        public void SwitchToTab(int index)
        {
            _toggleList[index].Select();
            _curIndex = index;

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

        public BaseWidget GetCurPanel()
        {
            return _panelList[_curIndex];
        }

    }
}


