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
            if (raycastResults.Count > 0)
            {
                IContext context = raycastResults[0].gameObject.GetComponent<IContext>();
                if (context != null )
                {
                    ContextMenu.Current.Show(pointerEventData.position, context);
                }
            }
        }
    }
}
