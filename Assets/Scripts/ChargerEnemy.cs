using UnityEngine;

// Таран: ждёт, заметил игрока на своей высоте — трясётся и мигает, потом несётся по прямой.
// После рывка оглушён — это окно, чтобы прыгнуть на него или выстрелить.
// Idle -> WindUp -> Charge -> Recover -> Idle
public class ChargerEnemy : EnemyBase
{
    enum State { Idle, WindUp, Charge, Recover }

    [Header("Замах (только вид)")]
    public float shakeAmount = 0.05f;
    public float shakeSpeed = 60f;

    [Header("Стены и обрывы")]
    public float wallCheckDistance = 0.6f;
    public float groundCheckDistance = 1.5f;

    State state = State.Idle;
    Vector3 windUpPos;
    float travelled;

    void Update()
    {
        stateTimer += Time.deltaTime;

        switch (state)
        {
            case State.Idle:    Idle();    break;
            case State.WindUp:  WindUp();  break;
            case State.Charge:  Charge();  break;
            case State.Recover: Recover(); break;
        }
    }

    void SetState(State next)
    {
        state = next;
        stateTimer = 0f;
        ResetColor();
    }

    void Idle()
    {
        if (player == null) return;

        float dy = Mathf.Abs(player.position.y - transform.position.y);
        if (dy > stats.heightTolerance) return;

        FacePlayer();
        if (CanSeePlayer() && IsOnScreen())
        {
            windUpPos = transform.position;
            PlayWindUpSound();
            SetState(State.WindUp);          // направление рывка = facing, дальше не меняется
        }
    }

    void WindUp()
    {
        Telegraph();
        // дрожим на месте, чтобы замах было видно
        float offset = Mathf.Sin(Time.time * shakeSpeed) * shakeAmount;
        transform.position = windUpPos + new Vector3(offset, 0f, 0f);

        if (stateTimer >= stats.windUpTime)
        {
            transform.position = windUpPos;
            travelled = 0f;
            SetState(State.Charge);
        }
    }

    void Charge()
    {
        Vector2 dir = new Vector2(facing, 0f);

        // стена впереди или обрыв под ногами — останавливаемся
        bool wall = Physics2D.Raycast(transform.position, dir, wallCheckDistance, obstacleMask);
        Vector2 ahead = (Vector2)transform.position + dir * wallCheckDistance;
        bool ground = Physics2D.Raycast(ahead, Vector2.down, groundCheckDistance, obstacleMask);

        if (wall || !ground || travelled >= stats.attackDistance)
        {
            SetState(State.Recover);
            return;
        }

        float step = stats.attackSpeed * Time.deltaTime;
        transform.position += new Vector3(facing * step, 0f, 0f);
        travelled += step;
    }

    void Recover()
    {
        if (stateTimer >= stats.cooldown) SetState(State.Idle);
    }
}
