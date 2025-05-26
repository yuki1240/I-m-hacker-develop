using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;

public class PasswordInputPanelAnimation : MonoBehaviour
{
    [SerializeField] private RectTransform topImage;
    [SerializeField] private RectTransform bottomImage;
    [SerializeField] private RectTransform noteImage;
    [SerializeField] private RectTransform inputFieldImage;
    [SerializeField] private float duration = 1.0f;
    [SerializeField] private GameObject panel;

    private float offset = 1000f;
    private Vector2 topStartPos;
    private Vector2 bottomStartPos;
    private Vector2 centerTopPos;
    private Vector2 centerBottomPos;
    private Vector2 outwardTopPos;
    private Vector2 outwardBottomPos;

    void Start()
    {
        // 初期位置を取得
        topStartPos = topImage.anchoredPosition;
        bottomStartPos = bottomImage.anchoredPosition;

        // 中央の位置を計算
        float centerY = (topStartPos.y + bottomStartPos.y) / 2;
        centerTopPos = new Vector2(topStartPos.x, centerY);
        centerBottomPos = new Vector2(bottomStartPos.x, centerY);

        // 開く位置を計算
        outwardTopPos = new Vector2(topStartPos.x, topStartPos.y + offset);
        outwardBottomPos = new Vector2(bottomStartPos.x, bottomStartPos.y - offset);

        // 初期位置に移動
        topImage.anchoredPosition = new Vector2(0, 1000);
        bottomImage.anchoredPosition = new Vector2(0, -1000);
        noteImage.anchoredPosition = new Vector2(-3000, 144);
        inputFieldImage.anchoredPosition = new Vector2(0, -1000);
    }

    /// <summary>
    /// パネルを開くアニメーションを再生
    /// </summary>
    public void ShowAnimation()
    {
        panel.SetActive(true);
        MoveToCenter();
        StartCoroutine(MoveNoteImageToLeft());
    }

    /// <summary>
    /// パネルを閉じるアニメーションを再生
    /// </summary>
    public void HideAnimation()
    {
        // inputFieldImage.color = new Color(1, 1, 1, 1);
        MoveOutward();
        MoveNoteImageToRight();
        panel.SetActive(false);
    }

    /// <summary>
    /// 画像を中央に向かって移動させる
    /// </summary>
    private void MoveToCenter()
    {
        topImage.DOAnchorPos(centerTopPos, duration).SetEase(Ease.OutQuad);
        bottomImage.DOAnchorPos(centerBottomPos, duration).SetEase(Ease.OutQuad);
        inputFieldImage.DOAnchorPos(new Vector2(0, 10), duration).SetEase(Ease.OutQuad);
    }

    /// <summary>
    /// 画像を中央から外へ開く
    /// </summary>
    private void MoveOutward()
    {
        topImage.DOAnchorPos(outwardTopPos, duration).SetEase(Ease.OutQuad);
        bottomImage.DOAnchorPos(outwardBottomPos, duration).SetEase(Ease.OutQuad);
    }

    /// <summary>
    /// NoteImageが左から中央に移動するアニメーション
    /// </summary>
    private IEnumerator MoveNoteImageToLeft()
    {
        yield return new WaitForSeconds(1.0f);
        noteImage.DOAnchorPos(new Vector2(0, 144), 0.5f).SetEase(Ease.OutQuad);
    }

    /// <summary>
    // NoteImageが現在地から右に移動するアニメーション
    /// </summary>
    private void MoveNoteImageToRight()
    {
        noteImage.DOAnchorPos(new Vector3(3000, 144, 0), 0.5f).SetEase(Ease.OutQuad);
    }
}
