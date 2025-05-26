using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// カードの表示に関する処理を管理するクラス
/// </summary>
public class CardView : MonoBehaviour
{
    // カードの画像を表示するコンポーネント
    [SerializeField] public Image cardSprite;

    // カードの説明文を表示するテキストコンポーネント
    [SerializeField] public TextMeshProUGUI cardText;

    // カードの裏面を表示するパネル
    [SerializeField] GameObject backPanel;

    // カードが選択不可能な時に表示する半透明パネル
    [SerializeField] GameObject disselectablePanel;

    /// <summary>
    /// カードの表示内容を設定
    /// </summary>
    /// <param name="cardModel">設定するカードのモデル情報</param>
    public void SetCard(CardModel cardModel)
    {
        // カードの画像を設定
        cardSprite.sprite = cardModel.CardSprite;

        // カードの説明文を設定
        cardText.text = cardModel.Explanation;

        // プレイヤーのカードでない場合は裏面を表示
        backPanel.SetActive(!cardModel.IsPlayerCard);

        // 初期状態では選択不可能な状態に設定
        disselectablePanel.SetActive(true);
    }

    /// <summary>
    /// カードの表面を表示（裏面パネルを非表示に）
    /// </summary>
    public void Show()
    {
        backPanel.SetActive(false);
    }

    /// <summary>
    /// カードを裏向きにする（裏面パネルを表示）
    /// </summary>
    public void TurnDown()
    {
        backPanel.SetActive(true);
    }

    /// <summary>
    /// カードの選択可能/不可能状態を設定
    /// </summary>
    /// <param name="isNotSelectable">true: 選択不可能, false: 選択可能</param>
    public void IsNotSelectableCard(bool isNotSelectable)
    {
        disselectablePanel.SetActive(isNotSelectable);
    }
}
