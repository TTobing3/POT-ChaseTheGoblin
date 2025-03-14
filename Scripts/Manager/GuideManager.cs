using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class GuideManager : MonoBehaviour
{
    public Slider guideSlider;
    public Tween chargeTween;
    public int power, curPage;

    public GameObject[] pages;
    public GameObject[] buttons;

    public AudioSource audioSource;
    public GameObject guideScreen;

    float time = 0;

    private void Awake() 
    {
        guideSlider.maxValue = 100;    
    }

    private void Start() {
        Page1();

        curPage = -1;
        Next(true);
    }

    public void Next(bool isNext)
    {
        if(isNext)
        {
            if(curPage == pages.Length - 1) return;
            curPage++;
        }
        else
        {
            if(curPage == 0) return;
            curPage--;
        }   

        AudioManager.instance.PlayAudioSource(audioSource);

        foreach(GameObject i in pages) i.gameObject.SetActive(false);
        foreach(GameObject i in buttons) i.gameObject.SetActive(true);

        pages[curPage].SetActive(true);



        if(curPage == 0) buttons[0].gameObject.SetActive(false);
        if(curPage == pages.Length - 1) buttons[1].gameObject.SetActive(false);
    }

    public void Page1()
    {
        guideSlider.value = 0;
        ChangePowerSoft(100, 2);
    }

    void ChangePowerSoft(float value, float duration, float delay = 0, bool isCharge = true, System.Action action = null)
    {
        if(!guideSlider.gameObject.activeSelf) return;

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
        
        guideSlider.value = power;

        if(power == 100)
        {
            ChangePowerSoft(10, 1, 0.05f, false, ()=>{ ChangePowerSoft(100, 1.5f + Random.Range(-0.5f, 0)); });
        }
    }

    public void GuideButton()
    {
        if(guideScreen.activeSelf)
        {
            guideScreen.SetActive(false);
            Time.timeScale = time;
        }
        else
        {
            guideScreen.SetActive(true);
            
            time = Time.timeScale;
            Time.timeScale = 0;
        }
    }
}
