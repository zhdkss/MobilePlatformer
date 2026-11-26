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

- 2026-11-12  Playtest change: raised the player speed from 6 to 15 in the PlayerStats asset and checked it in Play mode.

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

- 2026-11-16  Agent mistake: the editor tool that sets up my data used `FindObjectsSortMode`, which this
  Unity version marks as obsolete, so the Console was full of warnings. I asked for a fix; the agent
  removed the argument from all 9 calls.
- 2026-11-16  My mistake: I ran "Assign Stats" and "Add LevelManager" before "Create Data Assets" and got
  "Run Data 1 first" / "No LevelData for scene Level2". The assets did not exist yet. I ran the menu items
  in the right order and it worked.
- 2026-11-19  Agent mistake: the new scripts and data assets were committed without their `.meta` files.
  On a fresh clone Unity would make new GUIDs and every reference in the scenes would turn into "Missing".
  I added all the `.meta` files and the `ItemStats` on the Gem and Heart prefabs in a new commit.
- 2026-11-19  Decision: the gem and the heart share one `Pickup` script and differ only by their
  `ItemStats` asset (score 10 / heal 1), so I can change the value without touching code.

- 2026-11-19 «Agent mistake: I was told to drag “any TMP text” into Timer Label, so the timer overwrote the Resume and Back to Menu button text with numbers. The agent now creates a dedicated TimerText and ignores button texts.»

- 2026-11-19  Live balance test: changed patrolSpeed in EnemyStats_Patroller from 2 to 10 in Play mode, and the enemy moved faster with no code edits.

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

- 2026-11-26  Agent mistake: again the new audio scripts and wav files were committed without `.meta` files. I found it
  with a local check before the pull request and committed all of them (plus `MainMixer` and `AudioConfig.asset`).
- 2026-11-26  Agent mistake: it did not tell me that the old `Music` object (a plain AudioSource) was still in the
  level scenes, so two music tracks would play at once. I deleted it from Level1, Level2, Level3_Boss and MainMenu.
- 2026-11-26  My mistake: I typed the exposed parameter names (`MusicVol`, `SFXVol`) into the mixer group names, so the
  "Audio 1" menu could not find the groups `Music` and `SFX`. I renamed the groups back and renamed the parameters.
- 2026-11-26  Mix: set the Music lowpass to 22000 Hz in `Normal` and 800 Hz in `Paused`, Master -6 dB in `Paused`.
