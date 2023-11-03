using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HoverPointer : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Texture2D _pointerTexture;
    [SerializeField] private Texture2D _defaultTexture;

    public void OnPointerEnter(PointerEventData eventData)
    {
        Cursor.SetCursor(_pointerTexture, new Vector2(12f, 0f), CursorMode.Auto);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Cursor.SetCursor(_defaultTexture, new Vector2(12f, 0f), CursorMode.Auto);
    }
}
