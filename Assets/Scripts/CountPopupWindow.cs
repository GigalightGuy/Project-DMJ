using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CountPopupWindow : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_Title;
    [SerializeField] private Slider m_CountSlider;
    [SerializeField] private Button m_ConfirmButton;
    [SerializeField] private Button m_CancelButton;

    private Canvas m_Canvas;

    private Action<int> m_Callback;

    private void Awake()
    {
        m_CountSlider.wholeNumbers = true;

        m_ConfirmButton.onClick.AddListener(OnConfirmed);
        m_CancelButton.onClick.AddListener(OnCancel);

        m_Canvas = GetComponent<Canvas>();

        m_Canvas.enabled = false;
    }

    public void Show(string title, int min, int max, Action<int> callback)
    {
        m_Title.text = title;
        m_CountSlider.minValue = min;
        m_CountSlider.maxValue = max;
        m_CountSlider.value = 1;
        m_Callback = callback;

        m_Canvas.enabled = true;
    }

    private void OnConfirmed()
    {
        m_Callback?.Invoke((int)m_CountSlider.value);
        m_Canvas.enabled = false;
    }

    private void OnCancel()
    {
        m_Canvas.enabled = false;
    }
}
