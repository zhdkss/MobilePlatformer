using UnityEngine;

public class Pickup : MonoBehaviour
{
    public bool isHeart;          // галочка в Inspector: сердце или камень
    public AudioClip sound;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (isHeart) other.GetComponent<PlayerHealth>().Heal(1);
        else CoinCounter.Instance.Add();

        if (sound) AudioSource.PlayClipAtPoint(sound, transform.position);
        Destroy(gameObject);
    }
}