using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            EventSystem currentEventSystem = EventSystem.current;
            PointerEventData pointerEventData = new(currentEventSystem);
            pointerEventData.position = Input.mousePosition;
            List<RaycastResult> raycastResults = new();
            currentEventSystem.RaycastAll(pointerEventData, raycastResults);
            // GetComponent allocates even if the component is not found,
            // so we use TryGetComponent instead
            if (raycastResults.Count > 0)
            {
                if (raycastResults[0].gameObject.TryGetComponent<IContext>(out var context))
                {
                    ContextMenu.Current.Show(pointerEventData.position, context);
                    currentEventSystem.SetSelectedGameObject(ContextMenu.Current.gameObject);
                }
            }
        }
    }
}
