# DECISIONS — Gem Runner

What I asked the AI agent, what went wrong, what I changed and why.

## Week 10 — Assignment 10: three enemy types

- 2026-11-12  Asked the agent (Claude Code) for three enemy types with state machines for my 2D
  platformer. It wrote `EnemyBase` (shared sight check: distance + angle + Raycast against the
  Ground layer) and three scripts: `PatrollerEnemy` (Patrol → Chase → WindUp → Lunge → Cooldown),
  `ShooterEnemy` (Watch → Aim → Shoot → Cooldown) and `ChargerEnemy` (Idle → WindUp → Charge → Recover).
- 2026-11-12  Decision: kept my old `Enemy.cs` on every new enemy for stomp / bullet kill / contact
  damage, so the new scripts only add behaviour and `Bullet.cs` did not need changes.
- 2026-11-12  Decision: enemies move with `transform`, not Rigidbody2D, so the Patroller is limited
  by a "leash" around its waypoints and the Charger stops on a wall or a ledge (two Raycasts).
- 2026-11-12  Fairness: every attack has a wind-up (Patroller 0.5 s, Shooter 0.6 s with a red aim
  line, Charger 0.7 s with shaking) and a cooldown; no enemy attacks while it is off-screen.

<!-- TODO (Dias): fill in after testing in Unity and after the playtest. Delete this comment.
- 2026-11-12  What the agent got wrong and how I fixed it: ...
- 2026-11-12  Playtest: <name> ... tries, <name> ... tries.
- 2026-11-12  Changed <number> from ... to ... because ...
- 2026-11-12  Changed <number> from ... to ... because ...
-->
