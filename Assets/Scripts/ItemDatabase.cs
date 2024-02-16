using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : SingletonBase<ItemDatabase>
{
    [SerializeField] private List<ItemData> m_ItemsData;

    public ItemData GetItemData(int id) { return m_ItemsData[id]; }
    public int GetRegisteredItemsCount() => m_ItemsData.Count;
}

[System.Flags]
public enum ItemContextFlags
{
    None = 0,

    Use = 1,
    Sell = 2,
    Equip = 4,

}

[System.Serializable]
public struct ItemData
{
    public string Name;
    public string Description;
    public int SellPrice;
    public Sprite Icon;
    public int MaxStackCount;
    public ItemContextFlags ContextFlags;
}
