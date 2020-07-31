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
    }

    //prefabs
    public TextMeshProUGUI goldAmountText;
    public TextMeshProUGUI talentPointsAmountText;

    //create a delegate event
    public delegate void OnInventoryVariableChangeDelegate(string varName);
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
               OnInventoryVariableChange("gold"); //invoke the delegate
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
                OnInventoryVariableChange("talents");//invoke the delegate
        }
    }

    private void UpdateDisplayTexts(string varName)
    {
        switch (varName)
        {
            case "gold":
                goldAmountText.text = TotalGold.ToString();
                break;
            case "talents":
                talentPointsAmountText.text = TotalSkillPoints.ToString();
                break;
        }
    }
}
