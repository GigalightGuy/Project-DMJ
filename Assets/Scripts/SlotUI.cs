using UnityEngine;
using UnityEngine.UI;

public class SlotUI : MonoBehaviour
{
    [SerializeField] private Color m_UnlockedColor = Color.white;
    [SerializeField] private Color m_LockedColor = Color.grey;

    private Image m_BackgroundImage;

    private bool m_Locked = true;

    public bool Locked
    {
        get { return m_Locked; }
        set
        {
            if (m_Locked != value)
            {
                m_Locked = value;
                m_BackgroundImage.color = m_Locked ? m_LockedColor : m_UnlockedColor;
            }
        }
    }

    private void Awake()
    {
        m_BackgroundImage = GetComponent<Image>();
        m_BackgroundImage.color = m_LockedColor;
    }
}
