using UnityEngine;

public class Storage : MonoBehaviour
{
    public delegate void StorageOpenedClosedDelegate();
    public event StorageOpenedClosedDelegate StorageOpened;
    public event StorageOpenedClosedDelegate StorageClosed;

    [SerializeField] private int m_TotalSlotCount = 81;
    [SerializeField] private int m_UnlockedSlotCount = 36;

    public int TotalSlotCount => m_TotalSlotCount;
    public int UnlockedSlotCount => m_UnlockedSlotCount;

    private bool m_IsOpen = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (!m_IsOpen) Open();
            else Close();
        }
    }

    public void Open()
    {
        m_IsOpen = true;
        StorageOpened?.Invoke();
    }

    public void Close()
    {
        m_IsOpen = false;
        StorageClosed?.Invoke();
    }
}
