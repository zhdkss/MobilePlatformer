using UnityEngine;

// Общая часть трёх типов врагов: поиск игрока, зрение (дистанция + угол + Raycast),
// мигание перед атакой. Само поведение (состояния) у каждого типа своё.
[RequireComponent(typeof(SpriteRenderer))]
public abstract class EnemyBase : MonoBehaviour
{
    public EnemyStats stats;                        // числа баланса этого типа врага

    [Header("Зрение")]
    public LayerMask obstacleMask;                  // стены и земля закрывают обзор

    [Header("Телеграф атаки")]
    public Color telegraphColor = Color.red;        // цвет мигания перед атакой
    public float flashSpeed = 10f;

    [Header("Спрайт")]
    public bool spriteFacesRight = true;            // куда смотрит картинка без flipX

    protected Transform player;
    protected SpriteRenderer sr;
    protected float stateTimer;                     // сколько секунд мы в текущем состоянии
    protected int facing = -1;                      // -1 влево, 1 вправо
    Color baseColor;

    protected virtual void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        baseColor = sr.color;
        GameObject p = GameObject.FindGameObjectWithTag("Player");  // один раз, не в Update
        if (p != null) player = p.transform;
        Face(facing);
    }

    // Видит ли враг игрока: дистанция, угол и луч без стен на пути
    protected bool CanSeePlayer()
    {
        if (player == null) return false;

        Vector2 toPlayer = player.position - transform.position;
        float dist = toPlayer.magnitude;
        if (dist > stats.viewRange) return false;

        float angle = Vector2.Angle(new Vector2(facing, 0f), toPlayer);
        if (angle > stats.viewAngle * 0.5f) return false;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, toPlayer.normalized, dist, obstacleMask);
        Debug.DrawLine(transform.position, player.position, hit.collider == null ? Color.yellow : Color.gray);
        return hit.collider == null;                // ни во что не упёрлись = видим
    }

    // Враг в кадре? Атаки из-за экрана запрещены
    protected bool IsOnScreen()
    {
        Vector3 v = Camera.main.WorldToViewportPoint(transform.position);
        return v.x > 0f && v.x < 1f && v.y > 0f && v.y < 1f;
    }

    protected void Face(int dir)
    {
        if (dir == 0) return;
        facing = dir > 0 ? 1 : -1;
        sr.flipX = spriteFacesRight ? facing < 0 : facing > 0;
    }

    protected void FacePlayer()
    {
        if (player != null) Face(player.position.x > transform.position.x ? 1 : -1);
    }

    protected void Telegraph()
    {
        sr.color = Color.Lerp(baseColor, telegraphColor, Mathf.PingPong(Time.time * flashSpeed, 1f));
    }

    protected void ResetColor() { sr.color = baseColor; }

    protected void PlayWindUpSound()
    {
        GameAudio.Play(Sfx.WindUp);
    }

    void OnDrawGizmosSelected()
    {
        if (stats == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, stats.viewRange);
    }
}
