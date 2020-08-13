using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// Generates all quests and keeps track of them here.
/// Quest accepting and reward claiming happens in DialogueController.cs. 
/// Quest progress check happens upon killing an enemy (every enemy upon death checks it)
/// </summary>
public class QuestComponent : MonoBehaviour
{
    private TPMovement_Controller playerController;
    private Player_Inventory playerInventory;
    public TMPro.TextMeshProUGUI[] text_Quests;

    private List<Quest> quests;

    private void Start()
    {
        playerController = GetComponent<TPMovement_Controller>();
        quests = new List<Quest>
        {
            new Quest("EnemyDouble", 3, 10, 1, "Slay 3 enemy swordsmen. [Lazo] asks you to help him get rid of his molesters, will you help?"),
            new Quest("EnemyBow", 2, 0, 0, "Slay 2 enemy bowmen. [Khasto] can no longer watch his adorable puppies suffer everytime the bowmen practise their aim. ")
        };

        quests[0].ShouldUnlockLightBoots = true;
        quests[1].ShouldUnlockBow = true;
    }

    public void AcceptQuest(int questID, string questGiverName)
    {
        quests[questID].AcceptQuest(questGiverName); 
        playerController.SetQuestPanelState(true);
        int firstFreeQuest = Chochosan.UI_Chochosan.Instance.GetFirstFreeQuestText(); //find out the first free quest text out of the whole text array
        quests[questID].SetCurrentQuestText(firstFreeQuest); //the current quest will use the first free quest text as its own
        Chochosan.UI_Chochosan.Instance.SetQuestTextOccupied(firstFreeQuest); //mark this quest text as occupied so that other quests skip it
        text_Quests[firstFreeQuest].gameObject.SetActive(true); //activate the text element
        text_Quests[firstFreeQuest].text = quests[questID].questDescription; //change its text
        Debug.Log("ACCEPT QUEST CALLED");
    }

    public void ClaimQuestRewards(int questID)
    {
        if(!quests[questID].IsRewardsClaimed())
        {
            quests[questID].ClaimQuestRewards();
            text_Quests[quests[questID].GetCurrentQuestText()].gameObject.SetActive(false); //turn off the text element the current quest is holding
            Chochosan.UI_Chochosan.Instance.SetQuestTextFree(quests[questID].GetCurrentQuestText()); //set that text element to free so other quests can now use it
            Debug.Log("Rewards claimed.");
        }   
    }

    public bool IsQuestComplete(int questID)
    {
        return quests[questID].IsQuestComplete();
    }

    //called when an enemy dies in an EnemyAI_Controller
    public void CheckQuestProgressKill(EnemyAI_Controller eContr)
    {
        int currQuest = 0;

        foreach (Quest tempQuest in quests)
        {
            if (tempQuest.IsQuestAccepted() && !tempQuest.IsQuestComplete() && eContr.gameObject.name.Contains(tempQuest.enemyToSlayNameContains))
            {
                tempQuest.CurrentEnemiesKilled++;
                Debug.Log("Added to quest progress");

                //if quest is complete then do some stuff
                if (tempQuest.IsQuestComplete())
                {
                    text_Quests[tempQuest.GetCurrentQuestText()].text = $"Return to {tempQuest.GetQuestGiverName()} to claim your quest rewards."; //change the description of the quest
                }
            }
            currQuest++;
        }
    }

    private class Quest
    {
        public Quest(string enemyNameContains, int enemiesToKill, int goldReward, int skillPointsReward, string questDescription)
        {
            this.enemyToSlayNameContains = enemyNameContains;
            this.enemiesToKill = enemiesToKill;
            this.goldReward = goldReward;
            this.skillPointsReward = skillPointsReward;
            this.questDescription = questDescription;
        }


        //Quest objectives
        public string questDescription;
        public string enemyToSlayNameContains;
        private int enemiesToKill;
        private string questGiverName; //use for display purposes

        //Current quest progress
        private int currentEnemiesKilled = 0;
        public int CurrentEnemiesKilled { get; set; }

        //Quest status
        private bool questAccepted = false;
        private bool questComplete = false;
        private bool rewardsClaimed = false;
        private int currentQuestTextIndex; //caches which quest text slot the quest is using; upon quest completion that quest text slot must be freed


        //Quest rewards
        public bool ShouldUnlockBow { get; set; }
        public bool ShouldUnlockLightBoots { get; set; }
        private int goldReward;
        private int skillPointsReward;

        #region Methods
        public void AcceptQuest(string questGiverName)
        {
            questAccepted = true;
            this.questGiverName = questGiverName;
        }

        public void SetCurrentQuestText(int questTextIndex)
        {
            currentQuestTextIndex = questTextIndex;
        }

        public int GetCurrentQuestText()
        {
            return currentQuestTextIndex;
        }

        public string GetQuestGiverName()
        {
            return questGiverName;
        }

        public bool IsQuestAccepted()
        {
            return questAccepted;
        }

        public bool IsQuestComplete()
        {
            if (CurrentEnemiesKilled >= enemiesToKill)
            {
                questComplete = true;
            }
            //  Debug.Log("Quest complete: " + questComplete);
            return questComplete;
        }

        public bool IsRewardsClaimed()
        {
            return rewardsClaimed;
        }

        public void ClaimQuestRewards()
        {
            if (IsQuestComplete() && !rewardsClaimed)
            {
                rewardsClaimed = true;
                if (ShouldUnlockBow)
                {
                    Player_Inventory.instance.UnlockBow();
                    Debug.Log("Bow unlocked");
                }
                else if(ShouldUnlockLightBoots)
                {
                    Player_Inventory.instance.UnlockLightBoots();
                }
                Player_Inventory.instance.TotalGold += goldReward;
                Player_Inventory.instance.TotalSkillPoints += skillPointsReward;

                Debug.Log("Gold: " + Player_Inventory.instance.TotalGold + " || Points: " + Player_Inventory.instance.TotalSkillPoints);
            }
        }
        #endregion
    }
}
