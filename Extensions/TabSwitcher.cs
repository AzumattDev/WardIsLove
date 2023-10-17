using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace WardIsLove.Extensions
{
    public class TabSwitcher : MonoBehaviour
    {
        public bool m_gamepadInput;


        public List<Tab> m_tabs = new();


        private int m_selected;


        private void Awake()
        {
            int activeTab = 0;
            for (int i = 0; i < m_tabs.Count; i++)
            {
                Tab tab = m_tabs[i];
                tab.m_button.onClick.AddListener(delegate { OnClick(tab.m_button); });
                Transform transform = tab.m_button.gameObject.transform.Find("Selected");
                if (transform)
                {
                    //transform.GetComponentInChildren<Text>().text = tab.m_button.GetComponentInChildren<Text>().text;
                }

                if (tab.m_default) activeTab = i;
            }

            SetActiveTab(activeTab);
        }


        private void OnClick(Button button)
        {
            SetActiveTab(button);
        }


        private void SetActiveTab(Button button)
        {
            for (int i = 0; i < m_tabs.Count; i++)
                if (m_tabs[i].m_button == button)
                {
                    SetActiveTab(i);
                    return;
                }
        }


        public void SetActiveTab(int index)
        {
            m_selected = index;
            for (int i = 0; i < m_tabs.Count; i++)
            {
                Tab tab = m_tabs[i];
                bool flag = i == index;
                tab.m_page.gameObject.SetActive(flag);
                tab.m_button.interactable = !flag;
                Transform transform = tab.m_button.gameObject.transform.Find("Selected");
                if (transform) transform.gameObject.SetActive(flag);
                if (flag) tab.m_onClick.Invoke();
            }
        }


        [Serializable]
        public class Tab
        {
            public Button m_button = null!;


            public RectTransform m_page = null!;


            public bool m_default;


            public UnityEvent m_onClick = null!;
        }
    }
}