using UnityEngine;
using UnityEngine.Audio;

// Все звуки, музыка и имена параметров миксера в одном ассете.
// Лежит в Assets/Resources/AudioConfig.asset, поэтому AudioManager находит его сам в любой сцене.
[CreateAssetMenu(menuName = "Gem Runner/Audio Config")]
public class AudioConfig : ScriptableObject
{
    [Header("Миксер")]
    public AudioMixer mixer;
    public AudioMixerGroup musicGroup;
    public AudioMixerGroup sfxGroup;
    public string musicVolumeParam = "MusicVol";   // exposed-параметры группы Music и SFX
    public string sfxVolumeParam = "SFXVol";
    public string normalSnapshot = "Normal";
    public string pausedSnapshot = "Paused";
    public float snapshotFade = 0.2f;

    [Header("Музыка")]
    public AudioClip menuMusic;                    // MainMenu и Win
    public AudioClip levelMusic;                   // все уровни
    public float crossfadeTime = 1f;

    [Header("Звуки (8+ событий)")]
    public AudioClip jump;
    public AudioClip hit;
    public AudioClip pickup;
    public AudioClip windUp;                       // замах врага: играет в начале замаха
    public AudioClip death;
    public AudioClip click;
    public AudioClip win;
    public AudioClip lose;
    public AudioClip shoot;
    public AudioClip bossHit;

    [Header("Вариация")]
    public float pitchMin = 0.92f;                 // повторяющиеся звуки чуть меняют высоту
    public float pitchMax = 1.08f;

    public AudioClip Get(Sfx sfx)
    {
        switch (sfx)
        {
            case Sfx.Jump: return jump;
            case Sfx.Hit: return hit;
            case Sfx.Pickup: return pickup;
            case Sfx.WindUp: return windUp;
            case Sfx.Death: return death;
            case Sfx.Click: return click;
            case Sfx.Win: return win;
            case Sfx.Lose: return lose;
            case Sfx.Shoot: return shoot;
            case Sfx.BossHit: return bossHit;
        }
        return null;
    }
}
