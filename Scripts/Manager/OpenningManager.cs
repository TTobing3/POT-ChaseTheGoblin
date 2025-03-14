using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Cinemachine;

public class OpenningManager : MonoBehaviour
{
    public Image fade;
    CanvasGroup fadeCanvasGroup;
    public Image[] buttons;
    public GameObject wife, jack, target, hammer, doubleButton, skipButton;
    Animator wifeAnimator, jackAnimator, targetAnimator;
    Transform wifeTransform, jackTransform, targetTransform;
    public CinemachineVirtualCamera virtualCamera;

    public AudioClip battleClip;
    public AudioSource targetAudioSource;


    Tween tween = null;

    void Awake() 
    {
        wifeAnimator = wife.GetComponent<Animator>();
        jackAnimator = jack.GetComponent<Animator>();
        targetAnimator = target.GetComponent<Animator>();

        wifeTransform = wife.GetComponent<Transform>();
        jackTransform = jack.GetComponent<Transform>();
        targetTransform = target.GetComponent<Transform>();

        fadeCanvasGroup = fade.GetComponent<CanvasGroup>();
    }

    void Start() 
    {
        jackTransform.position = new Vector3(11,0,0);
        Act(0);    
    }

    public void Act(int type)
    {
        if(type == 0)
        {
            jackTransform.DOMoveX(9,5f)
            .SetEase(Ease.Linear)
            .OnComplete(()=>{Act(1);});

            DOTween.To
            (
                () => virtualCamera.m_Lens.OrthographicSize, x => virtualCamera.m_Lens.OrthographicSize = x, 
                4.5f, 
                10
            ).SetEase(Ease.InCubic);
        }

        if(type == 1)
        {
            jackAnimator.SetInteger("type",type);
            jackAnimator.SetTrigger("act");
            tween =DOVirtual.DelayedCall(1f / Time.timeScale, () => Act(2));
        }

        
        if(type == 2)
        {
            jackAnimator.SetInteger("type",type);
            jackAnimator.SetTrigger("act");
            jackTransform.DOMoveX(-4,3)
            .SetEase(Ease.Linear)
            .OnComplete(()=>{Act(3);});
        }
        
        if(type == 3)
        {
            jackAnimator.SetInteger("type",type);
            jackAnimator.SetTrigger("act");
            tween = DOVirtual.DelayedCall(1.5f / Time.timeScale, () => Act(4));
        }
        if(type == 4)
        {
            jackAnimator.SetInteger("type",type);
            jackAnimator.SetTrigger("act");
            jackTransform.DOMoveX(-6.3f,4)
            .SetEase(Ease.Linear)
            .OnComplete(()=>{Act(5);});
        }

        if(type == 5)
        {
            jackAnimator.SetInteger("type",type);
            jackAnimator.SetTrigger("act");

            wifeAnimator.SetInteger("type", 1);
            wifeAnimator.SetTrigger("act");
            
            tween = DOVirtual.DelayedCall(5.5f / Time.timeScale, () => Act(6));
        }

        
        if(type == 6)
        {
            jackAnimator.SetInteger("type",type);
            jackAnimator.SetTrigger("act");
            
            tween = DOVirtual.DelayedCall(3f / Time.timeScale, () => Act(7));
        }

        if(type == 7)
        {
            jackAnimator.SetInteger("type",type);
            jackAnimator.SetTrigger("act");

            tween = DOVirtual.DelayedCall(1f / Time.timeScale, () => Act(8));
        }

        if(type == 8)
        {
            target.SetActive(true);
            targetTransform.localScale = new Vector3(-1,1,1);
            targetTransform.position = new Vector3(-8, 1.3f,0);
            targetTransform.DOMove( new Vector3(-1.5f, 0), 3)
            .SetEase(Ease.Linear)
            .OnComplete(()=>{Act(9);});
            
            targetAnimator.SetInteger("type", 1);
            targetAnimator.SetTrigger("act");

            
            tween = DOVirtual.DelayedCall(1.5f / Time.timeScale, () => { jackTransform.localScale = new Vector3(-1,1,1); });
        }

        
        if(type == 9)
        {
            targetAnimator.SetInteger("type", 0);
            targetAnimator.SetTrigger("act");

            targetTransform.localScale = new Vector3(1,1,1);
            

            AudioManager.instance.PlayAudioSource(targetAudioSource);
            AudioManager.instance.PlayAudioClip(1);
            
            DOTween.To
            (
                () => virtualCamera.m_Lens.OrthographicSize, x => virtualCamera.m_Lens.OrthographicSize = x, 
                6, 
                3
            ).SetEase(Ease.InCubic);

            tween = DOVirtual.DelayedCall(1f / Time.timeScale, () => {Act(10);});
        }

        if(type == 10)
        {
            targetTransform.localScale = new Vector3(-1,1,1);
            targetAnimator.SetInteger("type", 6);
            targetAnimator.SetTrigger("act");
            
            targetTransform.DOMove( new Vector3(12, 0), 3)
            .SetEase(Ease.Linear);

            fade.gameObject.SetActive(true);
            fadeCanvasGroup.alpha = 0;
            fadeCanvasGroup.DOFade(1,1).OnComplete(()=>{
            Time.timeScale = 1f;});

            doubleButton.gameObject.SetActive(false);

            fade.GetComponent<Image>().raycastTarget = false;
        }
    }

    public void Skip()
    {
        tween.Kill();

        DOTween.KillAll();

        fade.gameObject.SetActive(true);
        fadeCanvasGroup.alpha = 0;
        fadeCanvasGroup.DOFade(1,0.1f).OnComplete(()=>{
        Time.timeScale = 1f;});

        AudioManager.instance.PlayAudioClip(1);

    }

    public void Off()
    {
        target.SetActive(false);
        hammer.SetActive(false);
        jack.SetActive(false);
        skipButton.SetActive(false);
        
        foreach(Image i in buttons)
        {
            i.raycastTarget = false;
        }
    }

    public void Button(bool isEasy)
    {
        fadeCanvasGroup.DOFade(0,0.5f).OnComplete(()=>{fade.gameObject.SetActive(false);});
        Off();
        GameManager.instance.GameStart();

        if(isEasy)
        {
            GameManager.instance.level = 0;
            Time.timeScale = 2f;
        }
        else
        {
            GameManager.instance.level = 1;
            Time.timeScale = 3f;
        }
    }
}
