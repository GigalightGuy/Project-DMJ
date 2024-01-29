using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ContextMenu : MonoBehaviour
{
    private readonly Dictionary<string, Button> m_OptionButtons = new();

    private void Start()
    {
        m_OptionButtons["Equip"] = transform.Find("Equip").GetComponent<Button>();
        m_OptionButtons["Drop"] = transform.Find("Drop").GetComponent<Button>();
        m_OptionButtons["Sell"] = transform.Find("Sell").GetComponent<Button>();
        m_OptionButtons["Destroy"] = transform.Find("Destroy").GetComponent<Button>();
    }

    private void Show(Vector2 position, IContext context)
    {
        if (context is IEquipable equipable)
        {
            m_OptionButtons["Equip"].onClick.AddListener(() => equipable.Equip());
        }
        if (context is IDroppable droppable)
        {
            m_OptionButtons["Drop"].onClick.AddListener(() => droppable.Drop());
        }
        if (context is ISellable sellable)
        {
            m_OptionButtons["Sell"].onClick.AddListener(() => sellable.Sell());
        }
        if (context is IDestructible destructible)
        {
            m_OptionButtons["Destroy"].onClick.AddListener(() => destructible.Destroy());
        }
    }

    private void Hide()
    {
        m_OptionButtons["Equip"]?.onClick.RemoveAllListeners();
        m_OptionButtons["Drop"]?.onClick.RemoveAllListeners();
        m_OptionButtons["Sell"]?.onClick.RemoveAllListeners();
        m_OptionButtons["Destroy"]?.onClick.RemoveAllListeners();
    }
}

public interface IContext { }

public interface IEquipable
{
    public void Equip();
}

public interface IDroppable
{
    public void Drop();
}

public interface ISellable
{
    public void Sell();
}

public interface IDestructible
{
    public void Destroy();
}
