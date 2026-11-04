using UnityEngine;

public class SafeArea : MonoBehaviour
{
    RectTransform rt;
    Rect lastSafe;

    void Awake()
    {
        rt = GetComponent<RectTransform>();
        Apply();
    }

    void Update()
    {
        if (Screen.safeArea != lastSafe) Apply();   // на случай поворота экрана
    }

    void Apply()
    {
        Rect safe = Screen.safeArea;
        lastSafe = safe;

        Vector2 min = safe.position;
        Vector2 max = safe.position + safe.size;
        min.x /= Screen.width;  min.y /= Screen.height;
        max.x /= Screen.width;  max.y /= Screen.height;

        rt.anchorMin = min;                 // привязываем углы панели к безопасной зоне
        rt.anchorMax = max;
        rt.offsetMin = rt.offsetMax = Vector2.zero;
    }
}