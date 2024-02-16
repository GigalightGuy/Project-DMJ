using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private Storage m_AttachedStorage;
    [SerializeField] private Transform m_SlotsParent;

    [SerializeField] private GameObject m_SlotPrefab;

    [SerializeField] private int m_SlotPoolCapacity = 81;

    private List<SlotUI> m_SlotPool;

    public Storage AttachedStorage
    {
        get { return m_AttachedStorage; }
        set 
        {
            if (m_AttachedStorage == value) return;
            if (m_AttachedStorage)
            {
                StorageDettached();
            }
            m_AttachedStorage = value;
            if (m_AttachedStorage)
            {
                StorageAttached();
            }
        }
    }

    private bool m_IsDirty = false;

    private Canvas m_InventoryCanvas;

    private void Start()
    {
        m_InventoryCanvas = GetComponent<Canvas>();
        if (m_InventoryCanvas.enabled)
            m_InventoryCanvas.enabled = false;

        FillSlotsPool();

        if (!m_AttachedStorage) return;

        StorageAttached();
    }

    private void LateUpdate()
    {
        if (!m_IsDirty) return;

        int totalSlotCount = m_AttachedStorage.TotalSlotCount;
        int unlockedSlotCount = m_AttachedStorage.UnlockedSlotCount;
        int i = 0;
        for (; i < unlockedSlotCount; i++)
        {
            m_SlotPool[i].Locked = false;
            m_SlotPool[i].ItemIcon = ItemDatabase.Instance.GetItemData(m_AttachedStorage.Stacks[i].TypeId).Icon;
            m_SlotPool[i].ItemCount = m_AttachedStorage.Stacks[i].Count;
        }
        for (; i < totalSlotCount; i++)
        {
            m_SlotPool[i].Locked = true;
            m_SlotPool[i].ItemIcon = null;
            m_SlotPool[i].ItemCount = 0;
        }

        m_IsDirty = false;

    }

    public void CloseAttachedStorage()
    {
        m_AttachedStorage.Close();
    }

    private void FillSlotsPool()
    {
        m_SlotPool = new List<SlotUI>(m_SlotPoolCapacity);
        for (int i = 0; i < m_SlotPoolCapacity; i++)
        {
            var slot = Instantiate(m_SlotPrefab, m_SlotsParent).GetComponent<SlotUI>();
            slot.InventoryUI = this;
            slot.Id = i;
            m_SlotPool.Add(slot);
        }
    }

    private void StorageAttached()
    {
        m_AttachedStorage.Opened += OnStorageOpened;
        m_AttachedStorage.Closed += OnStorageClosed;
        m_AttachedStorage.Changed += OnStorageChanged;

        m_IsDirty = true;
    }

    private void StorageDettached()
    {
        m_AttachedStorage.Opened -= OnStorageOpened;
        m_AttachedStorage.Closed -= OnStorageClosed;
        m_AttachedStorage.Changed -= OnStorageChanged;

        m_IsDirty = false;
    }

    private void OnStorageOpened()
    {
        m_InventoryCanvas.enabled = true;
        GameManager.Instance.DisableControllers();
    }

    private void OnStorageClosed()
    {
        m_InventoryCanvas.enabled = false;
        GameManager.Instance.EnableControllers();
    }

    private void OnStorageChanged()
    {
        m_IsDirty = true;
    }
}
