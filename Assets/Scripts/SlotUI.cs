using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.Dependencies.Sqlite;
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

    private InventoryUI m_InventoryUI;
    public InventoryUI InventoryUI
    {
        get => m_InventoryUI;
        set => m_InventoryUI = value;
    }

    private int m_Id;
    public int Id
    {
        get => m_Id;
        set => m_Id = value;
    }

    private ContextOptionConfiguration[] m_UnlockedSlotContextOptions;
    private ContextOptionConfiguration[] m_LockedSlotContextOptions;

    public ContextOptionConfiguration[] GetOptionConfigurations()
    {
        if (m_Locked)
        {
            return m_LockedSlotContextOptions;
        }
        else
        {
            return FilterContextOptions();
        }
    }

    private void Awake()
    {
        m_UnlockedSlotContextOptions = new ContextOptionConfiguration[3];

        m_UnlockedSlotContextOptions[0].Text = "Use";
        m_UnlockedSlotContextOptions[0].FontColor = Color.white;
        m_UnlockedSlotContextOptions[0].ClickCallback = UseItems;

        m_UnlockedSlotContextOptions[1].Text = "Sell";
        m_UnlockedSlotContextOptions[1].FontColor = Color.yellow;
        m_UnlockedSlotContextOptions[1].ClickCallback = SellItems;

        m_UnlockedSlotContextOptions[2].Text = "Equip";
        m_UnlockedSlotContextOptions[2].FontColor = Color.yellow;
        m_UnlockedSlotContextOptions[2].ClickCallback = () => { Debug.Log("Item equipped!"); };


        m_LockedSlotContextOptions = new ContextOptionConfiguration[1];

        m_LockedSlotContextOptions[0].Text = "Unlock";
        m_LockedSlotContextOptions[0].FontColor = Color.yellow;
        m_LockedSlotContextOptions[0].ClickCallback = () => UnlockSlots();


        m_BackgroundImage = GetComponent<Image>();
        m_BackgroundImage.color = m_LockedColor;
    }

    private ContextOptionConfiguration[] FilterContextOptions()
    {
        ItemStack stack = m_InventoryUI.AttachedStorage.Stacks[m_Id];
        ItemContextFlags flags = ItemDatabase.Instance.GetItemData(stack.TypeId).ContextFlags;

        List<ContextOptionConfiguration> filteredOptions = new();
        int flagBit = 1;
        foreach (var option in m_UnlockedSlotContextOptions)
        {
            if (((int)flags & flagBit) == flagBit)
            {
                filteredOptions.Add(option);
            }
            flagBit <<= 1;
        }

        return filteredOptions.ToArray();
    }

    private void UseItems()
    {
        ItemStack stack = m_InventoryUI.AttachedStorage.Stacks[m_Id];
        UIManager.Instance.ShowCountPopupWindow("Use item(s)", 0, stack.Count, (int x) =>
        {
            m_InventoryUI.AttachedStorage.RemoveFromSlot(m_Id, x);
            int earnedMoney = x * ItemDatabase.Instance.GetItemData(stack.TypeId).SellPrice;
            ResourcesManager.Instance.Coins += earnedMoney;
        });
    }

    private void SellItems()
    {
        ItemStack stack = m_InventoryUI.AttachedStorage.Stacks[m_Id];
        UIManager.Instance.ShowCountPopupWindow("Sell item(s)", 0, stack.Count, (int x) =>
        {
            m_InventoryUI.AttachedStorage.RemoveFromSlot(m_Id, x);
            int earnedMoney = x * ItemDatabase.Instance.GetItemData(stack.TypeId).SellPrice;
            ResourcesManager.Instance.Coins += earnedMoney;
        });
    }

    private void UnlockSlots()
    {
        int unlockCount = m_Id - (m_InventoryUI.AttachedStorage.UnlockedSlotCount - 1);
        int cost = unlockCount * 50;
        UIManager.Instance.ShowCheckConditionPopupWindow("Unlock " + unlockCount + " slots?", ResourcesManager.Instance.Coins, cost, () =>
        {
            m_InventoryUI.AttachedStorage.UnlockSlots(unlockCount);
            ResourcesManager.Instance.Coins -= cost;
        });
    }
}
