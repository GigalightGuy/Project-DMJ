using System;
using UnityEngine;

public class UIManager : SingletonBase<UIManager>
{
    [SerializeField] private CountPopupWindow m_CountPopupWindow;

    public void ShowCountPopupWindow(string title, int min, int max, Action<int> callback)
    {
        m_CountPopupWindow.Show(title, min, max, callback);
    }
}
