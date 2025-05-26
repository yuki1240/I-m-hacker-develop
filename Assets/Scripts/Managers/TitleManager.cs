using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    [SerializeField] RuleManager rulePanel;

    public void Start()
    {
    }

    public void PushStartButton()
    {
        SceneManager.LoadScene("Game");
    }

    public void PushRuleButton()
    {
        rulePanel.ShowRulePanel();
    }
}
