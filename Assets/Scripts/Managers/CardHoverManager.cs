using UnityEngine;
using UnityEngine.UI;

public class CardHoverManager : MonoBehaviour
{
    [SerializeField] private GameObject previewCardImage;
    private Vector3 originalPosition;
    private CardController currentHoveredCard;

    private void Start()
    {
        previewCardImage = GameObject.Find("PreviewCardImage");

        // プレビューカードイメージを非表示にする
        previewCardImage.SetActive(false);
    }

    private void Update()
    {
        HandleCardHover();
    }

    /// <summary>
    /// カードのホバー処理を行う
    /// </summary>
    private void HandleCardHover()
    {
        // マウスの位置からレイを飛ばす
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // レイがカードに当たったかどうかを判定
        if (Physics.Raycast(ray, out hit))
        {
            CardController card = hit.transform.GetComponent<CardController>();
            if (card != null)
            {
                if (currentHoveredCard != card)
                {
                    ResetCardPosition();
                    currentHoveredCard = card;
                    originalPosition = card.transform.position;
                    card.transform.position += new Vector3(0, 0.5f, 0); // カードを上に突き出す

                    // プレビューカードイメージを更新して表示
                    previewCardImage.SetActive(true);
                    previewCardImage.GetComponent<Image>().sprite = card.GetComponent<Image>().sprite;
                }
            }
        }
        else
        {
            ResetCardPosition();
            // プレビューカードイメージを非表示にする
            previewCardImage.SetActive(false);
        }
    }

    /// <summary>
    /// カードの位置をリセットする
    /// </summary>
    private void ResetCardPosition()
    {
        if (currentHoveredCard != null)
        {
            currentHoveredCard.transform.position = originalPosition;
            currentHoveredCard = null;
        }
    }
}
