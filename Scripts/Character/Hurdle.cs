using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;

public class Hurdle : MonoBehaviour
{
    
    Rigidbody2D rigid2D;
    BoxCollider2D boxCollider2D;
    UnitEffect[] unitEffects;
    SpriteRenderer[] spriteRenderers;
    
    Tween rigidTween;

    public bool isBroken, isFixed;



    [Header("UI")]
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI damageText;

    [Header("Sound")]
    AudioSource[] audioSources;
    public AudioClip[] audioHitClips;
    public CanvasGroup canvasGroup;
    [Header("Status")]
    public int hp;
    public int maxHp;

    void Awake() 
    {
        rigid2D = GetComponent<Rigidbody2D>();  
        boxCollider2D = GetComponent<BoxCollider2D>();
        unitEffects = GetComponentsInChildren<UnitEffect>();
        canvasGroup = GetComponentInChildren<CanvasGroup>();
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        audioSources = GetComponentsInChildren<AudioSource>();


        hpText.text = hp.ToString();
    }

    void Start()
    {
        SetHp(maxHp);
    }
    
    public void Hit(int power)
    {
        if(isBroken) return;
        foreach (var effect in unitEffects) effect.HitWhiteFlash();

        if(!isFixed) 
        {
            rigid2D.velocity = new Vector2( rigid2D.velocity.x + power * 0.1f, 0);
            Stop();
        }
        SetHp(hp - power);
        
        damageText.text = $"-{power}";
        damageText.color = Color.white;
        damageText.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

        damageText.GetComponent<RectTransform>().DOAnchorPosY(200 + (power * 3), 5).SetEase(Ease.OutQuad);
        damageText.DOFade(0, 3).SetEase(Ease.OutQuad).SetDelay(2);
        damageText.fontSize = Mathf.Max(power * 3, 36) ;
        
        audioSources[0].clip = audioHitClips[Random.Range(0, audioHitClips.Length)];
        AudioManager.instance.PlayAudioSource(audioSources[0]);
    }

    void SetHp(int hp)
    {
        if(hp > maxHp) hp = maxHp;
        else if(hp <= 0)
        {
            hp = 0;
            Dead();
        }

        DOTween.Kill(hpText);

        hpText.text = hp.ToString();
        hpText.DOCounter(this.hp, hp, 0.5f).SetEase(Ease.OutCubic);
        
        this.hp = hp;

    }

    void Dead()
    {
        isBroken = true;

        canvasGroup.DOFade(0, 1f);
        spriteRenderers.ToList().ForEach(x => x.DOFade(0, 2f).SetDelay(1f));

        boxCollider2D.isTrigger = true;
    }

    void ChangeVelocity(float velocity = 0, float delay = 0)
    {
        if (rigidTween != null) rigidTween.Kill();

        Ease ease = Ease.InCubic;
        if(velocity == 0) ease = Ease.OutCubic;

        rigidTween = DOTween.To(() => rigid2D.velocity, x => rigid2D.velocity = x, new Vector2(velocity, 0), delay).SetEase(ease);
    }

    void Stop()
    {
        ChangeVelocity(0,1);
    }
}
