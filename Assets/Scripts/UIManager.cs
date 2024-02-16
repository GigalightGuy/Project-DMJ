using System;
using UnityEngine;

public class UIManager : SingletonBase<UIManager>
{
    [SerializeField] private CountPopupWindow m_CountPopupWindow;
    [SerializeField] private CheckConditionPopupWindow m_CheckConditionPopupWindow;

    public void ShowCountPopupWindow(string title, int min, int max, Action<int> callback)
    {
        m_CountPopupWindow.Show(title, min, max, callback);
    }

    public void ShowCheckConditionPopupWindow(string title, int possessed, int required, Action callback)
    {
        m_CheckConditionPopupWindow.Show(title, possessed, required, callback);
    }
}
