using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ContextOption : TextMeshProUGUI, IPointerClickHandler
{
    private Action m_ClickCallback;

    public void SetOption(in ContextOptionConfiguration config)
    {
        text = config.Text;
        color = config.FontColor;
        m_ClickCallback = config.ClickCallback;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            m_ClickCallback?.Invoke();
        }
    }
}

public struct ContextOptionConfiguration
{
    public string Text;
    public Color FontColor;
    public Action ClickCallback;
}
