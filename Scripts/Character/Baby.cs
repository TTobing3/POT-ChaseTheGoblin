using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;
using DG.Tweening;
using Unity.VisualScripting;
using System.Threading.Tasks;

public class Baby : MonoBehaviour
{
    public GameObject cryObject;
    public Sprite[] cryTexts;

    public SpriteRenderer spriteRenderer;

    public AudioSource audioSource;
    public AudioClip[] clips;

    public Enemy enemy;

    void Awake() 
    {
        enemy = GetComponentInParent<Enemy>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();

        enemy.deadAction += Escape;
    }

    void Start() 
    {
        Cry();    
        StartCoroutine(PlayRandomClip());
    }

    IEnumerator PlayRandomClip()
    {
        while (true)
        {
            print(clips.Length);
            print(Random.Range(0, 2));
            audioSource.clip = clips[Random.Range(0, 2)];
            AudioManager.instance.PlayAudioSource(audioSource);
            yield return new WaitForSeconds(Random.Range(5f, 10f));
        }
    }

    public void Cry()
    {
        var spriteRenderer = cryObject.GetComponent<SpriteRenderer>();

        cryObject.gameObject.SetActive(true);
        cryObject.transform.position = transform.position;
        cryObject.transform.rotation = Quaternion.Euler(Vector2.zero);

        spriteRenderer.color = Color.white;
        spriteRenderer.sprite = cryTexts[Random.Range(0, 2)];

        cryObject.transform.DORotate(new Vector3(0,0,Random.Range(-90, 90f)),1);
        cryObject.transform.DOMove(transform.position + new Vector3(Random.Range(-1,1f),Random.Range(1,2f)), 1);
        spriteRenderer.DOFade(0, 1).OnComplete(()=>
        {
            Cry();
        });
    }

    void Escape()
    {
        var player = GameManager.instance.player;
        var playerTransform = player.transform;

        spriteRenderer.sortingLayerName = "Unit";
        spriteRenderer.sortingOrder = 14;

        player.Catch(0);

        transform.SetParent(playerTransform);

        Vector2 targetPos = new Vector2( 
            playerTransform.position.x + 0.4f + (( transform.position.x - playerTransform.position.x ) / 2),
            playerTransform.position.y + 2);

        transform.DOMove(targetPos, 2).SetEase(Ease.OutCubic).OnComplete(()=>
        {
            targetPos = new Vector2( 
            playerTransform.position.x + 0.4f,
            playerTransform.position.y + 0.4f);

            transform.DOMove(targetPos, 2).SetEase(Ease.InCubic).OnComplete(()=>{player.Catch(1, gameObject);});
        });
    }
}
