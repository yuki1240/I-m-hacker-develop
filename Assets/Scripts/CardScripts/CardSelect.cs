using UnityEngine;

/// <summary>
/// カードの選択処理を管理するクラス
/// </summary>
public class CardSelect : MonoBehaviour
{
    /// <summary>
    /// カードの初期位置（将来的な機能のために予約）
    /// </summary>
    /*
    Vector3 initPosition;
    Vector3 currentPosition;
    */

    // GameManagerの参照
    private GameManager gameManager;

    private DetailPanelManager detailPanelManager = null;

    // カードの選択可能状態を管理
    public bool IsSelectable { get; set; }

    // カードが選択済みかどうかを管理
    public bool IsSelected { get; set; }

    /// <summary>
    /// 初期化処理
    /// </summary>
    void Start()
    {
        gameManager = GameManager.Instance;
    }

    /// <summary>
    /// 毎フレーム実行される処理
    /// 選択不可能またはプレイヤーのターンでない場合は処理をスキップ
    /// </summary>
    void Update()
    {
        if (!IsSelectable || !gameManager.IsPlayerTurn)
        {
            return;
        }
        /*
        // 将来的なカード移動機能のために予約
        currentPosition = transform.position;
        */
    }

    /// <summary>
    /// マウスがカードに重なった時の処理
    /// 将来的にカードを少し持ち上げる演出を追加予定
    /// </summary>
    public void MouseOnCard()
    {
        if (!IsSelectable || !gameManager.IsPlayerTurn)
        {
            return;
        }

        /*
        // カードを持ち上げる処理（将来実装予定）
        initPosition = currentPosition;
        Vector3 movePosition = currentPosition;
        movePosition.y += 30;
        transform.position = movePosition;
        */
    }

    /// <summary>
    /// マウスがカードから離れた時の処理
    /// 将来的にカードを元の位置に戻す処理を追加予定
    /// </summary>
    public void MouseExitCard()
    {
        if (!IsSelectable || !gameManager.IsPlayerTurn)
        {
            return;
        }
        /*
        // カードを元の位置に戻す処理（将来実装予定）
        transform.position = initPosition;
        */
    }

    /// <summary>
    /// カードがクリックされた時の処理
    /// カードを選択状態にしてGameManagerに通知
    /// </summary>
    public void MouseClickCard()
    {
        if (!IsSelectable || !gameManager.IsPlayerTurn)
        {
            return;
        }
        /*
        // カードを元の位置に戻す処理（将来実装予定）
        transform.position = initPosition;
        */

        // カードが選択されたことをGameManagerに通知
        gameManager.CardSelectflag = true;
        gameManager.NowSelectingCard = GetComponent<CardController>();
    }
}
