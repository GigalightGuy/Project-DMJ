using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ContextMenu : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    public static ContextMenu Current => s_Current;
    private static ContextMenu s_Current;

    public bool IsActive => gameObject.activeInHierarchy;

    [SerializeField] private GameObject m_OptionPrefab;
    [SerializeField] private int m_OptionPoolCapacity = 6;
    [SerializeField] private Vector2 m_Offset = new Vector2(10.0f, 10.0f);

    private readonly List<ContextOption> m_Options = new();

    private void Start()
    {
        if (!s_Current) s_Current = this;

        gameObject.SetActive(false);

        m_Options.Capacity = m_OptionPoolCapacity;
        for (int i = 0; i < m_OptionPoolCapacity; i++)
        {
            m_Options.Add(Instantiate(m_OptionPrefab, transform).GetComponent<ContextOption>());
            m_Options[i].gameObject.SetActive(false);
        }
    }

    // NOTE(Pedro): Only implemented ISelectHandler interface
    // so Unity recognizes this as a selectable component
    public void OnSelect(BaseEventData eventData) { }

    public void OnDeselect(BaseEventData eventData) { Hide(); }

    public void Show(Vector2 position, IContext context)
    {
        ContextOptionConfiguration[] optionConfigs = context.GetOptionConfigurations();
        if (optionConfigs == null || optionConfigs.Length <= 0)
            return;

        int i = 0;
        foreach (var config in optionConfigs)
        {
            m_Options[i].SetOption(config);
            m_Options[i].gameObject.SetActive(true);
            i++;
        }

        for (; i < m_Options.Count; i++)
        {
            m_Options[i].gameObject.SetActive(false);
        }

        RectTransform rectTransform = transform as RectTransform;
        if (!rectTransform)
        {
            Debug.LogError("Context Menu component should be attached to a game object inside a canvas!");
            return;
        }

        // Game Object needs to be active for ForceRebuildLayoutImmediate to take place
        gameObject.SetActive(true);

        // NOTE(Pedro): Should not abuse this function,
        // only using it here because there is only one context menu
        // and this should be the only place updating its transform
        LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);

        // TODO(Pedro): Do these calculations in canvas normalized dimensions
        Vector2 menuSize = rectTransform.sizeDelta;
        Vector2 positionDelta = 0.5f * new Vector2(menuSize.x, -menuSize.y);
        positionDelta += new Vector2(m_Offset.x, -m_Offset.y);
        position += positionDelta;
        if (position.x + positionDelta.x > Screen.width)
        {
            position.x -= 2f * positionDelta.x;
        }
        if (position.y + positionDelta.y < 0f)
        {
            position.y -= 2f * positionDelta.y;
        }
        transform.position = position;
    }

    public void Hide()
    {
        gameObject.SetActive(false);

    }
}

public interface IContext 
{
    public ContextOptionConfiguration[] GetOptionConfigurations();
}
