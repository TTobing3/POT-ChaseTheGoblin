using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    List<AudioSource> audioSources = new List<AudioSource>();

    AudioSource audioSource;
    public AudioClip[] bgmClip;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }

        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        PlayAudioSource(audioSource);

        if(GameManager.instance != null && GameManager.instance.isMute)
            foreach (AudioSource i in audioSources) i.volume = 0;
    }

    public void PlayAudioSource(AudioSource source)
    {
        audioSources.Add(source);

        if (GameManager.instance != null && GameManager.instance.isMute) return;

        source.Play();
    }
    
    public void PlayAudioClip(int type)
    {
        if (GameManager.instance != null && GameManager.instance.isMute) return;


        // 현재 볼륨 저장
        float originalVolume = audioSource.volume;
        float fadeDuration = 3.0f; // 페이드 지속 시간

        // 볼륨 줄이기
        audioSource.DOFade(0, fadeDuration).OnComplete(() =>
        {
            audioSource.clip = null;

            if(type == -1) return;

            // 오디오 클립 변경
            audioSource.clip = bgmClip[type];

            if (GameManager.instance != null && GameManager.instance.isMute) return;
            
            audioSource.Play();
            audioSource.DOFade(originalVolume, fadeDuration);
        });
    }

    private Dictionary<AudioSource, float> originalVolumes = new Dictionary<AudioSource, float>();

    [ContextMenu("Mute")]
    public void Mute()
    {
        if (GameManager.instance.isMute)
        {
            foreach (AudioSource source in audioSources)
            {
                if (originalVolumes.ContainsKey(source))
                {
                    source.volume = originalVolumes[source];
                }
            }
        }
        else
        {
            foreach (AudioSource source in audioSources)
            {
                originalVolumes[source] = source.volume;
                source.volume = 0;
            }
        }
        GameManager.instance.isMute = !GameManager.instance.isMute;
    }
}
