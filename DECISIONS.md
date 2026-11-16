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

- 2026-11-12  Agent mistake: the Shooter got the same sprite as my old enemies (sheet index 18),
  so I could not tell them apart in the level. Changed it to the orange block (index 11).
- 2026-11-12  Agent mistake: default vision range was too long (7–9 units), enemies reacted before
  I could read them. I noticed it while testing and asked for shorter defaults (5–6).
- 2026-11-12  Found while testing: the hero could not jump on the moving platform. Cause: the
  platform was on the Default layer and the ground check only looks at the Ground layer. Moved the
  platform to Ground; the agent also made the hero ride with the platform.
- 2026-11-12  Controls felt hard on the phone. Asked the agent to improve `PlayerController`: stick
  dead zone, coyote time 0.12 s, jump buffer 0.15 s, short press = low jump. It also fixed my old
  bug where the jump sound played even in the air.

<!-- TODO (Dias): fill in after the playtest. Delete this comment.
- 2026-11-12  Playtest: <name> ... tries, <name> ... tries.
- 2026-11-12  Changed <number> from ... to ... because ...
- 2026-11-12  Changed <number> from ... to ... because ...
-->

## Week 11 — Assignment 11: data and saves

- 2026-11-16  Asked the agent (Claude Code) to move all balance numbers into ScriptableObjects and add
  a JSON save. It made `PlayerStats`, `EnemyStats`, `BossStats`, `ItemStats`, `LevelData` and rewrote
  `PlayerController`, `PlayerHealth`, `PlayerShooter`, `Enemy`, the three enemy scripts, `Boss` and
  `Pickup` to read from them. One `EnemyStats` asset per enemy type (Patroller, Shooter, Charger, Walker).
- 2026-11-16  Save: `SaveData` + `SaveSystem` (JsonUtility, `Application.persistentDataPath/save.json`,
  try/catch, defaults if the file is missing or broken). I told it NOT to use BinaryFormatter.
  `unlockedLevels` is a `List<int>`, because JsonUtility cannot save a Dictionary.
- 2026-11-16  Saves happen after a level (`LevelManager.Complete`), when the volume changes
  (`SaveSystem.SetVolume`) and in `OnApplicationPause(true)`.
- 2026-11-16  `LevelManager` reads a `LevelData` asset (spawns, time limit, gems needed, next level), so
  the same code runs all three levels. The boss level finishes through `Boss.TakeHit` -> `Complete`.

<!-- TODO (Dias): add what the agent got wrong and what you changed (3-5 lines), for example:
- 2026-11-16  Agent got wrong: ...  I changed: ...
- 2026-11-16  Live balance test: changed <number> in <asset> from ... to ..., game changed without code edits.
-->

## Week 12 — Assignment 12: sound pass

- 2026-11-16  Asked the agent (Claude Code) for a full sound layer. It wrote `AudioConfig` (all clips and
  mixer names in one asset in `Assets/Resources`), `AudioManager` (one object, `DontDestroyOnLoad`, created
  before the first scene so music does not restart on level load) and a static `GameAudio.Play(Sfx.X)`
  that all my scripts call instead of `AudioSource.PlayClipAtPoint`.
- 2026-11-16  10 sound events: jump, hit, pickup, enemy wind-up, enemy death, UI click, win, lose, shoot,
  boss hit. The wind-up sound plays at the start of the wind-up in `EnemyBase` and in the boss loop, so an
  attack can be heard before it lands. UI click is added to every `Button` when a scene loads.
- 2026-11-16  Two music tracks (menu + win, level) with a 1 s crossfade on two AudioSources.
- 2026-11-16  Volume: Music and SFX sliders are saved in `save.json` (0..1) and sent to the mixer as
  `Log10(value) * 20` dB. Pause calls the `Paused` snapshot (quieter, lowpass).
- 2026-11-16  Variation: `PlaySfx` picks a random pitch 0.92–1.08 for each repeated sound.
- 2026-11-16  Decision: removed the old `AudioClip` fields from player, pickup, enemy and boss scripts, so
  there is one place to change sounds. Replaced the old single `volume` in the save with `musicVolume` and `sfxVolume`.

<!-- TODO (Dias): add what the agent got wrong and what you changed (3-5 lines), after you test on the phone:
- 2026-11-xx  Agent got wrong: ...  I changed: ...
- 2026-11-xx  Mix: changed <sound> volume / pitch / mixer level from ... to ... because ...
-->
