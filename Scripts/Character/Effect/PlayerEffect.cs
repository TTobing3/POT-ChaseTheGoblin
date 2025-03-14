using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerEffect : MonoBehaviour
{
    Player player; // Player 스크립트를 참조

    public int damage;

    private void Awake() 
    {
        player = GetComponentInParent<Player>();    
    }

    void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            Attack(other.GetComponent<Enemy>());
        }
        if (other.gameObject.CompareTag("Hurdle"))
        {
            Attack(other.GetComponent<Hurdle>());
        }
    }
    
    void Attack(GameObject enemy)
    {
        int power = player.power; // Player의 power 값을 가져옴
        Enemy enemyScript = enemy.GetComponent<Enemy>(); // Enemy 스크립트를 참조
        if (enemyScript != null)
        {
            enemyScript.Hit(damage); // Enemy의 Hit 메서드를 호출
        }
    }

    void Attack(Enemy enemy)
    {
        int power = player.power; // Player의 power 값을 가져옴
        if (enemy != null)
        {
            enemy.Hit(damage); // Enemy의 Hit 메서드를 호출
        }
    }
    void Attack(Hurdle hurdle)
    {
        int power = player.power; // Player의 power 값을 가져옴
        if (hurdle != null)
        {
            hurdle.Hit(damage); // Enemy의 Hit 메서드를 호출
        }
    }
    
}
