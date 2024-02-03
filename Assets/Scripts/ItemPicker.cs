using UnityEngine;

public class ItemPicker : MonoBehaviour
{
    private Storage m_Storage;

    private void Start()
    {
        m_Storage = GetComponent<Storage>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<ItemPickUp>(out var itemPickUp))
        {
            ItemStack stack = itemPickUp.Stack;
            int remaining = m_Storage.AddItems(stack.TypeId, stack.Count);
            itemPickUp.RemoveAmount(stack.Count - remaining);
        }
    }
}
