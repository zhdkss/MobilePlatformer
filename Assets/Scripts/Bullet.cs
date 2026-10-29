using UnityEngine;

public class Bullet : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        Boss boss = other.GetComponent<Boss>();
        if (boss != null)
        {
            boss.TakeHit();
            Destroy(gameObject);
            return;
        }

        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null){
            Destroy(enemy.gameObject);
            Destroy(gameObject);

        }
    }
}