using UnityEngine;
using TMPro;

public class FontManager : MonoBehaviour
{
    public static FontManager Instance;
    public TMP_FontAsset defaultFont; // フォントを設定する

    private void Awake()
    {
        // シングルトンパターン（どこからでもアクセスできるようにする）
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (FontManager.Instance != null)
        {
            TextMeshProUGUI[] allTexts = FindObjectsOfType<TextMeshProUGUI>();
            Instance.ApplyFontToAll(allTexts);
        }
    }

    public void ApplyFontToAll(TextMeshProUGUI[] texts)
    {
        foreach (var text in texts)
        {
            if (text != null)
            {
                text.font = defaultFont;
            }
        }
    }
}