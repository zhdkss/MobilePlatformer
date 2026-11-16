using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public PlayerStats stats;         // maxHp и время неуязвимости лежат в ассете
    public Image[] hearts;            // 3 сердца из HUD
    public float blinkInterval = 0.1f; // как часто мигает спрайт (только вид)
    public float fallY = -10f;        // ниже этой высоты = упал в яму

    int hp;
    bool invincible;
    bool dead;
    SpriteRenderer sr;

    void Start()
    {
        hp = stats.maxHp;
        sr = GetComponent<SpriteRenderer>();
        UpdateHearts();
    }

    void Update()
    {
        if (!dead && transform.position.y < fallY) Die();
    }

    public void TakeDamage(int amount)
    {
        if (invincible) return;              // ещё мигаем — урон не проходит
        hp -= amount;
        GameAudio.Play(Sfx.Hit);
        UpdateHearts();
        if (hp <= 0) { Die(); return; }
        StartCoroutine(Invincibility());
    }

    void Die()
    {
        dead = true;
        GameAudio.Play(Sfx.Lose);
        SceneLoader.Restart();
    }

    public void Heal(int amount)
    {
        hp = Mathf.Min(hp + amount, stats.maxHp);
        UpdateHearts();
    }

    IEnumerator Invincibility()
    {
        invincible = true;
        for (float t = 0; t < stats.invincibleTime; t += blinkInterval)
        {
            sr.enabled = !sr.enabled;          // мигание
            yield return new WaitForSeconds(blinkInterval);
        }
        sr.enabled = true;
        invincible = false;
    }

    void UpdateHearts()
    {
        for (int i = 0; i < hearts.Length; i++)
            hearts[i].enabled = i < hp;       // показываем столько сердец, сколько HP
    }
}