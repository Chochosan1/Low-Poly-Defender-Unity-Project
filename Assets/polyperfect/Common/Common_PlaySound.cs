using UnityEngine;
/// <summary>
/// The pack originally offers the same min/max distances for all animals. The code has been changed in a way that allows every single
/// spawned animal in the world to have its own custom min/max distances with a custom rolloff curve in Common_AudioManager.cs.
/// </summary>
namespace PolyPerfect
{
    public class Common_PlaySound : MonoBehaviour
    {
        [SerializeField]
        private AudioClip animalSound;
        [SerializeField]
        private AudioClip walking;
        [SerializeField]
        private AudioClip eating;
        [SerializeField]
        private AudioClip running;
        [SerializeField]
        private AudioClip attacking;
        [SerializeField]
        private AudioClip death;
        [SerializeField]
        private AudioClip sleeping;
        [SerializeField]
        private float minSoundDistance;
        [SerializeField]
        private float maxSoundDistance;

        void AnimalSound()
        {
            if (animalSound)
            {
                Common_AudioManager.PlaySound(animalSound, transform.position, minSoundDistance, maxSoundDistance);
            }
        }

        void Walking()
        {
            if (walking)
            {
                Common_AudioManager.PlaySound(walking, transform.position, minSoundDistance, maxSoundDistance);
            }
        }

        void Eating()
        {
            if (eating)
            {
                Common_AudioManager.PlaySound(eating, transform.position, minSoundDistance, maxSoundDistance);
            }
        }

        void Running()
        {
            if (running)
            {
                Common_AudioManager.PlaySound(running, transform.position, minSoundDistance, maxSoundDistance);
            }
        }

        void Attacking()
        {
            if (attacking)
            {
                Common_AudioManager.PlaySound(attacking, transform.position, minSoundDistance, maxSoundDistance);
            }
        }

        void Death()
        {
            if (death)
            {
                Common_AudioManager.PlaySound(death, transform.position, minSoundDistance, maxSoundDistance);
            }
        }

        void Sleeping()
        {
            if (sleeping)
            {
                Common_AudioManager.PlaySound(sleeping, transform.position, minSoundDistance, maxSoundDistance);
            }
        }
    }
}