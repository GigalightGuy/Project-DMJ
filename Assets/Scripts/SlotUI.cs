using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SlotUI : MonoBehaviour, IContext
{
    [SerializeField] private Color m_UnlockedColor = Color.white;
    [SerializeField] private Color m_LockedColor = Color.grey;

    private Image m_BackgroundImage;

    [SerializeField] private Image m_ItemIconImage;
    [SerializeField] private TextMeshProUGUI m_CountDisplay;

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
                m_ItemIconImage.enabled = !value;
            }
        }
    }

    public Sprite ItemIcon 
    { 
        set 
        { 
            m_ItemIconImage.sprite = value;
            m_ItemIconImage.color = value ? new Color(1f, 1f, 1f, 1f) : new Color(1f, 1f, 1f, 0f);
        } 
    }
    public int ItemCount { set { m_CountDisplay.text = value > 1 ? value.ToString() : ""; } }

    private ContextOptionConfiguration[] m_TestContextOptions;
    private ContextOptionConfiguration[] m_LockedSlotContextOptions;

    public ContextOptionConfiguration[] GetOptionConfigurations()
    {
        if (m_Locked)
        {
            return m_LockedSlotContextOptions;
        }
        else
        {
            return m_TestContextOptions;
        }
    }

    private void Awake()
    {
        m_TestContextOptions = new ContextOptionConfiguration[3];

        m_TestContextOptions[0].Text = "Print TEHE";
        m_TestContextOptions[0].FontColor = Color.white;
        m_TestContextOptions[0].ClickCallback = () => { Debug.Log("TE HE!!!"); };

        m_TestContextOptions[1].Text = "Say hello";
        m_TestContextOptions[1].FontColor = Color.green;
        m_TestContextOptions[1].ClickCallback = () => { Debug.Log("Hello World!"); };

        m_TestContextOptions[2].Text = "Sell";
        m_TestContextOptions[2].FontColor = Color.red;
        m_TestContextOptions[2].ClickCallback = () => { Debug.Log("Sold items!"); };

        m_LockedSlotContextOptions = new ContextOptionConfiguration[1];
        m_LockedSlotContextOptions[0].Text = "Unlock";
        m_LockedSlotContextOptions[0].FontColor = Color.yellow;
        m_LockedSlotContextOptions[0].ClickCallback = () => { Debug.Log("Unlocked slot!"); };

        m_BackgroundImage = GetComponent<Image>();
        m_BackgroundImage.color = m_LockedColor;
    }
}
