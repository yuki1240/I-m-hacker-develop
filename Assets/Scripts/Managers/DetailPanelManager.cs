using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Collections;

public class DetailPanelManager : MonoBehaviour
{
    [SerializeField] Image cardImage;
    [SerializeField] TMP_Text detailText;
    [SerializeField] GameObject cardDetailPanel;
    [SerializeField] GameObject cardSelectPanel;

    // 詳細パネルのボタン
    [SerializeField] Button dettail_DiscardButton;
    [SerializeField] Button dettail_UseButton;
    [SerializeField] Button dettail_CloseButton;

    // 選択パネルのボタン
    [SerializeField] Button select_DiscardButton;
    [SerializeField] Button select_CloseButton;



    [HideInInspector] public bool isSelectFlg = false;
    private bool isShowFlg = false;

    void Start()
    {
        cardDetailPanel.SetActive(false);
        cardSelectPanel.SetActive(false);

        dettail_DiscardButton.onClick.AddListener(OnButton);
        dettail_UseButton.onClick.AddListener(OnButton);
        dettail_CloseButton.onClick.AddListener(ClosePanel);

        select_DiscardButton.onClick.AddListener(OnButton);
        select_CloseButton.onClick.AddListener(ClosePanel);
    }

    /// <summary>
    /// カードの詳細情報を表示
    /// </summary>
    /// <param name="card"></param>
    public IEnumerator ShowDetailPanel(CardController card)
    {
        Debug.Log($"call ShowDetailPanel 詳細パネルを表示");
        // もしプレイヤーのターンだったら
        if (GameManager.Instance.IsPlayerTurn)
        {
            bool isEventPhase = GameManager.Instance.phase == GameManager.PHASE.EVENT;
            bool isEnemyPasswordField = card.transform.parent.name == "EnemyPasswordField";
            bool isMulliganPhase = GameManager.Instance.phase == GameManager.PHASE.MULLIGAN;

            // マリガンフェーズの場合は「捨てる」ボタンを表示
            if (isMulliganPhase)
            {
                Debug.Log($"Mulligan in ShowDetailPanel");

                cardDetailPanel.SetActive(true);

                dettail_DiscardButton.gameObject.SetActive(true);
                dettail_UseButton.gameObject.SetActive(false);

                var cardView = card.transform.gameObject.GetComponent<CardView>();
                detailText.text = "＜効果＞\n" + cardView.cardText.text.Replace("\\n", "\n");
                cardImage.sprite = cardView.cardSprite.sprite;
            }

            if (isEventPhase && !isEnemyPasswordField)
            {
                Debug.Log($"event");
                cardDetailPanel.SetActive(true);

                dettail_DiscardButton.gameObject.SetActive(false);
                dettail_UseButton.gameObject.SetActive(true);

                var cardView = card.transform.gameObject.GetComponent<CardView>();
                detailText.text = "＜効果＞\n" + cardView.cardText.text.Replace("\\n", "\n");
                cardImage.sprite = cardView.cardSprite.sprite;
            }

            if (isEventPhase && isEnemyPasswordField)
            {
                Debug.Log($"敵のパスワード選択時");
                cardSelectPanel.SetActive(true);
            }
        }

        isShowFlg = true;

        // ボタンが押されるか、画面外をタップするまで待機
        while (isShowFlg)
        {
            yield return null;
        }

        yield return null;
    }

    /// <summary>
    /// 「捨てる」or「使う」ボタンを押したとき
    /// </summary>
    private void OnButton()
    {
        isSelectFlg = true;
        isShowFlg = false;
        cardDetailPanel.SetActive(false);
        cardSelectPanel.SetActive(false);
    }

    /// <summary>
    /// パネルの表示/非表示を設定
    /// </summary>
    /// <param name="isActive"></param>
    private void SetActive(bool isActive)
    {
        cardDetailPanel.SetActive(isActive);
        if (!isActive)
        {
            detailText.text = "";
            cardImage.sprite = null;
            transform.localScale = new Vector3(1, 1, 1);
        }
    }

    /// <summary>
    /// パネルを閉じる
    /// </summary>
    private void ClosePanel()
    {
        isSelectFlg = false;
        isShowFlg = false;
        cardDetailPanel.SetActive(false);
        cardSelectPanel.SetActive(false);
    }
}
