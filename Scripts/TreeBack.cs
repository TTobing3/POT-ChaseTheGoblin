using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeBack : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    public Sprite[] sprites;

    void Awake() 
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    void Start() 
    {
        var shadow = Random.Range(0.5f, 1f);

        spriteRenderer.color = new Color(shadow, shadow, shadow, 1);

        var gap = Random.Range(-1, 0.5f);
        transform.position = new Vector3(transform.position.x, transform.position.y + gap );

        spriteRenderer.sprite = sprites[Random.Range(0, sprites.Length)];
    }
}
