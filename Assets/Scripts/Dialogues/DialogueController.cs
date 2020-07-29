using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Attached to every interactable object that needs to use the dialog system when interacted with.
/// </summary>
public class DialogueController : MonoBehaviour, IInteractable
{
    public string nameDisplay;
    [Tooltip("The messages that should be displayed when interacted with. Picked randomly.")]
    public string[] messages;
    [Tooltip("The range from which the player is able to press a key and interact with the object. Set to 0 if nothing should happen - this is mostly for story mode NPCs.")]
    public float interactionRange;
    [Tooltip("Delay between printing each letter.")]
    public float typeWriterDelay;
    [Tooltip("After the interaction is over and the text has been printed, wait this many seconds and close the panel.")]
    public float closeNonStoryNPCDialogPanelAfterSeconds;

    public GameObject dialogPanel;
    public UnityEngine.UI.Text dialogText;

    [Header("STORY MODE")]
    [Tooltip("If true then the messages will come in order one by one instead of randomly.")]
    public bool storyMode;
    [Tooltip("The minimum time between individual sentences of the NPC. The time is internally set to give the player 1 second for every 18 letters. If this time is too low then use the minTimeBetweenMessages.")]
    public float minTimeBetweenMessages;
    [Tooltip("After the NPC is done talking and the texts have been printed, wait this many seconds and close the panel.")]
    public float closeStoryDialogPanelAfterSeconds;
    [HideInInspector]
    public bool storyComplete = false;

    [Header("Quest System")]
    [Tooltip("Set to true if this object must give a quest to the player.")]
    public bool questGiver = false;
    [Tooltip("Specify the quest index that must be unlocked.")]
    public int questID;

    [Tooltip("Enable this if the interactable NPC should start the story if the player is close instead of pressing interact.")]
    public bool proximityMessagePop;
    private bool proximityTriggered = false;

    private string typeWriterString;
    private int currLetter;
    private int currMessageIndex = 0;
    private float originalTypeWriteDelay;
    private static bool is_Interacted = false; //static flag to control all objects, if one is currently being interacted with don't call the coroutine again

    private void Start()
    {
        originalTypeWriteDelay = typeWriterDelay; //save the original delay  

        //necessary for the questGiver to tell his story before giving the quest
        if(questGiver)
        {
            storyMode = true; 
        }
    }


    private void OnTriggerStay(Collider collision)
    {
        if (collision.transform.CompareTag("Player") && !proximityTriggered && storyMode && !storyComplete && !is_Interacted && proximityMessagePop)
        {
            proximityTriggered = true;
            //disable the collider when done so it does not mess up collsions with other objects
            //IMPORTANT: the to-be disabled collider must be placed first in the hierarchy in order to work as intended
            GetComponent<Collider>().enabled = false;
            StartCoroutine(StoryTypeWriterEffect());
        }
    }

    void IInteractable.Interact()
    {
        if (Vector3.Distance(TPMovement_Controller.instance.transform.position, transform.position) <= interactionRange)
        {
            if (!is_Interacted && !storyMode) //random NPC with no story, just says a random phrase
            {
                currMessageIndex = Random.Range(0, messages.Length); //pick a random message to say
                StartCoroutine(TypeWriterEffect());
            }
            else if (!is_Interacted && storyMode && !storyComplete && !proximityMessagePop && !questGiver) //story NPC that can be interacted with only once and does not give quests
            {
                StartCoroutine(StoryTypeWriterEffect());
            }
            else if (!is_Interacted && storyMode && !proximityMessagePop && questGiver) //gives you your quest rewards if quest is complete OR tells you a story just once and gives you a quest
            {
                if (Player_Location.instance.GetComponent<QuestComponent>().IsQuestComplete(questID))
                {
                    Player_Location.instance.GetComponent<QuestComponent>().ClaimQuestRewards(questID);
                }
                else
                {
                    if (!storyComplete)
                    {
                        StartCoroutine(StoryTypeWriterEffect()); //tells you a story and gives you a quest if tagged as a questgiver
                    }
                }
            }
            else //make the delay faster if the user is annoyed; allow him to smash the interaction key to see the whole text faster
            {
                typeWriterDelay /= 2;
            }
        }
    }

    //call this when the player manually interacts with something. Will type a random text.
    private IEnumerator TypeWriterEffect()
    {
        typeWriterDelay = originalTypeWriteDelay; //set the original delay back to what it was in case the user made if faster
        is_Interacted = true;
        dialogPanel.SetActive(true);
        while (currLetter < messages[currMessageIndex].Length)
        {
            typeWriterString += messages[currMessageIndex][currLetter];
            currLetter++;
            dialogText.text = typeWriterString;
            yield return new WaitForSeconds(typeWriterDelay);
        }

        //end and reset
        if (currLetter >= messages[currMessageIndex].Length)
        {
            typeWriterString = "";
            currLetter = 0;
        }

        yield return new WaitForSeconds(closeNonStoryNPCDialogPanelAfterSeconds);
        dialogPanel.SetActive(false);
        is_Interacted = false;
    }

    //called automatically by the game, not by the player. Will print all messages one by one as if the NPC is talking; will disable player controls
    //so that he can read the texts in story mode and when the NPC is done talking controls will resume
    private IEnumerator StoryTypeWriterEffect()
    {
        typeWriterDelay = originalTypeWriteDelay; //set the original delay back to what it was in case the user made if faster
        is_Interacted = true;
        //   GameState_Controller.reference.ChangeGameMode(GameStates.StoryMode);
        dialogPanel.SetActive(true);
        while (currMessageIndex < messages.Length && currLetter < messages[currMessageIndex].Length)
        {
            typeWriterString += messages[currMessageIndex][currLetter];
            currLetter++;
            dialogText.text = typeWriterString;
            yield return new WaitForSeconds(typeWriterDelay);

            //end and reset
            if (currLetter >= messages[currMessageIndex].Length)
            {
                currMessageIndex++;
                typeWriterDelay = originalTypeWriteDelay; //set the original delay back to what it was in case the user made if faster
                typeWriterString = "";
                currLetter = 0;
                int totalLetters = typeWriterString.Length;
                float timeToWaitForNextMessage = totalLetters / 18; //the average human can read 25 letters per second; white spaces are included here; adjust as needed
                if (timeToWaitForNextMessage < minTimeBetweenMessages)
                {
                    timeToWaitForNextMessage = minTimeBetweenMessages;
                }
                yield return new WaitForSeconds(timeToWaitForNextMessage);
            }
        }
        //  GameState_Controller.reference.ChangeGameMode(GameStates.PlayMode);
        yield return new WaitForSeconds(closeStoryDialogPanelAfterSeconds);
        dialogPanel.SetActive(false);
        is_Interacted = false;
        storyComplete = true;

        //the player should have the QuestComponent class
        if (questGiver && !Player_Location.instance.GetComponent<QuestComponent>().IsQuestComplete(questID))
        {
            Player_Location.instance.GetComponent<QuestComponent>().AcceptQuest(questID, nameDisplay);
        }
    }
}

