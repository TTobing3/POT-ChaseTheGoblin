using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;

public enum ActState
{
    Idle,
    Move,
    Back,
    Attack,
    Hit,
    Dead
}

public class Enemy : MonoBehaviour
{
    public System.Action deadAction;
    
    Rigidbody2D rigid2D;
    BoxCollider2D boxCollider2D;
    Animator animator;

    
    UnitEffect[] unitEffects;
    Coroutine actCoroutine;

    ActState actState = ActState.Idle;

    Tween rigidTween;
    public bool isDead = false;

    public Transform character;

    [Header("UI")]
    public Slider hpSlider;
    public TextMeshProUGUI hpText, damageText;
    public CanvasGroup canvasGroup;
    
    [Header("Sound")]
    AudioSource[] audioSources;
    public AudioClip[] audioNormalClips;
    public AudioClip[] audioAttackClips, audioHitClips, audioDeadClips;

    [Header("Status")]
    public int hp;
    public int maxHp;
    public float speed = 1;
    public int type;

    bool superArmor = false;

    #region System

    void Awake() 
    {
        rigid2D = GetComponent<Rigidbody2D>();  
        boxCollider2D = GetComponent<BoxCollider2D>();
        unitEffects = GetComponentsInChildren<UnitEffect>();
        canvasGroup = GetComponentInChildren<CanvasGroup>();
        animator = GetComponentInChildren<Animator>();
        audioSources = GetComponentsInChildren<AudioSource>();

        hpSlider.maxValue = maxHp;
        hpSlider.value = hp;
        hpText.text = hp.ToString();
    }

    void Start()
    {
        StartAct();
        GameManager.instance.finishAction += Finish;   

        SetHp(maxHp); 
    }

    #endregion

    #region Act

    void Act()
    {
        if (isDead) return;

        if(type == 1) ActTarget();
        else ActNormal();

    }

    void StartAct()
    {
        StartCoroutine(CoAct());
    }

    IEnumerator CoAct()
    {
        Act();
        yield return new WaitForSeconds(0.5f);
        actCoroutine = StartCoroutine(CoAct());
    }

    void ActNormal()
    {
        if (GameManager.instance.player.isDead)
        {
            actState = ActState.Idle;
            Stop();
        }
        else if (transform.position.x < GameManager.instance.player.transform.position.x)
        {
            actState = ActState.Back;
            Move(true);
        }
        else if (Vector2.Distance(transform.position, GameManager.instance.player.transform.position) < 1.6f && actState != ActState.Attack)
        {
            actState = ActState.Attack;
            Attack();
        }
        else if (Vector2.Distance(transform.position, GameManager.instance.player.transform.position) < 1f && actState != ActState.Back)
        {
            actState = ActState.Back;
            Move(true);
        }
        else if(Vector2.Distance(transform.position, GameManager.instance.player.transform.position) < 12f && actState != ActState.Move)
        {
            actState = ActState.Move;
            Move();
        }
        else if(Vector2.Distance(transform.position, GameManager.instance.player.transform.position) > 12f)
        {
            actState = ActState.Idle;
            Stop();
        }
    }

    void ActTarget()
    {
        //actState = ActState.Move;

        if(transform.position.x > MapManager.instance.mapWidth)
        {
            isDead = true;
            print("도주 성공");
            //Stop();
            GameManager.instance.FinishGame(1);
        }
        else
        {
            Run();
        }
    }

    #endregion

    #region Pattern

    void Attack()
    {
        Stop();
        
        animator.SetInteger("type", 5);
        animator.SetTrigger("act");

        DOVirtual.DelayedCall(0.3f, () => GameManager.instance.player.Hit(5));
        
        audioSources[0].clip = audioAttackClips[Random.Range(0, audioAttackClips.Length)];
        AudioManager.instance.PlayAudioSource(audioSources[0]);
    }

    void Move(bool isClose = false)
    {
        if (isDead) return;

        if(isClose)
        {
            ChangeVelocity(speed * 1.5f, 0f);
            
            animator.SetInteger("type", 2);
            animator.SetTrigger("act");
        }
        else
        {
            
            ChangeVelocity(speed * -1,0.5f);
            
            animator.SetInteger("type", 1);
            animator.SetTrigger("act");
        }
    }

    void Stop()
    {
        ChangeVelocity(0,1);
        animator.SetInteger("type", 0);
        animator.SetTrigger("act");
    }

    void Run()
    {
        character.transform.localScale = new Vector3(-1, 1, 1);
        ChangeVelocity(speed, 0);
        animator.SetInteger("type", 6);
        animator.SetTrigger("act");
    }

    void Cry()
    {

    }

    public void CrySound()
    {
        audioSources[0].clip = audioNormalClips[Random.Range(0, audioNormalClips.Length)];
        AudioManager.instance.PlayAudioSource(audioSources[0]);
    }

    #endregion  

    #region State

    void SetHp(int hp)
    {
        if(hp > maxHp) hp = maxHp;
        else if(hp <= 0)
        {
            hp = 0;
            Dead();
        }

        DOTween.Kill(hpSlider);
        DOTween.Kill(hpText);

        hpText.text = hp.ToString();
        hpText.DOCounter(this.hp, hp, 0.5f).SetEase(Ease.OutCubic);
        hpSlider.DOValue(hp, 0.5f);
        
        this.hp = hp;

    }

    public void Hit(int power)
    {
        if(superArmor) return;

        if(actCoroutine != null) 
        {
            actState = ActState.Idle;
            StopCoroutine(actCoroutine);
            DOVirtual.DelayedCall(0.5f, StartAct);
        }

        print(power+"타격");
        print(hp+"hp");

        superArmor = true;
        DOVirtual.DelayedCall(0.5f, ()=>{superArmor = false;});


        DOTween.Kill(damageText);

        damageText.text = $"-{power}";
        damageText.color = Color.white;
        damageText.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

        damageText.GetComponent<RectTransform>().DOAnchorPosY(200 + (power * 3), 5).SetEase(Ease.OutQuad);
        damageText.DOFade(0, 3).SetEase(Ease.OutQuad).SetDelay(2);
        damageText.fontSize = Mathf.Max(power * 3, 36) ;
        
        animator.SetInteger("type", 3);
        animator.SetTrigger("act");
        
        foreach (var effect in unitEffects) effect.HitWhiteFlash();

        rigid2D.velocity = new Vector2( rigid2D.velocity.x + power * 0.1f, 0);

        Stop();

        SetHp(hp - power);

        if(isDead) return;

        audioSources[0].clip = audioHitClips[Random.Range(0, audioHitClips.Length)];
        AudioManager.instance.PlayAudioSource(audioSources[0]);
    }

    void Dead()
    {
        if (actCoroutine != null) StopCoroutine(actCoroutine);
        
        Stop();
        animator.SetInteger("type", 4);
        animator.SetTrigger("act");

        isDead = true;

        canvasGroup.DOFade(0, 1f);

        boxCollider2D.isTrigger = true;

        if(deadAction != null) deadAction();

        
        audioSources[0].clip = audioDeadClips[Random.Range(0, audioDeadClips.Length)];
        AudioManager.instance.PlayAudioSource(audioSources[0]);
    }
    void ChangeVelocity(float velocity = 0, float delay = 0)
    {
        if (rigidTween != null) rigidTween.Kill();

        Ease ease = Ease.InCubic;
        if(velocity == 0) ease = Ease.OutCubic;

        rigidTween = DOTween.To(() => rigid2D.velocity, x => rigid2D.velocity = x, new Vector2(velocity, 0), delay).SetEase(ease);
    }
    
    #endregion
    
    void Finish(int type)
    {
        canvasGroup.DOFade(0,1);
    }
}
