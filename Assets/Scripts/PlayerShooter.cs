using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooter : MonoBehaviour
{
    public InputActionReference attackAction;   // Player/Attack
    public GameObject bulletPrefab;
    public Transform firePoint;
    public PlayerStats stats;                   // скорость и время жизни пули

    PlayerController controller;

    void Awake() { controller = GetComponent<PlayerController>(); }

    void OnEnable()
    {
        attackAction.action.Enable();
        attackAction.action.performed += Shoot;
    }

    void OnDisable() { attackAction.action.performed -= Shoot; }

    void Shoot(InputAction.CallbackContext ctx)
    {
        float dir = controller.FacingLeft ? -1f : 1f;
        GameObject b = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        b.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(dir * stats.bulletSpeed, 0);
        Destroy(b, stats.bulletLife);
        GameAudio.Play(Sfx.Shoot);
    }
}