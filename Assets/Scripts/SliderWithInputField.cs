using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SliderWithInputField : MonoBehaviour
{
    private Slider m_Slider;
    private TMP_InputField m_InputField;

    private void Awake()
    {
        m_Slider = GetComponentInChildren<Slider>();
        m_InputField = GetComponentInChildren<TMP_InputField>();

        m_Slider.onValueChanged.AddListener(OnSliderValueChanged);
        m_InputField.onValueChanged.AddListener(OnInputFieldValueChanged);
    }

    private void OnSliderValueChanged(float value)
    {
        int sliderValue = (int)value;
        int inputFieldValue;
        if (!int.TryParse(m_InputField.text, out inputFieldValue))
        {
            m_InputField.text = sliderValue.ToString();
            return;
        }
        if (inputFieldValue != sliderValue)
        {
            m_InputField.text = sliderValue.ToString();
        }
    }

    private void OnInputFieldValueChanged(string input)
    {
        int sliderValue = (int)m_Slider.value;
        int inputFieldValue;
        if (!int.TryParse(m_InputField.text, out inputFieldValue))
        {
            return;
        }
        if (inputFieldValue < m_Slider.minValue || inputFieldValue > m_Slider.maxValue)
        {
            return;
        }
        if (inputFieldValue != sliderValue)
        {
            m_Slider.value = inputFieldValue;
        }
    }
}
