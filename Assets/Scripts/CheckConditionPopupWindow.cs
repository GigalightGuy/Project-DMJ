using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CheckConditionPopupWindow : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_Title;
    [SerializeField] private TextMeshProUGUI m_ConditionText;
    [SerializeField] private Button m_ConfirmButton;
    [SerializeField] private Button m_CancelButton;

    private Canvas m_Canvas;

    private int m_Possessed;
    private int m_Required;
    private Action m_Callback;

    private void Awake()
    {
        m_ConfirmButton.onClick.AddListener(OnConfirmed);
        m_CancelButton.onClick.AddListener(OnCancel);

        m_Canvas = GetComponent<Canvas>();

        m_Canvas.enabled = false;
    }

    public void Show(string title, int possessed, int required, Action callback)
    {
        m_Title.text = title;
        m_Possessed = possessed;
        m_Required = required;
        m_Callback = callback;

        m_ConditionText.text = possessed + "/" + required;
        m_ConditionText.color = possessed >= required ? Color.green : Color.red;

        m_Canvas.enabled = true;
    }

    private void OnConfirmed()
    {
        if (m_Possessed >= m_Required)
        {
            m_Callback?.Invoke();
        }
        m_Canvas.enabled = false;
    }

    private void OnCancel()
    {
        m_Canvas.enabled = false;
    }
}
