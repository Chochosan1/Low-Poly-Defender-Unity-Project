using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Location : MonoBehaviour
{
    public static Player_Location instance;
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }
}
