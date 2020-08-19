using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
        }

        //ROLL
        [SerializeField]
        private Image rollCooldownImage;
        [SerializeField]
        private TextMeshProUGUI rollCooldownText;
        private bool rollCoolingDown = false;
        private float rollCooldownTimer, rollCooldown;
        private Animator rollAnim;

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
                    rollAnim.SetBool("TriggerFeedback", true);
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
            }           
        }
    }
}
