using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Collections;

public class PasswordSelectionCard : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Image frontImage;
    [SerializeField] private Image backImage;
    [SerializeField] private float flipDuration = 1.0f;

    public bool isFaceUp = false;
    private bool isZoomed = false;
    private Vector3 originalPosition;
    private Vector3 zoomedScale = new Vector3(1.5f, 1.5f, 1.5f);
    private PasswordSelectionCard cardScript;
    private PasswordCardEntity passwordCardEntity;
    private PasswordSelectionCardManager manager;

    void Start()
    {
        originalPosition = transform.localPosition;
        cardScript = GetComponent<PasswordSelectionCard>();
        manager = GameObject.Find("PasswordSelectionCardManager").GetComponent<PasswordSelectionCardManager>();
        SetCardEntity();
    }

    private void SetCardEntity()
    {
        var passwordCardEntities = Resources.LoadAll<PasswordCardEntity>("CardEntityList");
        foreach (var entity in passwordCardEntities)
        {
            // entity.nameとgameobjectの名前の先頭が一致する場合にentityを取得
            if (entity.name == name.Substring(0, 1))
            {
                passwordCardEntity = entity;
                break;
            }
        }
    }

    void Update()
    {
        if (isZoomed && Input.GetMouseButtonDown(0))
        {
            // カード以外をクリックした場合にズームを解除
            if (!IsPointerOverUIObject())
            {
                ResetZoom();
            }
        }
    }

    public void HiddenImage()
    {
        if (!isFaceUp) return;
        isFaceUp = false;
        Rotate(backImage, frontImage);
    }

    public void ShowImage()
    {
        if (isFaceUp) return;
        isFaceUp = true;
        // initCardPos = transform.localPosition;
        Rotate(frontImage, backImage);
    }

    private void Rotate(Image showImage, Image hideImage)
    {
        transform.DOLocalRotate(new Vector3(0, 90, 0), flipDuration / 2)
            .OnComplete(() =>
            {
                if (this != null && gameObject != null)
                {
                    hideImage.gameObject.SetActive(false);
                    showImage.gameObject.SetActive(true);
                    transform.DOLocalRotate(new Vector3(0, 0, 0), flipDuration / 2);
                }
            });
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (cardScript != null && !cardScript.isFaceUp)
        {
            return;
        }

        if (isZoomed)
        {
            ResetZoom();
        }
        else
        {
            Zoom();
        }
    }

    private IEnumerator ShowEplanationText()
    {
        yield return new WaitForSeconds(0.5f);
        manager.zoomBackPanel.SetActive(true);
        var text = passwordCardEntity.explanation.Replace("\\n", "\n");
        manager.explanationText.text = string.Empty;
        // textを1文字1文字流れるように表示していく
        foreach (var c in text)
        {
            manager.explanationText.text += c;
            yield return new WaitForSeconds(0.1f);
        }
        isZoomed = true;
    }

    public void Zoom()
    {
        // 他のカードのズームを解除
        PasswordSelectionCard[] cards = transform.parent.GetComponentsInChildren<PasswordSelectionCard>();
        foreach (var card in cards)
        {
            card.ResetZoom();
        }

        transform.parent.SetAsLastSibling();
        transform.DOScale(zoomedScale, 0.5f);
        StartCoroutine(ShowEplanationText());
    }

    public void ResetZoom()
    {
        transform.DOLocalMove(originalPosition, 0f);
        transform.DOScale(1, 0.5f).OnComplete(() =>
        {
            manager.zoomBackPanel.SetActive(false);
            isZoomed = false;
            manager.explanationText.text = string.Empty;
        });
    }

    private bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current)
        {
            position = new Vector2(Input.mousePosition.x, Input.mousePosition.y)
        };
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }
}
