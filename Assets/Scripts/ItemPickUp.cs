using UnityEngine;

public class ItemPickUp : MonoBehaviour
{
    public ItemStack Stack => m_Stack;
    [SerializeField] private ItemStack m_Stack;

    public void RemoveAmount(int amount)
    {
        m_Stack.Count -= amount;
        if (m_Stack.Count <= 0) Destroy(gameObject);
    }
}
