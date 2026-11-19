using System.IO;
using TMPro;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Tools > Gem Runner: создаёт ассеты данных с текущими числами игры,
// подключает их в открытой сцене и собирает UI выбора уровня в главном меню.
public static class DataSetupMenu
{
    const string Dir = "Assets/Data";

    // ---------- 1. ассеты ----------

    [MenuItem("Tools/Gem Runner/Data 1 - Create Data Assets")]
    static void CreateAssets()
    {
        if (!AssetDatabase.IsValidFolder(Dir)) AssetDatabase.CreateFolder("Assets", "Data");

        Make<PlayerStats>("PlayerStats");   // значения по умолчанию = то, что было в коде

        EnemyStats walker = Make<EnemyStats>("EnemyStats_Walker");        // старый Enemy.cs
        walker.stompBounce = 10f; walker.contactDamage = 1;

        EnemyStats patroller = Make<EnemyStats>("EnemyStats_Patroller");
        patroller.viewRange = 4.5f; patroller.viewAngle = 120f;
        patroller.patrolSpeed = 2f; patroller.waitAtPoint = 0.5f; patroller.chaseSpeed = 3.5f;
        patroller.loseSightTime = 1.5f; patroller.chaseLeash = 2f; patroller.attackRange = 1.6f;
        patroller.windUpTime = 0.5f; patroller.attackSpeed = 9f; patroller.attackDistance = 2.25f;
        patroller.cooldown = 1f;

        EnemyStats shooter = Make<EnemyStats>("EnemyStats_Shooter");
        shooter.viewRange = 6f; shooter.viewAngle = 360f;
        shooter.windUpTime = 0.6f; shooter.attackSpeed = 6f; shooter.projectileLife = 4f; shooter.cooldown = 1.6f;

        EnemyStats charger = Make<EnemyStats>("EnemyStats_Charger");
        charger.viewRange = 5f; charger.viewAngle = 360f; charger.heightTolerance = 1.2f;
        charger.windUpTime = 0.7f; charger.attackSpeed = 11f; charger.attackDistance = 7f; charger.cooldown = 1.2f;

        Make<BossStats>("BossStats");

        ItemStats gem = Make<ItemStats>("ItemStats_Gem");
        gem.scoreValue = 10; gem.healAmount = 0;
        ItemStats heart = Make<ItemStats>("ItemStats_Heart");
        heart.scoreValue = 0; heart.healAmount = 1;

        LevelData l3 = MakeLevel("Level_03", 3, "LEVEL 3", "Level3_Boss", 180f, 0, null);
        LevelData l2 = MakeLevel("Level_02", 2, "LEVEL 2", "Level2", 150f, 4, l3);
        MakeLevel("Level_01", 1, "LEVEL 1", "Level1", 120f, 3, l2);

        foreach (Object o in new Object[] { walker, patroller, shooter, charger, gem, heart })
            EditorUtility.SetDirty(o);
        AssetDatabase.SaveAssets();
        Debug.Log("Data assets are in " + Dir);
    }

    static T Make<T>(string name) where T : ScriptableObject
    {
        string path = Dir + "/" + name + ".asset";
        T existing = AssetDatabase.LoadAssetAtPath<T>(path);
        if (existing != null) return existing;          // уже есть: ваши правки не затираем
        T asset = ScriptableObject.CreateInstance<T>();
        AssetDatabase.CreateAsset(asset, path);
        return asset;
    }

    static LevelData MakeLevel(string name, int number, string title, string scene, float time, int gems, LevelData next)
    {
        LevelData d = Make<LevelData>(name);
        d.levelNumber = number; d.levelName = title; d.sceneName = scene;
        d.timeLimit = time; d.gemsToExit = gems; d.nextLevel = next;
        EditorUtility.SetDirty(d);
        return d;
    }

    static T Load<T>(string name) where T : Object
    {
        return AssetDatabase.LoadAssetAtPath<T>(Dir + "/" + name + ".asset");
    }

    // ---------- 2. подключить ассеты в открытой сцене ----------

    [MenuItem("Tools/Gem Runner/Data 2 - Assign Stats In Open Scene")]
    static void AssignStats()
    {
        PlayerStats player = Load<PlayerStats>("PlayerStats");
        if (player == null) { Debug.LogError("Run 'Data 1 - Create Data Assets' first."); return; }

        foreach (var c in Object.FindObjectsByType<PlayerController>()) Set(c, "stats", player);
        foreach (var c in Object.FindObjectsByType<PlayerHealth>()) Set(c, "stats", player);
        foreach (var c in Object.FindObjectsByType<PlayerShooter>()) Set(c, "stats", player);
        foreach (var c in Object.FindObjectsByType<Boss>()) Set(c, "stats", Load<BossStats>("BossStats"));

        foreach (var c in Object.FindObjectsByType<PatrollerEnemy>()) AssignEnemy(c, "Patroller");
        foreach (var c in Object.FindObjectsByType<ShooterEnemy>()) AssignEnemy(c, "Shooter");
        foreach (var c in Object.FindObjectsByType<ChargerEnemy>()) AssignEnemy(c, "Charger");

        foreach (var e in Object.FindObjectsByType<Enemy>())
            if (e.GetComponent<EnemyBase>() == null) Set(e, "stats", Load<EnemyStats>("EnemyStats_Walker"));

        foreach (var p in Object.FindObjectsByType<Pickup>())
            Set(p, "stats", Load<ItemStats>(p.isHeart ? "ItemStats_Heart" : "ItemStats_Gem"));

        // префабы камня и сердца: новые копии сразу с данными
        foreach (string prefab in new[] { "Gem", "Heart" })
        {
            string path = "Assets/Prefabs/" + prefab + ".prefab";
            GameObject root = PrefabUtility.LoadPrefabContents(path);
            Pickup pk = root.GetComponent<Pickup>();
            if (pk != null) Set(pk, "stats", Load<ItemStats>(pk.isHeart ? "ItemStats_Heart" : "ItemStats_Gem"));
            PrefabUtility.SaveAsPrefabAsset(root, path);
            PrefabUtility.UnloadPrefabContents(root);
        }

        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        Debug.Log("Stats assigned in " + SceneManager.GetActiveScene().name + ". Save the scene (Ctrl+S).");
    }

    static void AssignEnemy(EnemyBase enemy, string kind)
    {
        EnemyStats stats = Load<EnemyStats>("EnemyStats_" + kind);
        Set(enemy, "stats", stats);
        Enemy touch = enemy.GetComponent<Enemy>();       // касание и прыжок сверху
        if (touch != null) Set(touch, "stats", stats);
    }

    static void Set(Object target, string field, Object value)
    {
        SerializedObject so = new SerializedObject(target);
        SerializedProperty p = so.FindProperty(field);
        if (p == null) return;
        p.objectReferenceValue = value;
        so.ApplyModifiedProperties();
    }

    // ---------- 3. LevelManager в уровне ----------

    [MenuItem("Tools/Gem Runner/Data 3 - Add LevelManager To Open Scene")]
    static void AddLevelManager()
    {
        string scene = SceneManager.GetActiveScene().name;
        LevelData data = null;
        foreach (string name in new[] { "Level_01", "Level_02", "Level_03" })
        {
            LevelData d = Load<LevelData>(name);
            if (d != null && d.sceneName == scene) data = d;
        }
        if (data == null) { Debug.LogError("No LevelData for scene " + scene); return; }

        LevelManager lm = Object.FindAnyObjectByType<LevelManager>();
        if (lm == null) lm = new GameObject("LevelManager").AddComponent<LevelManager>();
        Set(lm, "data", data);
        Selection.activeGameObject = lm.gameObject;
        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        Debug.Log("LevelManager uses " + data.name + ". Drag a TMP text into 'Timer Label' to show the timer. Save the scene.");
    }

    // ---------- 4. UI выбора уровня в MainMenu ----------

    [MenuItem("Tools/Gem Runner/Data 4 - Build Menu UI (MainMenu scene)")]
    static void BuildMenuUI()
    {
        Canvas canvas = Object.FindAnyObjectByType<Canvas>();
        if (canvas == null || SceneManager.GetActiveScene().name != "MainMenu")
        { Debug.LogError("Open the MainMenu scene first."); return; }

        MainMenuController ctrl = Object.FindAnyObjectByType<MainMenuController>();
        if (ctrl == null) ctrl = new GameObject("MenuController").AddComponent<MainMenuController>();

        LevelData[] levels = { Load<LevelData>("Level_01"), Load<LevelData>("Level_02"), Load<LevelData>("Level_03") };
        Transform root = canvas.transform;

        // нижний ряд кнопок главного экрана
        Button levels_ = MakeButton(root, "LevelsButton", "LEVELS", new Vector2(0, -400), new Vector2(460, 140));
        UnityEventTools.AddPersistentListener(levels_.onClick, ctrl.OpenLevelSelect);
        Button reset = MakeButton(root, "ResetButton", "RESET", new Vector2(700, -400), new Vector2(360, 140));
        UnityEventTools.AddPersistentListener(reset.onClick, ctrl.AskReset);

        TMP_Text best = MakeText(root, "BestScore", "BEST SCORE: 0", new Vector2(0, -560), new Vector2(900, 90), 44);
        Slider volume = MakeSlider(root, "VolumeSlider", new Vector2(-640, -400), new Vector2(420, 60));
        UnityEventTools.AddPersistentListener(volume.onValueChanged, ctrl.OnVolumeChanged);

        // панель выбора уровня
        GameObject select = MakePanel(root, "LevelSelectPanel");
        Button[] lb = new Button[3];
        TMP_Text[] ll = new TMP_Text[3];
        for (int i = 0; i < 3; i++)
        {
            lb[i] = MakeButton(select.transform, "Level" + (i + 1) + "Button", "LEVEL", new Vector2(0, 200 - i * 180), new Vector2(520, 140));
            ll[i] = lb[i].GetComponentInChildren<TMP_Text>();
            UnityEventTools.AddIntPersistentListener(lb[i].onClick, ctrl.PlayLevel, i);
        }
        Button back = MakeButton(select.transform, "BackButton", "BACK", new Vector2(0, -360), new Vector2(520, 140));
        UnityEventTools.AddPersistentListener(back.onClick, ctrl.CloseLevelSelect);

        // панель подтверждения сброса
        GameObject confirm = MakePanel(root, "ConfirmPanel");
        MakeText(confirm.transform, "ConfirmText", "RESET ALL PROGRESS?", new Vector2(0, 160), new Vector2(1300, 120), 56);
        Button yes = MakeButton(confirm.transform, "YesButton", "YES", new Vector2(-260, -40), new Vector2(400, 140));
        UnityEventTools.AddPersistentListener(yes.onClick, ctrl.ConfirmReset);
        Button no = MakeButton(confirm.transform, "NoButton", "NO", new Vector2(260, -40), new Vector2(400, 140));
        UnityEventTools.AddPersistentListener(no.onClick, ctrl.CancelReset);

        SerializedObject so = new SerializedObject(ctrl);
        Fill(so.FindProperty("levels"), levels);
        Fill(so.FindProperty("levelButtons"), lb);
        Fill(so.FindProperty("levelLabels"), ll);
        so.FindProperty("bestScoreLabel").objectReferenceValue = best;
        so.FindProperty("volumeSlider").objectReferenceValue = volume;
        so.FindProperty("levelSelectPanel").objectReferenceValue = select;
        so.FindProperty("confirmPanel").objectReferenceValue = confirm;
        so.ApplyModifiedProperties();

        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        Debug.Log("Menu UI built. Move the buttons where you like, then save the scene (Ctrl+S).");
    }

    static void Fill(SerializedProperty list, Object[] items)
    {
        list.arraySize = items.Length;
        for (int i = 0; i < items.Length; i++) list.GetArrayElementAtIndex(i).objectReferenceValue = items[i];
    }

    static GameObject MakePanel(Transform parent, string name)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(Image));
        go.transform.SetParent(parent, false);
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one;
        rt.offsetMin = rt.offsetMax = Vector2.zero;
        go.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.85f);
        return go;
    }

    static Button MakeButton(Transform parent, string name, string label, Vector2 pos, Vector2 size)
    {
        GameObject go = TMP_DefaultControls.CreateButton(new TMP_DefaultControls.Resources());
        go.name = name;
        go.transform.SetParent(parent, false);
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = pos; rt.sizeDelta = size;
        go.GetComponent<Image>().color = new Color(1f, 0.38f, 0f);
        TMP_Text t = go.GetComponentInChildren<TMP_Text>();
        t.text = label; t.fontSize = 44; t.color = Color.white;
        t.font = FindFont();
        return go.GetComponent<Button>();
    }

    static TMP_Text MakeText(Transform parent, string name, string text, Vector2 pos, Vector2 size, float fontSize)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(TextMeshProUGUI));
        go.transform.SetParent(parent, false);
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = pos; rt.sizeDelta = size;
        TMP_Text t = go.GetComponent<TMP_Text>();
        t.text = text; t.fontSize = fontSize; t.alignment = TextAlignmentOptions.Center;
        t.color = new Color(0.17f, 0.17f, 0.17f);
        t.font = FindFont();
        return t;
    }

    static Slider MakeSlider(Transform parent, string name, Vector2 pos, Vector2 size)
    {
        GameObject go = DefaultControls.CreateSlider(new DefaultControls.Resources());
        go.name = name;
        go.transform.SetParent(parent, false);
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = pos; rt.sizeDelta = size;
        Slider s = go.GetComponent<Slider>();
        s.minValue = 0f; s.maxValue = 1f; s.value = 0.8f;
        return s;
    }

    static TMP_FontAsset FindFont()
    {
        return AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/PressStart2P-Regular SDF.asset");
    }
}
