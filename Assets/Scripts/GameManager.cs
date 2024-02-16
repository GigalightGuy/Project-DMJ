using UnityEngine;

public class GameManager : SingletonBase<GameManager>
{
    [SerializeField] private MonoBehaviour[] m_gameplayControllerScripts;

    public void DisableControllers()
    {
        foreach (var script in m_gameplayControllerScripts)
        {
            script.enabled = false;
        }

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void EnableControllers()
    {
        foreach (var script in m_gameplayControllerScripts)
        {
            script.enabled = true;
        }

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
