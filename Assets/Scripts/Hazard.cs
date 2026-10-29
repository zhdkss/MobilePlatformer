using UnityEngine;

public class Hazard : MonoBehaviour
{
    public int damage = 1;
    public bool destroyOnHit;     // для огненного шара

    void OnTriggerEnter2D(Collider2D other)
    {
        PlayerHealth player = other.GetComponent<PlayerHealth>();
        if (player == null) return;
        player.TakeDamage(damage);
        if (destroyOnHit) Destroy(gameObject);
    }
}