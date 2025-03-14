using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using Cinemachine;

public class MapManager : MonoBehaviour
{
    public static MapManager instance;
    
    [Header("UI")]
    public Image playerIcon;
    public Image enemyIcon;

    [Header("Transforms")]
    public Transform playerTransform;
    public Transform targetTransform;

    [Header("Minimap")]
    public RectTransform minimapRectTransform;

    public float mapWidth;

    private void Awake() 
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            //Destroy(gameObject);
        }
        //DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        UpdateIconPos();
    }

    void UpdateIconPos()
    {
        float playerPosX = playerTransform.position.x;
        float playerIconPosX = (playerPosX / mapWidth) * minimapRectTransform.rect.width;
        playerIcon.rectTransform.anchoredPosition = new Vector2(playerIconPosX, playerIcon.rectTransform.anchoredPosition.y);

        float targetPosX = targetTransform.position.x;
        float targetIconPosX = (targetPosX / mapWidth) * minimapRectTransform.rect.width;
        enemyIcon.rectTransform.anchoredPosition = new Vector2(targetIconPosX, enemyIcon.rectTransform.anchoredPosition.y);
    }
}
