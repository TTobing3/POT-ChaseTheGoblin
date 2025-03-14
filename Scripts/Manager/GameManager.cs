using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Cinemachine;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;

public class GameManager : MonoBehaviour
{ 
    public System.Action<int> finishAction;

    public static GameManager instance { get; private set; }
    public GameObject onGame, onGameScene;
    public Player player;
    public Enemy target;
    public CinemachineVirtualCamera virtualCamera;
    public float defaultZoom;
    public int level = 0;
    public bool isMute = false;
    Tween zoomTween;
    bool isDouble = false;

    [Header("UI")]
    public CanvasGroup onScreenCanvasGroup;
    public CanvasGroup onFinishScreen;
    public TextMeshProUGUI finishText, scoreText;
    public Button timeButton;
    TextMeshProUGUI timeText;
    public Image fade;

    public bool isDone= false;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            //Destroy(gameObject);
        }

        timeButton.onClick.AddListener(SpeedDouble);
        timeText = timeButton.GetComponentInChildren<TextMeshProUGUI>();
    }

    void Start()
    {
        fade.gameObject.SetActive(true);
        fade.DOFade(0,1).OnComplete(()=>{fade.gameObject.SetActive(false);});

        AudioSource audioSource = GetComponent<AudioSource>();
        if (audioSource != null)
        {
            audioSource.volume = 0;
            audioSource.DOFade(1,1f); // Adjust the duration as needed
        }
    }

    public void GameStart()
    {
        onGame.SetActive(true);
        onGameScene.SetActive(true);
        onScreenCanvasGroup.alpha = 0;
        onScreenCanvasGroup.DOFade(1,1).SetDelay(1f);

        Zoom();
    }

    public void Zoom(float value)
    {
        //GameManager.instance.defaultZoom - power * 0.05f
        if (zoomTween != null) zoomTween.Kill();
        zoomTween = DOTween.To
        (
            () => virtualCamera.m_Lens.OrthographicSize, x => virtualCamera.m_Lens.OrthographicSize = x, 
            Mathf.Max(defaultZoom - value * 0.05f, 3f), 
            Mathf.Max(value * 0.1f, 5f)
        ).SetEase(Ease.InCubic);
    }
    public void Zoom()
    {
        if (zoomTween != null) zoomTween.Kill();
        zoomTween = DOTween.To
        (
            () => virtualCamera.m_Lens.OrthographicSize, 
            x => virtualCamera.m_Lens.OrthographicSize = x, 
            defaultZoom, 
            1f
        ).SetEase(Ease.OutCubic);
    }

    public void Zoom(bool isFinish)
    {
        if (zoomTween != null) zoomTween.Kill();
        zoomTween = DOTween.To
        (
            () => virtualCamera.m_Lens.OrthographicSize, 
            x => virtualCamera.m_Lens.OrthographicSize = x, 
            2, 
            4
        ).SetEase(Ease.InCubic);
    }

    public void SpeedDouble()
    {
        if(isDouble) 
        {
            isDouble = false;
            Time.timeScale = 1f;
            timeText.text = "x1";
        }
        else 
        {
            isDouble = true;
            Time.timeScale = 2f;
            timeText.text = "x2";
        }
        
    }

    public void GameSpeed(int speed)
    {
        Time.timeScale = speed;
    }

    public void FinishGame(int type)
    {
        if(isDone) return;
        Time.timeScale = 1f;

        onScreenCanvasGroup.DOFade(0, 1);
        finishAction(type);

        isDone= true;

        scoreText.text = "Your Run : "+(int)player.transform.position.x+"m";

        if(level == 1)
        {
            scoreText.text += " (HARD)";
        }
        // 구출
        if(type == 0)
        {
            finishText.text = "Successfully Save\nYour Baby...";

            Zoom(true);
            DOVirtual.DelayedCall(5f, () => 
            {
                onFinishScreen.gameObject.SetActive(true);
                onFinishScreen.DOFade(1, 3);
            } );

            AudioManager.instance.PlayAudioClip(3);
        }
        else if(type == 1) // 납치치
        {
            finishText.text = "The Goblin has vanished...\nWith the Baby...";

            virtualCamera.m_Follow = target.transform;

            target.speed = 4;
            
            Zoom(true);
            DOVirtual.DelayedCall(1f, () => 
            {
                onFinishScreen.gameObject.SetActive(true);
                onFinishScreen.DOFade(1, 3);
            } );
            
            AudioManager.instance.PlayAudioClip(2);
        }
        else if(type == 2) // 죽음음
        {
            finishText.text = "Your Vision Fading to Black...\nYou Collapse...";
            
            Zoom(true);
            DOVirtual.DelayedCall(1f, () => 
            {
                onFinishScreen.gameObject.SetActive(true);
                onFinishScreen.DOFade(1, 3);
            } );
            
            AudioManager.instance.PlayAudioClip(4);
        }
    }

    public void Restart()
    {
        SceneManager.LoadScene("PlayScene");
    }
}
