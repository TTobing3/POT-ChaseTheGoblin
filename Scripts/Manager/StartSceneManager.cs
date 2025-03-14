using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

        using DG.Tweening;

public class StartSceneManager : MonoBehaviour
{


    public Image fadeImage;

    public AudioClip[] audioClips;
    public AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(PlayRandomAudioSource(GetComponent<AudioSource>()));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadPlayScene()
    {
        fadeImage.gameObject.SetActive(true);
        audioSource.DOFade(0, 1f);
        fadeImage.DOFade(1, 1f).OnComplete(() => 
        {
            SceneManager.LoadScene("PlayScene");
        });
    }


    IEnumerator PlayRandomAudioSource(AudioSource audioSource)
    {
        while (true)
        {
            float waitTime = Random.Range(2f, 5f);
            yield return new WaitForSeconds(waitTime);
            audioSource.clip = audioClips[Random.Range(0, audioClips.Length)];
            audioSource.Play();
        }
    }
}
