using System.Collections.Generic;
using UnityEngine;

public class Storage : MonoBehaviour
{
    public delegate void StorageEventDelegate();
    public event StorageEventDelegate Opened;
    public event StorageEventDelegate Closed;
    public event StorageEventDelegate Changed;

    [SerializeField] private int m_TotalSlotCount = 81;
    [SerializeField] private int m_UnlockedSlotCount = 36;

    public int TotalSlotCount => m_TotalSlotCount;
    public int UnlockedSlotCount => m_UnlockedSlotCount;

    private Dictionary<int, HashSet<int>> m_TypeStackMap;
    public List<ItemStack> Stacks => m_Stacks;
    private List<ItemStack> m_Stacks;

    private IdRecyclingSparseSet m_SlotIds;

    public bool IsOpen => m_IsOpen;
    private bool m_IsOpen = false;

    private void Awake()
    {
        m_SlotIds = new IdRecyclingSparseSet(m_UnlockedSlotCount);
        m_Stacks = new List<ItemStack>(m_TotalSlotCount);
        for (int i = 0; i < m_TotalSlotCount; i++)
        {
            m_Stacks.Add(new ItemStack { TypeId = 0, Count = 0});
        }
        m_TypeStackMap = new Dictionary<int, HashSet<int>>();
        m_TypeStackMap[0] = new HashSet<int>();
        for (int i = 0; i < m_TotalSlotCount; i++)
        {
            m_TypeStackMap[0].Add(i);
        }
    }

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
        Opened?.Invoke();
    }

    public void Close()
    {
        m_IsOpen = false;
        Closed?.Invoke();
    }

    public int AddItems(int typeId, int count)
    {
        int maxStackCount = ItemDatabase.Instance.GetItemData(typeId).MaxStackCount;
        int remainingCount = count;
        FillStacksOfType(typeId, maxStackCount, ref remainingCount);
        FillEmptySlotsWithType(typeId, maxStackCount, ref remainingCount);

        if (remainingCount != count) Changed?.Invoke();

        return remainingCount;
    }

    private void FillStacksOfType(int typeId, int maxStackCount, ref int remainingCount)
    {
        if (!m_TypeStackMap.ContainsKey(typeId)) return;
        HashSet<int> indices = m_TypeStackMap[typeId];
        foreach (var i in indices)
        {
            FillSlotWithType(i, typeId, maxStackCount, ref remainingCount);
            if (remainingCount <= 0) break;
        }
    }

    private void FillEmptySlotsWithType(int typeId, int maxStackCount, ref int remainingCount)
    {
        if (!m_TypeStackMap.ContainsKey(typeId))
        {
            m_TypeStackMap[typeId] = new HashSet<int>();
        }
        HashSet<int> indices = m_TypeStackMap[typeId];
        while (remainingCount > 0)
        {
            int slotId = m_SlotIds.GetNextFreeId();
            if (slotId == -1)
                return;

            indices.Add(slotId);
            FillSlotWithType(slotId, typeId, maxStackCount, ref remainingCount);
        }
    }

    private void FillSlotWithType(int slotId, int typeId, int maxStackCount, ref int remainingCount)
    {
        ItemStack stack = m_Stacks[slotId];
        int diff = maxStackCount - stack.Count;
        int increment = remainingCount < diff ? remainingCount : diff;
        remainingCount -= increment;
        stack.TypeId = typeId;
        stack.Count += increment;
        m_Stacks[slotId] = stack;
    }

    public void RemoveFromSlot(int slotId, int count)
    {
        if (count < 1) return;

        ItemStack stack = m_Stacks[slotId];
        stack.Count -= count;
        m_Stacks[slotId] = stack;
        if (stack.Count <= 0) EmptySlot(slotId);

        Changed?.Invoke();
    }

    private void EmptySlot(int slotId)
    {
        m_SlotIds.FreeId(slotId);
        ItemStack stack = m_Stacks[slotId];
        m_TypeStackMap[stack.TypeId].Remove(slotId);
        stack.TypeId = 0;
        m_Stacks[slotId] = stack;
    }
}

public class IdRecyclingSparseSet
{
    public IdRecyclingSparseSet(int capacity = 0)
    {
        m_Dense = new List<int>(capacity);
        m_Sparse = new List<int>(capacity);
        m_Capacity = capacity;
        m_OccupiedCount = 0;

        for (int i = 0; i < m_Capacity; i++)
        {
            m_Dense.Add(i);
            m_Sparse.Add(i);
        }
    }

    public int GetNextFreeId()
    {
        if (m_OccupiedCount >= m_Dense.Count) m_Dense.Add(m_Dense.Count);
        if (m_OccupiedCount >= m_Capacity) return -1;
        int id =  m_Dense[m_OccupiedCount];
        m_Sparse[id] = m_OccupiedCount;
        m_OccupiedCount++;
        return id;
    }

    public void FreeId(int id)
    {
        if (id >= m_Capacity) return;
        m_OccupiedCount--;
        int lastOccupiedId = m_Dense[m_OccupiedCount];
        m_Dense[m_OccupiedCount] = id;
        int indexIntoDense = m_Sparse[id];
        m_Dense[indexIntoDense] = lastOccupiedId;
        m_Sparse[lastOccupiedId] = indexIntoDense;
    }

    private List<int> m_Dense;
    private List<int> m_Sparse;
    private int m_Capacity;
    private int m_OccupiedCount;
}

[System.Serializable]
public struct ItemStack
{
    public int TypeId;
    public int Count;
}
