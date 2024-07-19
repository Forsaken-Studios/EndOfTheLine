using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragInspectView : MonoBehaviour, IBeginDragHandler, IDragHandler
{
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        //We put this item in front of others
        transform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }


}
