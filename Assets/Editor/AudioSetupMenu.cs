using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;

// Tools > Gem Runner > Audio 1: создаёт Assets/Resources/AudioConfig.asset
// и сам находит в Assets/Audio все клипы по именам файлов.
public static class AudioSetupMenu
{
    const string Dir = "Assets/Resources";
    const string Path = Dir + "/AudioConfig.asset";

    [MenuItem("Tools/Gem Runner/Audio 1 - Create or Update AudioConfig")]
    static void CreateConfig()
    {
        if (!AssetDatabase.IsValidFolder(Dir)) AssetDatabase.CreateFolder("Assets", "Resources");

        AudioConfig cfg = AssetDatabase.LoadAssetAtPath<AudioConfig>(Path);
        if (cfg == null)
        {
            cfg = ScriptableObject.CreateInstance<AudioConfig>();
            AssetDatabase.CreateAsset(cfg, Path);
        }

        cfg.menuMusic = Clip("menu_music");
        cfg.levelMusic = Clip("music");
        cfg.jump = Clip("jump");
        cfg.hit = Clip("hurt");
        cfg.pickup = Clip("pickup");
        cfg.windUp = Clip("windup");
        cfg.death = Clip("death");
        cfg.click = Clip("click");
        cfg.win = Clip("win");
        cfg.lose = Clip("lose");
        cfg.shoot = Clip("shoot");
        cfg.bossHit = Clip("bosshit");

        // миксер ищем автоматически, если он уже создан
        string[] mixers = AssetDatabase.FindAssets("t:AudioMixer");
        if (mixers.Length > 0)
        {
            AudioMixer mixer = AssetDatabase.LoadAssetAtPath<AudioMixer>(AssetDatabase.GUIDToAssetPath(mixers[0]));
            cfg.mixer = mixer;
            AudioMixerGroup[] music = mixer.FindMatchingGroups("Music");
            AudioMixerGroup[] sfx = mixer.FindMatchingGroups("SFX");
            if (music.Length > 0) cfg.musicGroup = music[0];
            if (sfx.Length > 0) cfg.sfxGroup = sfx[0];
        }
        else
        {
            Debug.LogWarning("No AudioMixer found. Create one (Assets > Create > Audio > Audio Mixer), then run this menu again.");
        }

        EditorUtility.SetDirty(cfg);
        AssetDatabase.SaveAssets();
        Selection.activeObject = cfg;
        Debug.Log("AudioConfig ready: " + Path);
    }

    static AudioClip Clip(string name)
    {
        AudioClip clip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/" + name + ".wav");
        if (clip == null) Debug.LogWarning("Clip not found: Assets/Audio/" + name + ".wav");
        return clip;
    }
}
