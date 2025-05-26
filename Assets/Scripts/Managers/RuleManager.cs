using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class RuleManager : MonoBehaviour
{
    [SerializeField] private Sprite[] tutorialImages = new Sprite[12];
    [SerializeField] private Image tutorialImagePanel;
    [SerializeField] private Button leftButton;
    [SerializeField] private Button rightButton;
    [SerializeField] private TextMeshProUGUI ruleIndex;

    private int currentRuleIndex = 1;

    void Start()
    {

    }

    /// <summary>
    /// ルールパネルを表示する
    /// </summary>
    public void ShowRulePanel()
    {
        gameObject.SetActive(true);
    }

    /// <summary>
    /// 左スクロールボタンが押されたときの処理
    /// </summary>
    public void PushScrollButtonLeft()
    {
        // インデックスが1より大きければ-1する
        if (currentRuleIndex > 1) currentRuleIndex--;
        ChangeUI();
    }

    /// <summary>
    /// 右スクロールボタンが押されたときの処理
    /// </summary>
    public void PushScrollButtonRight()
    {
        // インデックスが12より小さければ+1する
        if (currentRuleIndex < 12) currentRuleIndex++;
        ChangeUI();
    }

    /// <summary>
    /// チュートリアル画像とインデックスを変更する
    /// </summary>
    void ChangeUI()
    {
        tutorialImagePanel.sprite = tutorialImages[currentRuleIndex - 1];
        ruleIndex.text = $"{currentRuleIndex.ToString()} / 12";
    }

    /// <summary>
    /// ルールパネルを閉じる
    /// </summary>
    public void CloseRulePanel()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// パネルがアクティブになったときの処理
    /// </summary>
    void OnEnable()
    {
        tutorialImagePanel.sprite = tutorialImages[0];
        currentRuleIndex = 1;
        ruleIndex.text = $"{currentRuleIndex.ToString()} / 12";
    }
}
