using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class Boss : MonoBehaviour
{
    public BossStats stats;            // числа баланса босса
    public Slider hpBar;
    public GameObject fireballPrefab;
    public Transform firePoint;
    public Transform player;

    int hp;
    float startX;

    SpriteRenderer sr;

    void Start()
    {
        startX = transform.position.x;
        hp = stats.maxHp;
        sr = GetComponent<SpriteRenderer>();
        hpBar.value = 1f;
        StartCoroutine(AttackLoop());
    }
    
    void Update()
    {
        // босс ходит влево-вправо вокруг стартовой точки
        float x = startX + Mathf.Sin(Time.time * stats.moveSpeed) * stats.moveRange;
        transform.position = new Vector3(x, transform.position.y, 0);
    }

    IEnumerator AttackLoop()
    {
        while (hp > 0)
        {
            // Вторая фаза: ниже половины HP стреляет чаще
            float delay = hp > stats.maxHp / 2 ? stats.shootDelay : stats.enragedShootDelay;
            sr.color = Color.red;
            GameAudio.Play(Sfx.WindUp);              // предупреждение перед выстрелом
            yield return new WaitForSeconds(stats.windUpTime);
            sr.color = Color.white;
            Shoot();
            yield return new WaitForSeconds(delay);
        }
    }

    void Shoot()
    {
        Vector2 dir = (player.position - firePoint.position).normalized;
        GameObject f = Instantiate(fireballPrefab, firePoint.position, Quaternion.identity);
        f.GetComponent<Rigidbody2D>().linearVelocity = dir * stats.fireballSpeed;
        Destroy(f, stats.fireballLife);
    }

    public void TakeHit()
    {
        hp--;
        GameAudio.Play(Sfx.BossHit);
        hpBar.value = (float)hp / stats.maxHp;
        if (hp <= 0)
        {
            // победа над боссом = уровень пройден: рекорд и сохранение в LevelManager
            if (LevelManager.Instance != null) LevelManager.Instance.Complete();
            else SceneLoader.LoadNext();
        }

    }

}