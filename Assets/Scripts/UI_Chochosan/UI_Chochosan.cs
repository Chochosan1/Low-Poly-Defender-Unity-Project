using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
/// <summary>
/// Controls the updating of the UI texts. Attached to the canvas component.
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
    }
}

