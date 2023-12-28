using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabsWindow : Window
{
    public GameObject[] Tabs;
    public void OpenTab(GameObject tab)
    {
        for (int i=0; i<Tabs.Length; i++)
        {
            if (Tabs[i] == tab)
            {
                OpenTab(i);
                return;
            }
        }
    }
    void OpenTab(int val)
    {
        CloseAllTabs();
        if (val >=0 && val < Tabs.Length)
            Tabs[val].SetActive(true);
    }
    public void CloseAllTabs()
    {
        foreach (GameObject tab in Tabs)
        {
            tab.SetActive(false);
        }
    }
    public override void OnDisable()
    {
        OpenTab(0);
        base.OnDisable();
    }
}
