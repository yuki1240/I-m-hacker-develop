using UnityEngine;
using TMPro;
using DG.Tweening;

public class NavigationMessagesManager : MonoBehaviour
{
    [SerializeField] TMP_Text navigationMessages;
    [SerializeField] float slideDuration = 0.5f;
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] GameObject panel;

    private Sequence sequence;

    void Start()
    {
        panel.SetActive(false);
    }

    /// <summary>
    /// メッセージの表示（画面右から左へスライドイン）
    /// </summary>
    /// <param name="message"></param>
    public void ShowNavigationMessage(string message, float duration = 2f)
    {
        panel.SetActive(true);
        navigationMessages.text = message;
        rectTransform.localPosition = new Vector2(3000, 0);
        sequence = DOTween.Sequence();
        sequence.Append(rectTransform.DOAnchorPosX(0, slideDuration).SetEase(Ease.OutQuad));
        sequence.AppendInterval(duration);
        sequence.Append(rectTransform.DOAnchorPosX(-Screen.width, slideDuration).SetEase(Ease.InQuad));
        sequence.OnComplete(() => ClearMessage());
    }

    /// <summary>
    /// メッセージの初期化
    /// </summary>
    private void ClearMessage()
    {
        navigationMessages.text = "";
        panel.SetActive(false);
    }
}
