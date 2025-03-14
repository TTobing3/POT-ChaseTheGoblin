using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SkipButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        Time.timeScale = 3f;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Time.timeScale = 1f;
    }
}
