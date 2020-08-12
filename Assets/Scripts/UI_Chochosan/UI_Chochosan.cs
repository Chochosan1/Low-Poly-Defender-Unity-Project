using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chochosan
{
    public class UI_Chochosan : MonoBehaviour
    {
        public void TogglePanel(GameObject panel)
        {
            panel.SetActive(!panel.activeSelf);
        }
    }
}

