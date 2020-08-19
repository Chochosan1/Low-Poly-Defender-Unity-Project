using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
/// <summary>
/// Attached to the spells canvas. A separate canvas is used in order to avoid redrawing the main canvas all the time due to spell icon 
/// animations. This script controlls the cooling down feedback process like icon mask overlay, number countdown and feedback animations.
/// </summary>
namespace Chochosan
{
    public class UI_Chochosan_Spells : MonoBehaviour
    {
        public static UI_Chochosan_Spells Instance;

        private void Awake()
        {
            if(Instance == null)
            {
                Instance = this;
            }           
        }

        private void Start()
        {
            rollAnim = rollCooldownImage.GetComponentInParent<Animator>();
            attack3Anim = attack3CooldownImage.GetComponentInParent<Animator>();
            attack1Anim = attack1Image.GetComponentInParent<Animator>();
            attack2Anim = attack2Image.GetComponentInParent<Animator>();
        }

        //ROLL
        [SerializeField]
        private Image rollCooldownImage;
        [SerializeField]
        private TextMeshProUGUI rollCooldownText;
        private bool rollCoolingDown = false;
        private float rollCooldownTimer, rollCooldown;
        private Animator rollAnim;

        //Attack3
        [SerializeField]
        private Image attack3CooldownImage;
        [SerializeField]
        private TextMeshProUGUI attack3CooldownText;
        private bool attack3CoolingDown = false;
        private float attack3CooldownTimer, attack3Cooldown;
        private Animator attack3Anim;

        //Attack1
        [SerializeField]
        private Image attack1Image;
        private Animator attack1Anim;

        //Attack2
        [SerializeField]
        private Image attack2Image;
        private Animator attack2Anim;

        // Update is called once per frame
        void Update()
        {
            if(rollCoolingDown)
            {
                rollCooldownTimer -= Time.deltaTime;
                rollCooldownText.text = rollCooldownTimer.ToString("F1");
                rollCooldownImage.fillAmount = rollCooldownTimer / rollCooldown;

                if (rollCooldownTimer <= 0)
                {               
                    rollCoolingDown = false;
                    rollCooldownText.gameObject.SetActive(false);
                    rollCooldownImage.gameObject.SetActive(false);
                    rollAnim.SetBool("TriggerFeedback_CooldownDone", true);
                }
            }

            if (attack3CoolingDown)
            {
                attack3CooldownTimer -= Time.deltaTime;
                attack3CooldownText.text = attack3CooldownTimer.ToString("F1");
                attack3CooldownImage.fillAmount = attack3CooldownTimer / attack3Cooldown;

                if (attack3CooldownTimer <= 0)
                {
                    attack3CoolingDown = false;
                    attack3CooldownText.gameObject.SetActive(false);
                    attack3CooldownImage.gameObject.SetActive(false);
                    attack3Anim.SetBool("TriggerFeedback_CooldownDone", true);
                }
            }
        }

        public void DisplayCooldown(string spellName, float cooldown)
        {
            switch (spellName)
            {
                case "Roll":
                    rollCooldownImage.fillAmount = 1;
                    rollCooldown = rollCooldownTimer = cooldown;                   
                    rollCooldownText.gameObject.SetActive(true);
                    rollCooldownImage.gameObject.SetActive(true);
                    rollCoolingDown = true;
                    break;
                case "Attack3":
                    attack3CooldownImage.fillAmount = 1;
                    attack3Cooldown = attack3CooldownTimer = cooldown;
                    attack3CooldownText.gameObject.SetActive(true);
                    attack3CooldownImage.gameObject.SetActive(true);
                    attack3CoolingDown = true;
                    break;
            }           
        }

        public void TriggerFeedback_AbilityUsed(string spellName)
        {
            switch (spellName)
            {
                case "Attack1":
                    attack1Anim.SetBool("TriggerFeedback_AbilityUsed", true);
                    break;
                case "Attack2":
                    attack2Anim.SetBool("TriggerFeedback_AbilityUsed", true);
                    break;
            }
            
        }
    }
}
