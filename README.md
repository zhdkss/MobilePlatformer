GEM RUNNER — Mobile Platformer (Midterm Project)
Game Development Fundamentals
Author: Abris Dias

ENGINE
Unity 6.6 (6000.6.0f1), 2D URP template
Unity Input System 1.20 (on-screen stick + on-screen buttons, keyboard works in parallel)
Target: iPhone via Unity Remote 5 / Android APK

GAME
Main Menu -> Level 1 -> Level 2 -> Level 3 (Boss) -> Win screen -> Main Menu
- Collect gems (counter in the HUD), avoid spikes and patrolling enemies
- Hero has 3 HP, blinks while invincible after a hit; hearts restore HP
- Death or falling into a pit restarts the level
- Enemies can be killed by jumping on them or by shooting
- Boss: HP bar, moves left-right, red warning flash before every shot,
  shoots faster below half HP (second phase)

CONTROLS
Joystick / A,D - move    JUMP button / Space - jump    FIRE button / X - shoot

SCRIPTS (Assets/Scripts)
SceneLoader, PlayerController, PlayerHealth, PlayerShooter, CoinCounter,
Pickup, Hazard, LevelExit, Enemy, Bullet, Boss

ASSETS AND LICENSES
- Kenney "Pixel Platformer" (tiles, characters, items, backgrounds)
  https://kenney.nl/assets/pixel-platformer — License: CC0 1.0
- Font "Press Start 2P" by CodeMan38
  https://fonts.google.com/specimen/Press+Start+2P — License: SIL Open Font License 1.1
- Sound effects and background music (Assets/Audio):
  generated chiptune sounds made for this project — no license restrictions
- Joystick and button images (Assets/UI): pixel-art images made for this project
  — no license restrictions

AI USAGE
An AI assistant (Claude) was used during development. See AI_LOG.txt.
