using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// The pack itself originally offers a logarithmic falloff, which does not work as I want it to so a custom solution has been
/// added to the existing code.
/// </summary>
namespace PolyPerfect
{
    public class Common_AudioManager : MonoBehaviour
    {
        private static Common_AudioManager instance;
        [SerializeField]
        private bool muteSound;

        [SerializeField]
        private int objectPoolLength = 20;

        [SerializeField]
        private float minSoundDistance = 7f;
        [SerializeField]
        private float maxSoundDistance = 14f;

        [SerializeField]
        private bool logSounds = false;

        private List<AudioSource> pool = new List<AudioSource>();

        private void Awake()
        {
            instance = this;

          /*  for (int i = 0; i < objectPoolLength; i++)
            {
                GameObject soundObject = new GameObject();
                soundObject.transform.SetParent(instance.transform);
                soundObject.name = "Sound Effect" + i;
                AudioSource audioSource = soundObject.AddComponent<AudioSource>();
                audioSource.rolloffMode = AudioRolloffMode.Linear;
                audioSource.spatialBlend = 1f;
                audioSource.minDistance = instance.minSoundDistance;
                audioSource.maxDistance = instance.maxSoundDistance;
                audioSource.gameObject.SetActive(false);
                pool.Add(audioSource);
            }*/
        }

        public static void PlaySound(AudioClip clip, Vector3 pos, float animalMinSoundDistance, float animalMaxSoundDistance)
        {
            if (!instance)
            {
                Debug.LogError("No Audio Manager found in the scene.");
                return;
            }

            if (instance.muteSound)
            {
                return;
            }

            if (!clip)
            {
                Debug.LogError("Clip is null");
                return;
            }

            if (instance.logSounds)
            {
                Debug.Log("Playing Audio: " + clip.name);
            }

            for (int i = 0; i < instance.pool.Count; i++)
            {
                if (!instance.pool[i].gameObject.activeInHierarchy)
                {
                    //apply the specific animal distances and calculate the curve upon using an ALREADY existing audio source from the pool
                    //a.k.a override the current settings with the animal specifics
                    var AS = instance.pool[i];
                    AS.minDistance = animalMinSoundDistance;
                    AS.maxDistance = animalMaxSoundDistance;
                    AS.spatialBlend = 1.0f;
                    var animationCurve = new AnimationCurve( //non-linear rolloff but it actually does not produce sound after the max distance (unlike the logarithmic rolloff)
                    new Keyframe(AS.minDistance, 1f),
                    new Keyframe(AS.minDistance + (AS.maxDistance - AS.minDistance) / 4f, .35f),
                    new Keyframe(AS.maxDistance, 0f));
                    AS.rolloffMode = AudioRolloffMode.Custom;
                    animationCurve.SmoothTangents(1, .025f);
                    AS.SetCustomCurve(AudioSourceCurveType.CustomRolloff, animationCurve);


                    instance.pool[i].clip = clip;
                    instance.pool[i].transform.position = pos;
                    instance.pool[i].gameObject.SetActive(true);
                    instance.pool[i].Play();
                    instance.StartCoroutine(instance.ReturnToPool(instance.pool[i].gameObject, clip.length));
                    return;
                }
            }

            //create a new audiosource if no free one is found in the pool with the settings below
            GameObject soundObject = new GameObject();
            soundObject.transform.SetParent(instance.transform);
            soundObject.name = "Sound Effect";
            AudioSource audioSource = soundObject.AddComponent<AudioSource>();

            //set sound distances and calculate the curve (logarithmic by default does not work properly so I use a custom solution)
            audioSource.minDistance = animalMinSoundDistance;
            audioSource.maxDistance = animalMaxSoundDistance;
            audioSource.spatialBlend = 1.0f;
            var animCurve = new AnimationCurve(
            new Keyframe(audioSource.minDistance, 1f),
            new Keyframe(audioSource.minDistance + (audioSource.maxDistance - audioSource.minDistance) / 4f, .35f),
            new Keyframe(audioSource.maxDistance, 0f));
            audioSource.rolloffMode = AudioRolloffMode.Custom;
            animCurve.SmoothTangents(1, .025f);
            audioSource.SetCustomCurve(AudioSourceCurveType.CustomRolloff, animCurve);
           
            audioSource.reverbZoneMix = 1.0f;
            audioSource.dopplerLevel = 1.0f;
            audioSource.spread = 1.0f;
           
            instance.pool.Add(audioSource);
            audioSource.clip = clip;
            soundObject.transform.position = pos;
            audioSource.Play();
            instance.StartCoroutine(instance.ReturnToPool(soundObject, clip.length));
        }

        private IEnumerator ReturnToPool(GameObject obj, float delay)
        {
            yield return new WaitForSeconds(delay);
            obj.SetActive(false);
        }
    }
}