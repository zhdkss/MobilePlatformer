using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class Boss : MonoBehaviour
{
    public int maxHp = 5;
    public Slider hpBar;
    public GameObject fireballPrefab;
    public Transform firePoint;
    public Transform player;
    public float fireballSpeed = 6f;
    public float moveSpeed = 2f;
    public float moveRange = 3f;
    public AudioClip hitSound;

    int hp;
    float startX;

    SpriteRenderer sr;

    void Start()
    {
        startX = transform.position.x;
        hp = maxHp;
        sr = GetComponent<SpriteRenderer>();
        hpBar.value = 1f;
        StartCoroutine(AttackLoop());
    }
    
    void Update()
    {
        // босс ходит влево-вправо вокруг стартовой точки
        float x = startX + Mathf.Sin(Time.time * moveSpeed) * moveRange;
        transform.position = new Vector3(x, transform.position.y, 0);
    }

    IEnumerator AttackLoop()
    {
        while (hp > 0)
        {
            // Вторая фаза: ниже половины HP стреляет чаще
            float delay = hp > maxHp / 2 ? 2f : 1f;
            sr.color = Color.red;              // предупреждение перед выстрелом
            yield return new WaitForSeconds(0.4f);
            sr.color = Color.white;
            Shoot();
            yield return new WaitForSeconds(delay);
        }
    }

    void Shoot()
    {
        Vector2 dir = (player.position - firePoint.position).normalized;
        GameObject f = Instantiate(fireballPrefab, firePoint.position, Quaternion.identity);
        f.GetComponent<Rigidbody2D>().linearVelocity = dir * fireballSpeed;
        Destroy(f, 4f);
    }

    public void TakeHit()
    {
        hp--;
        AudioSource.PlayClipAtPoint(hitSound, Camera.main.transform.position);
        hpBar.value = (float)hp / maxHp;
        if (hp <= 0) SceneLoader.LoadNext();

    }

}