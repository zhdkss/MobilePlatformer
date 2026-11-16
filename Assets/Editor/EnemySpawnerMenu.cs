using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

// Меню Tools > Gem Runner: ставит готового врага в центр окна Scene.
// Дальше его можно двигать и настраивать в Inspector.
public static class EnemySpawnerMenu
{
    const string SpriteSheet = "Assets/tilemap-characters_packed.png";
    const string FireballPrefab = "Assets/Prefabs/Fireball.prefab";

    [MenuItem("Tools/Gem Runner/Add Patroller")]
    static void AddPatroller()
    {
        GameObject go = CreateEnemy("Patroller", 15, new Color(1f, 0.85f, 0.85f));
        PatrollerEnemy e = go.AddComponent<PatrollerEnemy>();
        e.obstacleMask = GroundMask();
        e.waypoints = new[]
        {
            CreateChild(go, "WaypointA", new Vector3(-3f, 0f, 0f)),
            CreateChild(go, "WaypointB", new Vector3(3f, 0f, 0f))
        };
    }

    [MenuItem("Tools/Gem Runner/Add Shooter")]
    static void AddShooter()
    {
        GameObject go = CreateEnemy("Shooter", 11, Color.white);
        ShooterEnemy e = go.AddComponent<ShooterEnemy>();
        e.obstacleMask = GroundMask();
        e.projectilePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(FireballPrefab);
        e.firePoint = CreateChild(go, "FirePoint", new Vector3(0f, 0.2f, 0f));
        e.shootSound = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/shoot.wav");

        LineRenderer line = go.AddComponent<LineRenderer>();
        line.positionCount = 2;
        line.useWorldSpace = true;
        line.startWidth = line.endWidth = 0.06f;
        line.sharedMaterial = AssetDatabase.GetBuiltinExtraResource<Material>("Sprites-Default.mat");
        line.startColor = line.endColor = new Color(1f, 0.2f, 0.2f, 0.8f);
        SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
        line.sortingLayerID = sr.sortingLayerID;
        line.sortingOrder = sr.sortingOrder + 1;
        line.enabled = false;
        e.aimLine = line;
    }

    [MenuItem("Tools/Gem Runner/Add Charger")]
    static void AddCharger()
    {
        GameObject go = CreateEnemy("Charger", 21, new Color(1f, 0.8f, 0.6f));
        ChargerEnemy e = go.AddComponent<ChargerEnemy>();
        e.obstacleMask = GroundMask();
    }

    static GameObject CreateEnemy(string name, int spriteIndex, Color tint)
    {
        GameObject go = new GameObject(name);
        Undo.RegisterCreatedObjectUndo(go, "Add " + name);

        Vector3 pos = SceneView.lastActiveSceneView != null ? SceneView.lastActiveSceneView.pivot : Vector3.zero;
        pos.z = 0f;
        go.transform.position = pos;

        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = LoadSprite(spriteIndex);
        sr.color = tint;

        // слой сортировки и размер берём у уже стоящего врага, чтобы новый выглядел так же
        Enemy old = Object.FindAnyObjectByType<Enemy>();
        if (old != null && old.gameObject != go)
        {
            SpriteRenderer oldSr = old.GetComponent<SpriteRenderer>();
            if (oldSr != null)
            {
                sr.sortingLayerID = oldSr.sortingLayerID;
                sr.sortingOrder = oldSr.sortingOrder;
            }
            go.transform.localScale = old.transform.localScale;
        }

        BoxCollider2D col = go.AddComponent<BoxCollider2D>();   // размер подгоняется под спрайт
        col.isTrigger = true;
        go.AddComponent<Enemy>();                               // прыжок сверху и урон от касания

        Selection.activeGameObject = go;
        EditorSceneManager.MarkSceneDirty(go.scene);
        return go;
    }

    static Transform CreateChild(GameObject parent, string name, Vector3 localPos)
    {
        GameObject child = new GameObject(name);
        child.transform.SetParent(parent.transform, false);
        child.transform.localPosition = localPos;
        return child.transform;
    }

    static Sprite LoadSprite(int index)
    {
        string wanted = "tilemap-characters_packed_" + index;
        foreach (Object o in AssetDatabase.LoadAllAssetsAtPath(SpriteSheet))
            if (o is Sprite s && s.name == wanted) return s;
        Debug.LogWarning("Sprite not found: " + wanted);
        return null;
    }

    static LayerMask GroundMask()
    {
        PlayerController pc = Object.FindAnyObjectByType<PlayerController>();
        return pc != null ? pc.groundLayer : (LayerMask)LayerMask.GetMask("Ground");
    }
}
