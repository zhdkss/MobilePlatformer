using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float bounceForce = 10f;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        Rigidbody2D playerRb = other.GetComponent<Rigidbody2D>();

        // игрок падает сверху -> враг умирает
        if (playerRb.linearVelocity.y < 0 && other.transform.position.y > transform.position.y)
        {
            playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, bounceForce);
            Destroy(gameObject);
        }
        else
        {
            other.GetComponent<PlayerHealth>().TakeDamage(1);
        }
    }
}