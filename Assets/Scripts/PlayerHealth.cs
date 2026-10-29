using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public int maxHp = 3;
    public Image[] hearts;            // 3 сердца из HUD
    public float invincibleTime = 1.5f;
    public float fallY = -10f;        // ниже этой высоты = упал в яму
    public AudioClip hurtSound;

    int hp;
    bool invincible;
    SpriteRenderer sr;

    void Start()
    {
        hp = maxHp;
        sr = GetComponent<SpriteRenderer>();
        UpdateHearts();
    }

    void Update()
    {
        if (transform.position.y < fallY) SceneLoader.Restart();
    }

    public void TakeDamage(int amount)
    {
        if (invincible) return;              // ещё мигаем — урон не проходит
        hp -= amount;
        AudioSource.PlayClipAtPoint(hurtSound, Camera.main.transform.position);
        UpdateHearts();
        if (hp <= 0) { SceneLoader.Restart(); return; }
        StartCoroutine(Invincibility());
    }

    public void Heal(int amount)
    {
        hp = Mathf.Min(hp + amount, maxHp);
        UpdateHearts();
    }

    IEnumerator Invincibility()
    {
        invincible = true;
        for (float t = 0; t < invincibleTime; t += 0.1f)
        {
            sr.enabled = !sr.enabled;          // мигание
            yield return new WaitForSeconds(0.1f);
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