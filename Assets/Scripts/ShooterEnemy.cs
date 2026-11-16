using UnityEngine;

// Стрелок: стоит на месте, увидел игрока — целится (красная линия), стреляет, перезаряжается.
// Watch -> Aim -> Shoot -> Cooldown -> Watch
public class ShooterEnemy : EnemyBase
{
    enum State { Watch, Aim, Shoot, Cooldown }

    [Header("Стрельба")]
    public GameObject projectilePrefab;   // Fireball: в нём уже есть Hazard и Rigidbody2D
    public Transform firePoint;
    public AudioClip shootSound;

    [Header("Линия прицела")]
    public LineRenderer aimLine;          // показывает, куда полетит выстрел

    State state = State.Watch;
    Vector2 aimDir;

    protected override void Start()
    {
        base.Start();
        if (aimLine) aimLine.enabled = false;
    }

    void Update()
    {
        stateTimer += Time.deltaTime;

        switch (state)
        {
            case State.Watch:    Watch();    break;
            case State.Aim:      Aim();      break;
            case State.Shoot:    Shoot();    break;
            case State.Cooldown: Cooldown(); break;
        }
    }

    void SetState(State next)
    {
        state = next;
        stateTimer = 0f;
        ResetColor();
        if (aimLine) aimLine.enabled = next == State.Aim;
    }

    void Watch()
    {
        FacePlayer();
        if (CanSeePlayer() && IsOnScreen())   // из-за экрана не стреляем
        {
            PlayWindUpSound();
            SetState(State.Aim);
        }
    }

    void Aim()
    {
        // игрок спрятался за стену — отменяем выстрел
        if (!CanSeePlayer()) { SetState(State.Watch); return; }

        FacePlayer();
        Telegraph();
        Vector3 from = firePoint ? firePoint.position : transform.position;
        aimDir = (player.position - from).normalized;

        if (aimLine)
        {
            aimLine.SetPosition(0, from);
            aimLine.SetPosition(1, player.position);
        }

        if (stateTimer >= stats.windUpTime) SetState(State.Shoot);
    }

    void Shoot()
    {
        Vector3 from = firePoint ? firePoint.position : transform.position;
        GameObject p = Instantiate(projectilePrefab, from, Quaternion.identity);
        p.GetComponent<Rigidbody2D>().linearVelocity = aimDir * stats.attackSpeed;
        Destroy(p, stats.projectileLife);
        if (shootSound) AudioSource.PlayClipAtPoint(shootSound, Camera.main.transform.position);

        SetState(State.Cooldown);
    }

    void Cooldown()
    {
        if (stateTimer >= stats.cooldown) SetState(State.Watch);
    }
}
