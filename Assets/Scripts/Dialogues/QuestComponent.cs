using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// Generates all quests and keeps track of them.
/// </summary>
public class QuestComponent : MonoBehaviour
{
    public GameObject questPanel;
    public Text[] text_Quests;

    private List<Quest> quests;

    private void Start()
    {
        quests = new List<Quest>
        {
            new Quest("EnemyDouble", 3, "Slay 3 double sworded enemies"),
            new Quest("EnemyBow", 2, "Slay 2 bowmen")
        };

        quests[1].ShouldUnlockBow = true;
    }
   
    public void AcceptQuest(int questID)
    {
        quests[questID].AcceptQuest();
        questPanel.SetActive(true);
        text_Quests[questID].gameObject.SetActive(true);
        text_Quests[questID].text = quests[questID].questDescription;
        Debug.Log("ACCEPT QUEST CALLED");
    }

    public void CheckQuestProgressKill(EnemyAI_Controller eContr)
    {
        int currQuest = 0;

        foreach (Quest tempQuest in quests)
        {
            if (tempQuest.GetQuestAcceptedStatus() && !tempQuest.GetCurrentQuestCompleteStatus() && eContr.gameObject.name.Contains(tempQuest.enemyToSlayNameContains))
            {
                tempQuest.CurrentEnemiesKilled++;
                Debug.Log("Added to quest progress");
                if (tempQuest.GetCurrentQuestCompleteStatus())
                {
                    text_Quests[currQuest].gameObject.SetActive(false);
                    if(tempQuest.ShouldUnlockBow)
                    {
                        GetComponent<TPMovement_Controller>().UnlockBow();
                    }
                }
            }
            currQuest++;
        }


    }


    private class Quest
    {
        public Quest(string enemyNameContains, int enemiesToKill, string questDescription)
        {
            this.enemyToSlayNameContains = enemyNameContains;
            this.enemiesToKill = enemiesToKill;
            this.questDescription = questDescription;
        }

        //Quest objectives
        public string questDescription;
        public string enemyToSlayNameContains;
        private int enemiesToKill;
        private string NpcToTalkToNameContains;

        //Current quest progress
        private int currentEnemiesKilled = 0;
        public int CurrentEnemiesKilled { get; set; }

        //Quest status
        private bool questAccepted = false;
        private bool questComplete = false;


        //Quest rewards
        public bool ShouldUnlockBow { get; set; }

        #region Methods
        public void AcceptQuest()
        {
            questAccepted = true;
        }

        public bool GetQuestAcceptedStatus()
        {
            return questAccepted;
        }

        public bool GetCurrentQuestCompleteStatus()
        {
            if (CurrentEnemiesKilled >= enemiesToKill)
            {
                questComplete = true;
            }
          //  Debug.Log("Quest complete: " + questComplete);
            return questComplete;
        }
        #endregion
    }
}
