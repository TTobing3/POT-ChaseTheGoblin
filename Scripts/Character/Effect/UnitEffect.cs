using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;
using DG.Tweening;

public class UnitEffect : MonoBehaviour
{
    public Material paintWhite;
    
    Material originMaterial;

    SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originMaterial = spriteRenderer.material;
    }

    public void HitWhiteFlash()
    {
        StartCoroutine(CoHitWhiteFlash());
    }

    IEnumerator CoHitWhiteFlash()
    {
        spriteRenderer.material = paintWhite;

        yield return new WaitForSeconds(0.1f);

        spriteRenderer.material = originMaterial;
    }
}
