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

    private bool isBowUnlocked = false;

    [Header("Light boots")]
    public Material normalBootsMat;
    public Material lightBootsMat;
    private bool isLightBootsEquipped = false;
    private bool isLightBootsUnlocked;
    public GameObject boots;
    public GameObject bootsLight;

    private bool isCoroutineRunning = false;

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
        if(varValue > 0)
        {
            rewardPanelText.text += varName + " " + varValue + "\n "; //add multiple rewards to the same string in case the player receives multiple rewards at once
        }
        else
        {
            rewardPanelText.text += varName + "\n "; //add multiple rewards to the same string in case the player receives multiple rewards at once
        }
       
        StartCoroutine(DisableGameObjectAfter(rewardAlertPanel, 3, true));
    }

    public void EquipLightBoots()
    {
        if (!isLightBootsUnlocked)
            return;

        if(isLightBootsEquipped)
        {
            boots.GetComponent<Renderer>().material = normalBootsMat;
            bootsLight.SetActive(false);
            isLightBootsEquipped = false;
        }
        else
        {
            boots.GetComponent<Renderer>().material = lightBootsMat;
            bootsLight.SetActive(true);
            isLightBootsEquipped = true;
        }
    }

    //called in QuestComponent.cs upon claiming a quest reward
    public void UnlockLightBoots()
    {
        isLightBootsUnlocked = true;
        UpdateRewardPanelText("Light boots unlocked!", 0);
        Chochosan.UI_Chochosan.Instance.UI_UpdateTextsOnMagicUnlocked("Light boots");
    }

    public void UnlockBow()
    {
        isBowUnlocked = true;
        UpdateRewardPanelText("Bow unlocked!", 0);
        Chochosan.UI_Chochosan.Instance.UI_UpdateTextsOnMagicUnlocked("Bow fire arrow");
    }

    public void UnlockAllCheat()
    {
        UnlockBow();
        UnlockLightBoots();
    }

    public bool IsBowUnlocked()
    {
        return isBowUnlocked;
    }

    public bool IsLightBootsUnlocked()
    {
        return isLightBootsUnlocked;
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
        if (!isCoroutineRunning)
        {
            isCoroutineRunning = true;
            yield return new WaitForSeconds(disableAfterSeconds);
            if (clearTextAtTheEnd)
            {
                objectToDisable.GetComponentInChildren<TextMeshProUGUI>().text = ""; //clear the string at the end
            }
            objectToDisable.SetActive(false);
            isCoroutineRunning = false;
        }
    }
}
