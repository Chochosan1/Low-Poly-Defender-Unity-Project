using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
/// <summary>
/// Controls the player's inventory and creates certain inventory change events that get called automatically upon 
/// changing the state of parts of the inventory from external classes.
/// </summary>
public class Player_Inventory : MonoBehaviour
{
    public static Player_Inventory instance; //singleton
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        //subscribe to events here
        OnInventoryVariableChange += UpdateDisplayTexts;
        OnInventoryVariableChange += UpdateRewardPanelText;
    }

    private void OnDisable()
    {
        OnInventoryVariableChange -= UpdateDisplayTexts;
        OnInventoryVariableChange -= UpdateRewardPanelText;
    }

    private void Start()
    {
        rewardPanelText = rewardAlertPanel.GetComponentInChildren<TextMeshProUGUI>();
    }

    //prefabs
    public TextMeshProUGUI goldAmountText;
    public TextMeshProUGUI talentPointsAmountText;
    private TextMeshProUGUI rewardPanelText;

    public GameObject rewardAlertPanel;

    //create a delegate event
    public delegate void OnInventoryVariableChangeDelegate(string varName, int varValue);
    public event OnInventoryVariableChangeDelegate OnInventoryVariableChange;

    private int totalGold;
    public int TotalGold
    {
        get { return totalGold; }
        set
        {
            if (totalGold == value) return;
            totalGold = value;
            if (OnInventoryVariableChange != null)
                OnInventoryVariableChange("Gold", totalGold); //invoke the delegate
        }
    }

    private int totalSkillPoints;
    public int TotalSkillPoints
    {
        get { return totalSkillPoints; }
        set
        {
            if (totalSkillPoints == value) return;
            totalSkillPoints = value;
            if (OnInventoryVariableChange != null)
                OnInventoryVariableChange("Talents", totalSkillPoints);//invoke the delegate
        }
    }

    public void UpdateRewardPanelText(string varName, int varValue)
    {
        rewardAlertPanel.SetActive(true);
        rewardPanelText.text += "\n " + varName + " " + varValue; //add multiple rewards to the same string in case the player receives multiple rewards at once
        StartCoroutine(DisableGameObjectAfter(rewardAlertPanel, 3, true));
    }

    private void UpdateDisplayTexts(string varName, int varValue)
    {
        switch (varName)
        {
            case "Gold":
                goldAmountText.text = TotalGold.ToString();
                break;
            case "Talents":
                talentPointsAmountText.text = TotalSkillPoints.ToString();
                break;
        }
    }

    private IEnumerator DisableGameObjectAfter(GameObject objectToDisable, float disableAfterSeconds, bool clearTextAtTheEnd)
    {
        yield return new WaitForSeconds(disableAfterSeconds);
        if (clearTextAtTheEnd)
        {
            objectToDisable.GetComponentInChildren<TextMeshProUGUI>().text = ""; //clear the string at the end
        }
        objectToDisable.SetActive(false);
    }
}
