using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MainButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        GameManager.instance.player.Act(true);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        GameManager.instance.player.Act(false);
    }
}
