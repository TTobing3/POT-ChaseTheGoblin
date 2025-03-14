using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using Cinemachine;

public class Player : MonoBehaviour
{
    Rigidbody2D rigid2D;
    Animator animator;
    List<Collider2D> hitColliders = new List<Collider2D>();
    UnitEffect[] unitEffects;
    [SerializeField] PlayerEffect attackEffect;
    [SerializeField] VisualEffect visualEffect;
    [SerializeField] CinemachineVirtualCamera virtualCamera;

    [SerializeField] Transform handF;

    [Header("UI")]
    public Slider hpSlider, powerSlider;
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI powerText, powerText_, maxPowerText;
    public CanvasGroup canvasGroup;

    [Header("Camera")]
    CinemachineBasicMultiChannelPerlin perlin;

    [Header("Sound")]
    public AudioSource[] audioSources;
    public AudioSource hitAudioSource;


    [Header("Status")]
    public int hp;
    public int maxHp;
    public int maxPower, power, powerIncrease;
    public float speed = 1;
    int state = 0;
    bool buttonDown = false, actLock = false;
    Tween rigidTween, chargeTween;
    Coroutine chargeCoroutine, recoverCoroutine, triggerCoroutine;

    public bool isMove, isDead;

    public bool isStopMove
    {
        get
        {
            return hitColliders.Exists(x => 
                (x.gameObject.CompareTag("Enemy") && !x.GetComponent<Enemy>().isDead) || 
                (x.gameObject.CompareTag("Hurdle") && !x.GetComponent<Hurdle>().isBroken));
        }
    }

    void Awake() 
    {
        rigid2D = GetComponent<Rigidbody2D>();    
        animator = GetComponentInChildren<Animator>();
        unitEffects = GetComponentsInChildren<UnitEffect>();
        perlin = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        canvasGroup = GetComponentInChildren<CanvasGroup>();
    
        hpSlider.maxValue = maxHp;
        hpSlider.value = hp;
        hpText.text = hp.ToString();
    }

    void Start() 
    {
        GameManager.instance.finishAction += Finish;    

        ChangeMaxPower(maxPower);
        ChangePower(0);
    }

    public void Act(bool isAct)
    {
        if(actLock || isDead) return;

        buttonDown = isAct;

        animator.ResetTrigger("stopMove");

        if(isAct && state == 0) // 누름
        {
            animator.ResetTrigger("act");
            animator.SetBool("mouseDown", true);
            SetTrigger("act");

            if(!isStopMove) 
            {
                Move();
            }
            else 
            {
                animator.SetTrigger("stopMove");
                Stop();
            }

            if(hp>1) SetHp(hp - 1);

            Charge();
            Recover(false);
            
            GameManager.instance.Zoom(maxPower);
        }
        if(!isAct && state == 1) // 뗌
        {
            bool isReady = power >= 10;
        
            animator.ResetTrigger("act");
            animator.SetBool("ready", isReady);
            animator.SetBool("mouseDown", false);
            SetTrigger("act");
            
            if(isReady) 
            {
                Smash();
            }
            else
            {
                Cancle();
            }

            animator.SetTrigger("stopMove");
            Stop();

            Recover();
        }
    }

    #region Controller

    void Charge()
    {
        state = 1;

        ChangePower(0);

        ChangePowerSoft(maxPower, 1.5f + Random.Range(-0.5f, 0), 0.2f);
    }

    //

    void Smash()
    {
        state = 2;

        if(chargeCoroutine != null) StopCoroutine(chargeCoroutine);

        chargeTween.Kill();

        int damage = power;

        ActDelay(()=>
        {
            state = 0;
            
            if(buttonDown) Act(true);
        }, 1.5f); // smash 클립 길이 + 0.1f

        ActDelay(()=>
        {
            // 카메라 진동 효과 트리거
            perlin.m_AmplitudeGain = power * 0.1f;
            perlin.m_FrequencyGain = power * 0.1f;

            visualEffect.gameObject.SetActive(true);
            
            attackEffect.gameObject.SetActive(true);
            attackEffect.transform.position = new Vector3(transform.position.x, 0, 0);
            attackEffect.transform.DOMoveX(transform.position.x+ 1,0.1f);

            attackEffect.damage = damage;
            
            GameManager.instance.Zoom();

            ActDelay(()=>
            {
                // 카메라 진동 효과 트리거
                perlin.m_AmplitudeGain = 0f;
                perlin.m_FrequencyGain = 0f;
                
                ChangePowerSoft(0, 0.5f);

                attackEffect.gameObject.SetActive(false);
            }, 0.2f);
        }, 0.2f); // smash 클립 중 내려찍는 순간

        foreach(AudioSource i in audioSources)
        {
            AudioManager.instance.PlayAudioSource(i);
        }
    }

    //

    void Cancle()
    {
        state = 2;
        
        ChangePowerSoft(0, 0.2f);

        if(chargeCoroutine != null) StopCoroutine(chargeCoroutine);
        
        GameManager.instance.Zoom();
        
        ActDelay(()=>
        {
            state = 0;
            if(buttonDown) Act(true);
        }, 0.3f); // smash 클립 길이 + 0.1f
    }

    //

    void ChangePowerSoft(float value, float duration, float delay = 0, bool isCharge = true, System.Action action = null)
    {
        int level = GameManager.instance.level;

        chargeTween.Kill();

        if(isCharge)
        {
            chargeTween = DOTween.To(() => power, x => power = (int)x, value, duration)
                .SetEase(Ease.InExpo)
                .OnUpdate(() => ChangePower(power))
                .SetDelay(delay)
                .OnComplete(()=>{ if(action != null) action();});
        }
        else
        {
            chargeTween = DOTween.To(() => power, x => power = (int)x, value, duration)
                .SetEase(Ease.OutExpo)
                .OnUpdate(() => ChangePower(power))
                .SetDelay(delay)
                .OnComplete(()=>{ if(action != null) action();});
        }
    }

    void ChangePower(int power)
    {
        this.power = power;
        
        powerText.text = power.ToString();
        powerText.fontSize = power*2;
        powerText.alpha = 1;
        
        powerText_.text = $"{power} / {maxPower}";
        //maxPowerText.fontSize = Mathf.Max(power * 2, 24);

        powerSlider.value = power;

        if(power == maxPower)
        {
            if(hp>1) SetHp(hp-1);
            ChangePowerSoft(10, 1, 0.05f, false, ()=>{ ChangePowerSoft(maxPower, 1.5f + Random.Range(-0.5f, 0)); });
        }
    }

    void ChangeMaxPower(int maxPower)
    {
        this.maxPower = maxPower;

        powerSlider.maxValue = maxPower;
        powerText_.text = $"{power} / {maxPower}";
    }

    //

    void Move()
    {
        isMove = true;

        if (rigidTween != null) rigidTween.Kill();
        rigidTween = DOTween.To(() => rigid2D.velocity, x => rigid2D.velocity = x, new Vector2(speed, 0), 1f);//.SetDelay(0.3f);
    }

    void Stop()
    {
        isMove = false;
        
        //animator.ResetTrigger("stopMove");
        //animator.SetTrigger("stopMove");
        if (rigidTween != null) rigidTween.Kill();
        rigidTween = DOTween.To(() => rigid2D.velocity, x => rigid2D.velocity = x, new Vector2(0, 0), 0.5f);
    }

    public void Catch(int l, GameObject baby = null)
    {
        if(l == 0)
        {
            GameManager.instance.FinishGame(0);
            animator.SetTrigger("catch");
        }
        else if(l == 1)
        {
            baby.transform.SetParent(handF);
        }
    }

    #endregion

    #region  Status

    public void Hit(int power)
    {
        if(isDead) return;
        foreach (var effect in unitEffects) 
        {
            if(effect.gameObject.activeSelf)
                effect.HitWhiteFlash();
        }
        SetHp(hp - power);

        AudioManager.instance.PlayAudioSource(hitAudioSource);
    }

    void Dead()
    {
        if(GameManager.instance.isDone) return;
        //animator.SetInteger("type", 4);
        //animator.SetTrigger("act");
        isDead = true;

        canvasGroup.DOFade(0, 1f);
        Stop();

        SetTrigger("dead");
        
        GameManager.instance.FinishGame(2);
    }

    void SetHp(int hp)
    {
        if(hp > maxHp) hp = maxHp;
        else if(hp <= 0)
        {
            this.hp = hp;
            Dead();
        }
        
        DOTween.Kill(hpSlider);
        DOTween.Kill(hpText);

        //hpText.text = hp.ToString();
        hpText.DOCounter(this.hp, hp, 0.5f);
        hpSlider.DOValue(hp, 0.5f);

        this.hp = hp;
    }
    
    #endregion

    #region  Coroutine


    void Recover(bool isAct = true)
    {
        if(isAct)
        {
            recoverCoroutine = StartCoroutine(CoRecover());
        }
        else
        {
            if(recoverCoroutine != null) StopCoroutine(recoverCoroutine);
            recoverCoroutine = null;
        }
    }

    IEnumerator CoRecover()
    {   
        yield return new WaitForSeconds(1f);
        SetHp(hp + 2);
        
        recoverCoroutine = StartCoroutine(CoRecover());
    }


    void ActDelay(System.Action action)
    {
        StartCoroutine(CoActDelay(action));
    }

    private IEnumerator CoActDelay(System.Action action)
    {
        yield return null;

        action();
    }

    void ActDelay(System.Action action, float delay)
    {
        StartCoroutine(CoActDelay(action, delay));
    }

    private IEnumerator CoActDelay(System.Action action, float delay)
    {
        yield return new WaitForSeconds(delay);

        action();
    }

    void SetTrigger(string trigger)
    {
        if (triggerCoroutine != null) StopCoroutine(triggerCoroutine);
        triggerCoroutine = StartCoroutine(SetTriggerAfterTransition(trigger));
    }

    IEnumerator SetTriggerAfterTransition(string triggerName)
    {
        // 트랜지션이 완료될 때까지 대기합니다.
        while (animator.IsInTransition(0))
        {
            yield return null;
        }

        // 트랜지션이 완료된 후 트리거를 설정합니다.
        animator.SetTrigger(triggerName);
    }

    #endregion

    void Finish(int type)
    {
        canvasGroup.DOFade(0,1);
        actLock = true;
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        hitColliders.Add(collider);

        if (collider.gameObject.CompareTag("Enemy") && !collider.GetComponent<Enemy>().isDead)
        {
            animator.SetTrigger("stopMove");
            Stop();
        }

        if (collider.gameObject.CompareTag("Hurdle") && !collider.GetComponent<Hurdle>().isBroken)
        {
            animator.SetTrigger("stopMove");
            Stop();
        }
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        hitColliders.Remove(collider);
    }
}
