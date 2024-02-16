using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmPopupWindow : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_Title;
    [SerializeField] private TextMeshProUGUI m_Body;
    [SerializeField] private Button m_ConfirmButton;
    [SerializeField] private Button m_CancelButton;

    private Action m_Callback;

    private void Awake()
    {
        m_ConfirmButton.onClick.AddListener(OnConfirmed);
        m_CancelButton.onClick.AddListener(OnCancel);
    }

    public void Show(string title, string body, int min, int max, Action callback)
    {
        m_Title.text = title;
        m_Body.text = body;
        m_Callback = callback;

        gameObject.SetActive(true);
    }

    private void OnConfirmed()
    {
        m_Callback?.Invoke();
        gameObject.SetActive(false);
    }

    private void OnCancel()
    {
        gameObject.SetActive(false);
    }
}