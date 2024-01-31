using System.Collections.Generic;
using UnityEngine;

public class ContextMenu : MonoBehaviour
{
    public static ContextMenu Current => s_Current;
    private static ContextMenu s_Current;

    [SerializeField] private GameObject m_OptionPrefab;
    [SerializeField] private int m_OptionPoolCapacity = 6;

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

        // TODO(Pedro): Reposition menu based on position on the screen
        transform.position = position;
        gameObject.SetActive(true);
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
