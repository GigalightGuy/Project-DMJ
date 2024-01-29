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
            if (m_AttachedStorage)
            {
                
            }
            m_AttachedStorage = value; 
        }
    }

    private Canvas m_InventoryCanvas;

    private void Start()
    {
        m_InventoryCanvas = GetComponent<Canvas>();
        if (m_InventoryCanvas.enabled)
            m_InventoryCanvas.enabled = false;

        m_AttachedStorage.StorageOpened += OnStorageOpened;
        m_AttachedStorage.StorageClosed += OnStorageClosed;

        FillSlotsPool();

        int totalSlotCount = m_AttachedStorage.TotalSlotCount;
        int unlockedSlotCount = m_AttachedStorage.UnlockedSlotCount;
        int i = 0;
        for (; i < unlockedSlotCount; i++)
        {
            m_SlotPool[i].Locked = false;
        }
        for (; i < totalSlotCount; i++)
        {
            m_SlotPool[i].Locked = true;
        }
        
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
            m_SlotPool.Add(Instantiate(m_SlotPrefab, m_SlotsParent).GetComponent<SlotUI>());
        }
    }

    private void OnStorageOpened()
    {
        m_InventoryCanvas.enabled = true;
    }

    private void OnStorageClosed()
    {
        m_InventoryCanvas.enabled = false;
    }
}
