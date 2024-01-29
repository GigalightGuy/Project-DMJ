using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private Storage m_AttachedStorage;
    [SerializeField] private Transform m_SlotsParent;

    [SerializeField] private GameObject m_UnlockedSlotPrefab;
    [SerializeField] private GameObject m_LockedSlotPrefab;

    private Canvas m_InventoryCanvas;

    private void Start()
    {
        m_InventoryCanvas = GetComponent<Canvas>();
        if (m_InventoryCanvas.enabled)
            m_InventoryCanvas.enabled = false;

        m_AttachedStorage.StorageOpened += OnStorageOpened;
        m_AttachedStorage.StorageClosed += OnStorageClosed;

        int totalSlotCount = m_AttachedStorage.TotalSlotCount;
        int unlockedSlotCount = m_AttachedStorage.UnlockedSlotCount;
        for (int i = 0; i < unlockedSlotCount; i++)
        {
            Instantiate(m_UnlockedSlotPrefab, m_SlotsParent);
        }

        int lockedSlotCount = totalSlotCount - unlockedSlotCount;
        for (int i = 0; i < lockedSlotCount; i++)
        {
            Instantiate(m_LockedSlotPrefab, m_SlotsParent);
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
