using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Attached to the player. Scripts can then just call the PlaySound method to play the desired sound through their audio source.
/// </summary>
namespace Chochosan
{
    public class Sound_Manager : MonoBehaviour
    {
        public static Sound_Manager Instance;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        public enum Sounds
        {
            PlayerMove,
            PlayerSwordHit,
            PlayerBowHit,
            PlayerBowAttack1,
            PlayerBowAttack2,
            PlayerSwordDraw,
            PlayerBowDraw,
            EnemyDeath
        }

        [Tooltip("These values are used for every audiosource that uses the sound manager. Min and max distances are used to create a custom rolloff mode.")]
        public float minDistanceCustomRolloff = 10, maxDistanceCustomRolloff = 40;
         
     

        [SerializeField]
        private SO_Sound_Assets soundAssets;

        ////   private List<GameObject> spawnedSoundGameObjectsPool;
        //private Dictionary<GameObject, int> pooledObjectsAvailabilityDictionary;

        //[SerializeField]
        //private int maxSoundGameObjectPoolSize = 20;
        //private bool stillSpawningSoundGameObjects = true;


        //public void PlaySound(Sounds sound, Vector3 position)
        //{
        //    GameObject soundGameObject = new GameObject();
        //    soundGameObject.transform.position = position;
        //    AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();
        //    AddGameObjectToPool(soundGameObject);
        //    audioSource.clip = GetAudioClip(sound, audioSource);
        //    audioSource.Play();
        //}

        public void PlaySound(Sounds sound, AudioSource audioSource)
        {
            audioSource.clip = GetAudioClip(sound, audioSource);
            audioSource.Play();
        }

        private AudioClip GetAudioClip(Sounds sound, AudioSource audioSource)
        {
            switch (sound)
            {
                case Sounds.PlayerSwordHit:
                    return soundAssets.playerSwordHit;
                case Sounds.PlayerBowHit:
                    return soundAssets.playerBowHit;
                case Sounds.PlayerBowAttack1:
                    audioSource.volume = 0.6f;
                    return soundAssets.playerBowAttack1;
                case Sounds.PlayerBowAttack2:
                    audioSource.volume = 0.6f;
                    return soundAssets.playerBowAttack2;
                case Sounds.PlayerBowDraw:
                    audioSource.volume = 0.6f;
                    return soundAssets.bowDraw;
                case Sounds.PlayerSwordDraw:
                    audioSource.volume = 0.6f;
                    return soundAssets.swordDraw;
                case Sounds.EnemyDeath:
                    return soundAssets.enemyDeath;
                  
            }

            Debug.Log("No sound was found");
            return null;
        }

        //private void AddGameObjectToPool(GameObject objectToAdd)
        //{
        //    pooledObjectsAvailabilityDictionary.Add(objectToAdd, 0);

        //    if (pooledObjectsAvailabilityDictionary.Count >= maxSoundGameObjectPoolSize)
        //    {
        //        stillSpawningSoundGameObjects = false;
        //        //   Debug.Log("DISABLING INSTANTIATION!");
        //    }
        //}
    }
}
