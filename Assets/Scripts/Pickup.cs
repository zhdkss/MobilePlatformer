using UnityEngine;

public class Pickup : MonoBehaviour
{
    public bool isHeart;          // галочка в Inspector: сердце или камень
    public ItemStats stats;       // сколько очков или здоровья даёт
    public AudioClip sound;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (isHeart) other.GetComponent<PlayerHealth>().Heal(stats.healAmount);
        else CoinCounter.Instance.Add(stats.scoreValue);

        if (sound) AudioSource.PlayClipAtPoint(sound, transform.position);
        Destroy(gameObject);
    }
}