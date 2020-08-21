using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Sound Asset", menuName = "Chochosan/Sounds/Sound_Asset")]
public class SO_Sound_Assets : ScriptableObject
{
    [Header("Player")]
    public AudioClip playerMoveFootsteps;

    public AudioClip playerSwordHit;

    public AudioClip playerBowHit;

    public AudioClip playerBowAttack1;

    public AudioClip playerBowAttack2;

    public AudioClip swordDraw;

    public AudioClip bowDraw;

    [Header("Enemy")]
    public AudioClip enemyDeath;

}
