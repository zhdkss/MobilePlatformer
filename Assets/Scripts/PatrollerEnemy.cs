using UnityEngine;

// Патрульный: ходит между точками, увидел игрока — гонится, подошёл — замах и рывок.
// Patrol -> Chase -> WindUp -> Lunge -> Cooldown -> Chase / Patrol
public class PatrollerEnemy : EnemyBase
{
    enum State { Patrol, Chase, WindUp, Lunge, Cooldown }

    [Header("Патруль")]
    public Transform[] waypoints;        // точки маршрута (дочерние объекты)
    public float patrolSpeed = 2f;
    public float waitAtPoint = 0.5f;

    [Header("Погоня")]
    public float chaseSpeed = 3.5f;
    public float loseSightTime = 1.5f;   // сколько секунд ищет, потеряв игрока
    public float chaseLeash = 2f;        // насколько можно выйти за крайние точки

    [Header("Атака")]
    public float attackRange = 1.6f;
    public float windUpTime = 0.5f;      // замах: враг стоит и мигает
    public float lungeSpeed = 9f;
    public float lungeTime = 0.25f;
    public float cooldown = 1f;

    State state = State.Patrol;
    Vector2[] points;                    // позиции точек в мире, запомнены на старте
    int pointIndex;
    float minX, maxX;
    float waitTimer, lostTimer;

    protected override void Start()
    {
        base.Start();

        // Точки — дети врага и двигались бы вместе с ним, поэтому запоминаем их позиции один раз
        if (waypoints != null && waypoints.Length >= 2)
        {
            points = new Vector2[waypoints.Length];
            for (int i = 0; i < waypoints.Length; i++) points[i] = waypoints[i].position;
        }
        else
        {
            points = new Vector2[] { transform.position + Vector3.left * 3f, transform.position + Vector3.right * 3f };
        }

        minX = maxX = points[0].x;
        foreach (Vector2 p in points)
        {
            minX = Mathf.Min(minX, p.x);
            maxX = Mathf.Max(maxX, p.x);
        }
    }

    void Update()
    {
        stateTimer += Time.deltaTime;

        switch (state)
        {
            case State.Patrol:   Patrol();   break;
            case State.Chase:    Chase();    break;
            case State.WindUp:   WindUp();   break;
            case State.Lunge:    Lunge();    break;
            case State.Cooldown: Cooldown(); break;
        }
    }

    void SetState(State next)
    {
        state = next;
        stateTimer = 0f;
        ResetColor();
    }

    void Patrol()
    {
        if (CanSeePlayer()) { lostTimer = 0f; SetState(State.Chase); return; }

        float targetX = points[pointIndex].x;
        if (Mathf.Abs(targetX - transform.position.x) < 0.05f)
        {
            // дошли до точки: немного постоять и идти к следующей
            waitTimer += Time.deltaTime;
            if (waitTimer >= waitAtPoint)
            {
                waitTimer = 0f;
                pointIndex = (pointIndex + 1) % points.Length;
            }
            return;
        }

        Face(targetX > transform.position.x ? 1 : -1);
        MoveToX(targetX, patrolSpeed);
    }

    void Chase()
    {
        if (CanSeePlayer()) lostTimer = 0f;
        else lostTimer += Time.deltaTime;

        if (lostTimer >= loseSightTime) { SetState(State.Patrol); return; }

        FacePlayer();
        float dx = Mathf.Abs(player.position.x - transform.position.x);
        float dy = Mathf.Abs(player.position.y - transform.position.y);

        if (dx <= attackRange && dy < 1.5f && IsOnScreen())
        {
            PlayWindUpSound();
            SetState(State.WindUp);
            return;
        }

        // гонимся, но не дальше поводка, чтобы не убежать с платформы
        float targetX = Mathf.Clamp(player.position.x, minX - chaseLeash, maxX + chaseLeash);
        MoveToX(targetX, chaseSpeed);
    }

    void WindUp()
    {
        Telegraph();                                    // игрок видит замах и успевает отойти
        if (stateTimer >= windUpTime) SetState(State.Lunge);
    }

    void Lunge()
    {
        // направление рывка зафиксировано в момент замаха (facing в WindUp не менялся)
        float x = transform.position.x + facing * lungeSpeed * Time.deltaTime;
        x = Mathf.Clamp(x, minX - chaseLeash, maxX + chaseLeash);
        transform.position = new Vector3(x, transform.position.y, transform.position.z);

        if (stateTimer >= lungeTime) SetState(State.Cooldown);
    }

    void Cooldown()
    {
        if (stateTimer < cooldown) return;
        lostTimer = 0f;
        SetState(CanSeePlayer() ? State.Chase : State.Patrol);
    }

    void MoveToX(float targetX, float speed)
    {
        float x = Mathf.MoveTowards(transform.position.x, targetX, speed * Time.deltaTime);
        transform.position = new Vector3(x, transform.position.y, transform.position.z);
    }
}
