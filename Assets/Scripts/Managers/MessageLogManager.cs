using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// ログの表示管理用クラス
/// </summary>
public class MessageLogManager : MonoBehaviour
{
    [SerializeField] GameObject messageLogWindow;
    [SerializeField] Button messageLogWindowButton;
    [SerializeField] RectTransform initMessageLogWindowPos;
    [SerializeField] TextMeshProUGUI messageLogText;
    [SerializeField] RectTransform messageLogWindowPos;
    [SerializeField] ScrollRect scrollRect;

    private Vector3 messageWindowPos;
    private Coroutine typingCoroutine;
    private Queue<string> messageQueue = new Queue<string>();
    private float messageWaitTime = 0.1f;

    // メッセージウィンドウの表示フラグ
    private bool isMessageLogVisible = false;

    void Start()
    {
        messageWindowPos = messageLogWindowPos.localPosition;
    }

    /// <summary>
    /// メッセージウィンドウにテキスト表示する
    /// </summary>
    /// <param name="purpose"></param>
    public void ShowLog(string purpose, bool isActive = true, float typeSpeed = 0.1f, float showTime = 1)
    {
        // タイプする速度を設定
        messageWaitTime = typeSpeed;

        // メッセージウィンドウを表示
        PushMessageWindowButton(isActive);

        // メッセージをキューに追加
        messageQueue.Enqueue(purpose);

        // コルーチンが実行中でない場合のみ新しいコルーチンを開始
        if (typingCoroutine == null)
        {
            typingCoroutine = StartCoroutine(ProcessMessageQueue());
        }

        HideLog(showTime);
    }

    /// <summary>
    /// メッセージキューを処理する
    /// </summary>
    /// <returns></returns>
    private IEnumerator ProcessMessageQueue()
    {
        while (messageQueue.Count > 0)
        {
            string message = messageQueue.Dequeue();
            yield return StartCoroutine(TypeText(message));
        }
        typingCoroutine = null;
    }

    /// <summary>
    /// テキストを1文字ずつ表示する
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    private IEnumerator TypeText(string text)
    {
        text = text.Replace("\\n", "");
        foreach (char c in text)
        {
            messageLogText.text += c;
            yield return new WaitForSeconds(messageWaitTime);
            // スクロールを更新
            Canvas.ForceUpdateCanvases();
            scrollRect.verticalNormalizedPosition = 0f;
        }
        messageLogText.text += "\n\n";
        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0f;
        messageWaitTime = 0.1f;
    }

    /// <summary>
    /// メッセージウィンドウのボタンを押したときの処理
    /// </summary>
    public void PushMessageWindowButton()
    {
        PushMessageWindowButton(isMessageLogVisible);
    }

    /// <summary>
    /// メッセージウィンドウを表示する
    /// </summary>
    public void ShowLog()
    {
        PushMessageWindowButton(true);
    }

    /// <summary>
    /// メッセージウィンドウを非表示にする
    /// </summary>
    public void HideLog(float closeWaitTime = 0f)
    {
        if (typingCoroutine != null || messageQueue.Count > 0)
        {
            StartCoroutine(HideLogAfterDelay(closeWaitTime));
        }
        else
        {
            PushMessageWindowButton(false);
        }
    }

    /// <summary>
    /// 一定時間後にメッセージウィンドウを非表示にする
    /// </summary>
    /// <returns></returns>
    private IEnumerator HideLogAfterDelay(float closeWaitTime)
    {
        // タイプが終わるまで待つ
        while (typingCoroutine != null || messageQueue.Count > 0)
        {
            yield return null;
        }
        yield return new WaitForSeconds(closeWaitTime);
        PushMessageWindowButton(false);
    }

    /// <summary>
    /// メッセージウィンドウのボタンを押したときの処理
    /// </summary>
    /// <param name="isVisible"></param>
    public void PushMessageWindowButton(bool isVisible)
    {
        if (isVisible)
        {
            isMessageLogVisible = false;
            messageLogWindowPos.DOLocalMove(messageWindowPos, 0.5f);
        }
        else
        {
            isMessageLogVisible = true;
            messageLogWindowPos.DOLocalMove(initMessageLogWindowPos.localPosition, 0.5f);
        }
    }
}
