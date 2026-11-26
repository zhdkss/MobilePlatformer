using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum Sfx { Jump, Hit, Pickup, WindUp, Death, Click, Win, Lose, Shoot, BossHit }

// Короткий вызов для остальных скриптов: GameAudio.Play(Sfx.Jump)
public static class GameAudio
{
    public static void Play(Sfx sfx)
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySfx(sfx);
    }

    public static void SetPaused(bool paused)
    {
        if (AudioManager.Instance != null) AudioManager.Instance.SetPaused(paused);
    }
}

// Один на всю игру. Создаётся сам до первой сцены, не уничтожается при загрузке уровней,
// поэтому музыка не начинается заново на каждом уровне.
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    const int SfxVoices = 8;

    static AudioConfig pendingConfig;

    AudioConfig config;
    AudioSource[] sfxVoices;
    AudioSource[] musicSources;
    int activeMusic;
    int nextVoice;
    AudioClip currentTrack;
    Coroutine fade;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Boot()
    {
        if (Instance != null) return;

        pendingConfig = Resources.Load<AudioConfig>("AudioConfig");
        if (pendingConfig == null)
        {
            Debug.LogWarning("AudioConfig not found in Assets/Resources: the game has no sound. Use Tools > Gem Runner > Audio 1.");
            return;
        }
        new GameObject("AudioManager").AddComponent<AudioManager>();
    }

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        config = pendingConfig;
        DontDestroyOnLoad(gameObject);

        sfxVoices = new AudioSource[SfxVoices];
        for (int i = 0; i < SfxVoices; i++)
            sfxVoices[i] = MakeSource("Sfx" + i, config.sfxGroup, false);

        musicSources = new[] { MakeSource("MusicA", config.musicGroup, true), MakeSource("MusicB", config.musicGroup, true) };

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Start()
    {
        // SetFloat в Awake иногда не срабатывает, поэтому громкость ставим в Start
        ApplyVolumes();
        OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
    }

    void OnDestroy()
    {
        if (Instance == this) SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    AudioSource MakeSource(string name, AudioMixerGroup group, bool loop)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(transform, false);
        AudioSource src = go.AddComponent<AudioSource>();
        src.playOnAwake = false;
        src.loop = loop;
        src.spatialBlend = 0f;                    // 2D-звук
        src.outputAudioMixerGroup = group;        // каждый источник идёт в свою группу миксера
        return src;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ApplyVolumes();
        SetPaused(false);
        PlayMusic(IsMenuScene(scene.name) ? config.menuMusic : config.levelMusic);

        // клик на каждой кнопке сцены, включая скрытые панели
        foreach (Button b in FindObjectsByType<Button>(FindObjectsInactive.Include))
        {
            b.onClick.RemoveListener(PlayClick);   // чтобы не добавить дважды
            b.onClick.AddListener(PlayClick);
        }
    }

    static bool IsMenuScene(string sceneName)
    {
        return sceneName == "MainMenu" || sceneName == "Win";
    }

    void PlayClick() { PlaySfx(Sfx.Click); }

    // ---------- звуки ----------

    public void PlaySfx(Sfx sfx)
    {
        AudioClip clip = config.Get(sfx);
        if (clip == null) return;

        AudioSource voice = sfxVoices[nextVoice];
        nextVoice = (nextVoice + 1) % SfxVoices;

        voice.pitch = Random.Range(config.pitchMin, config.pitchMax);   // вариация, чтобы не звучало как робот
        voice.clip = clip;
        voice.Play();
    }

    // ---------- музыка ----------

    public void PlayMusic(AudioClip track)
    {
        if (track == null || track == currentTrack) return;    // уже играет: не перезапускаем
        currentTrack = track;

        AudioSource from = musicSources[activeMusic];
        activeMusic = 1 - activeMusic;
        AudioSource to = musicSources[activeMusic];

        to.clip = track;
        to.volume = 0f;
        to.Play();

        if (fade != null) StopCoroutine(fade);
        fade = StartCoroutine(Crossfade(from, to));
    }

    // Старая дорожка затихает, новая нарастает за crossfadeTime секунд, без щелчка.
    // unscaledDeltaTime: работает и когда игра на паузе (timeScale = 0)
    IEnumerator Crossfade(AudioSource from, AudioSource to)
    {
        float startFrom = from.isPlaying ? from.volume : 0f;
        for (float t = 0f; t < config.crossfadeTime; t += Time.unscaledDeltaTime)
        {
            float k = t / config.crossfadeTime;
            to.volume = k;
            from.volume = startFrom * (1f - k);
            yield return null;
        }
        to.volume = 1f;
        from.volume = 0f;
        from.Stop();
    }

    // ---------- громкость и пауза ----------

    // Ползунок 0..1 -> децибелы: 20 * log10(value). Линейно было бы слишком тихо на середине.
    public static float ToDecibels(float linear)
    {
        return linear <= 0.0001f ? -80f : Mathf.Log10(linear) * 20f;
    }

    public void ApplyVolumes()
    {
        if (config == null || config.mixer == null) return;
        config.mixer.SetFloat(config.musicVolumeParam, ToDecibels(SaveSystem.Data.musicVolume));
        config.mixer.SetFloat(config.sfxVolumeParam, ToDecibels(SaveSystem.Data.sfxVolume));
    }

    // Snapshot Paused: тише общая громкость и lowpass на музыке (настроено в миксере)
    public void SetPaused(bool paused)
    {
        if (config == null || config.mixer == null) return;
        AudioMixerSnapshot snap = config.mixer.FindSnapshot(paused ? config.pausedSnapshot : config.normalSnapshot);
        if (snap != null) snap.TransitionTo(config.snapshotFade);
    }
}
