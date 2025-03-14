using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class VisualEffect : MonoBehaviour
{
    SpriteRenderer spriteRenderer;

    public Transform[] particles;

    private void Awake() {
        
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    void OnEnable() 
    {
        spriteRenderer.color = Color.white;
        spriteRenderer.DOFade(0,0.5f).OnComplete(()=>{gameObject.SetActive(false);});

        foreach(Transform i in particles) 
        {
            i.GetComponent<SpriteRenderer>().color = Color.white;
            i.transform.position = transform.position + new Vector3(0.1f, -0.8f);
        }
        foreach(Transform i in particles)
        {
            //i.GetComponent<SpriteRenderer>().DOFade(0, Random.Range(0.2f, 0.5f)).SetEase(Ease.OutQuad);
            var pos =  i.transform.position + new Vector3( Random.Range(-0.8f, 0.8f),  Random.Range(0.2f, 0.8f));
            i.DOMove(pos, 0.5f).SetEase(Ease.OutQuad);
        }
    }
}
