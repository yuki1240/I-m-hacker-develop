using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI dialogueText;
    [SerializeField] public TextMeshProUGUI descriptionText;
    [SerializeField] public GameObject scrollField;
    [SerializeField] public GameObject description;
    [SerializeField] public Button yesButton;
    [SerializeField] public Button noButton;
    [SerializeField] public Button effectButton;
    [SerializeField] public Button meaningButton;
    [SerializeField] public Button returnButton;


    [HideInInspector] public bool selectYesFlag;
    [HideInInspector] public bool selectNoFlag;
    [HideInInspector] public bool selectEffectFlag;
    [HideInInspector] public bool selectMeaningFlag;
    [HideInInspector] public bool selectReturnFlag;
    [HideInInspector] public bool selectFieldEffectButtonFlag;

    void Start()
    {
        descriptionText = description.GetComponent<TextMeshProUGUI>();
    }

    public void InitDialogue()
    {
        dialogueText.text = "";
        descriptionText.text = "";

        gameObject.SetActive(false);
        scrollField.SetActive(false);
        returnButton.gameObject.SetActive(false);

        selectYesFlag = false;
        selectNoFlag = false;
        selectEffectFlag = false;
        selectMeaningFlag = false;
        selectReturnFlag = false;
    }

    public void PushYesButton()
    {
        selectYesFlag = true;
    }

    public void PushNoButton()
    {
        selectNoFlag = true;
    }

    public void PushEffectButton()
    {
        selectEffectFlag = true;
    }

    public void PushMeaningButton()
    {
        selectMeaningFlag = true;
    }

    public void DisplayDescriptionText()
    {
        dialogueText.enabled = false;
        scrollField.SetActive(true);
    }

    public void DisableDescriptionText()
    {
        dialogueText.enabled = true;
        scrollField.SetActive(false);
    }

    public IEnumerator ShowEffectDialogue(CardController card)
    {
        descriptionText.text = card.Model.Explanation;
        DisplayDescriptionText();
        yesButton.interactable = false;
        noButton.interactable = false;
        effectButton.interactable = false;
        meaningButton.interactable = false;
        returnButton.interactable = true;

        yesButton.gameObject.SetActive(false);
        noButton.gameObject.SetActive(false);
        effectButton.gameObject.SetActive(false);
        meaningButton.gameObject.SetActive(false);
        returnButton.gameObject.SetActive(true);

        yield return new WaitUntil(() => selectReturnFlag);

        selectReturnFlag = false;
        returnButton.gameObject.SetActive(false);
        DisableDescriptionText();
    }

    public IEnumerator ShowMeaningDialogue(CardController card)
    {
        descriptionText.text = card.Model.Meaning;
        DisplayDescriptionText();
        yesButton.interactable = false;
        noButton.interactable = false;
        effectButton.interactable = false;
        meaningButton.interactable = false;
        returnButton.interactable = true;

        yesButton.gameObject.SetActive(false);
        noButton.gameObject.SetActive(false);
        effectButton.gameObject.SetActive(false);
        meaningButton.gameObject.SetActive(false);
        returnButton.gameObject.SetActive(true);

        yield return new WaitUntil(() => selectReturnFlag);

        selectReturnFlag = false;
        returnButton.gameObject.SetActive(false);
        DisableDescriptionText();
    }
}
