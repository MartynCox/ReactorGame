using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HoverPointer : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // On hover change pointer
    [SerializeField] private Texture2D _pointerTexture;

    public void OnPointerEnter(PointerEventData eventData)
    {
        Cursor.SetCursor(_pointerTexture, new Vector2(12f, 0f), CursorMode.Auto);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }
}
