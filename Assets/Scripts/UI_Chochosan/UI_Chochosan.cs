using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
/// <summary>
/// Controls the updates of the UI. Attached to the canvas component. Singleton for easy access.
/// </summary>
namespace Chochosan
{
    public class UI_Chochosan : MonoBehaviour
    {
        public static UI_Chochosan Instance;

        private void Awake()
        {
            if(Instance == null)
            {
                Instance = this;
            }
        }

        public SO_Player_Stats playerStats;
        [SerializeField]
        private UnityEngine.UI.Slider healthBar;
        [SerializeField]
        private UnityEngine.UI.Slider rageBar;

        //supports up to 10 quests at a time 
        //if it holds 0 means its free; 1 means its occupied by a quest 
        //free/occupied is dynamically changed during playtime
        private int[] UI_FreeOccupiedQuestTextComponents = new int[10]; 

        [SerializeField]
        private List<TMPro.TextMeshProUGUI> lightBootsTexts; //buttonText; stateText; tooltipText;
        [SerializeField]
        private List<TMPro.TextMeshProUGUI> bowTexts; //buttonText; stateText; tooltipText;

        //used in Player_Inventory.cs upon unlocking something new
        public void UI_UpdateTextsOnMagicUnlocked(string UI_PartToUpdate)
        {
            switch(UI_PartToUpdate)
            {
                case "Light boots":
                    lightBootsTexts[0].text = "Light boots";
                    lightBootsTexts[1].text = "[Rechargeable]";
                    lightBootsTexts[2].text = "Imbues your boots with powerful magic causing them to emmit light.";
                    break;
                case "Bow fire arrow":
                    bowTexts[0].text = "Fire arrow";
                    bowTexts[1].text = "[Permanent]";
                    bowTexts[2].text = "You now have a bow that can shoot fire arrows.";
                    break;
            }
        }

        public void TogglePanel(GameObject panel)
        {
            panel.SetActive(!panel.activeSelf);
        }

        //when a quest in QuestComponent.cs is assigne a quest text slot -> then mark it as occupied
        public void SetQuestTextOccupied(int questTextID)
        {
            UI_FreeOccupiedQuestTextComponents[questTextID] = 1;
        }

        //free the quest text slot when the quest is completed
        public void SetQuestTextFree(int questTextID)
        {
            UI_FreeOccupiedQuestTextComponents[questTextID] = 0;
        }

        //find the first non-occupied quest text slot and return it so that the quest calling it can use it
        public int GetFirstFreeQuestText()
        {
            int firstFreeTextIndex = 0;
            for(int i = 0; i < UI_FreeOccupiedQuestTextComponents.Length; i++)
            {
                if(UI_FreeOccupiedQuestTextComponents[i] != 1)
                {
                    firstFreeTextIndex = i;
                    break;
                }
            }

            return firstFreeTextIndex;
        }

        public void UpdateBarValues(string resourceName)
        {
            switch (resourceName)
            {
                case "Rage":
                    rageBar.value = playerStats.currentRage;
                    break;
                case "Health":
                    healthBar.value = playerStats.currentHealth;
                    break;
            }
        }

        public void SetInitialBarValues(string resourceName)
        {
            switch (resourceName)
            {
                case "Rage":
                    rageBar.maxValue = playerStats.maxRage;
                    rageBar.value = playerStats.currentRage;
                    break;
                case "Health":
                    healthBar.maxValue = playerStats.maxHealth;
                    healthBar.value = playerStats.currentHealth;
                    break;
            }
           
        }
    }
}

