using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
/// <summary>
/// Attached to the cinemachine camera. Uses the amplitude gaine in the noise section to shake the camera. Must be prepared by hand beforehand.
///(in order to set the addition options in the camera rigs and to enable the noise)
/// </summary>
public class Screen_Shake : MonoBehaviour
{
    public static Screen_Shake Instance;
    private bool is_CurrentlyShaking;
    private CinemachineFreeLook vCamera; //free look cinemachine component
    private CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin0; //top rig of the free look camera
    private CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin1; //mid rig of the free look camera
    private CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin2; //bot rig of the free look camera
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        vCamera = GetComponent<CinemachineFreeLook>();
        cinemachineBasicMultiChannelPerlin0 = vCamera.GetRig(0).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        cinemachineBasicMultiChannelPerlin1 = vCamera.GetRig(1).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        cinemachineBasicMultiChannelPerlin2 = vCamera.GetRig(2).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    public void ShakeScreen(float duration, float magnitude)
    {
        if (!is_CurrentlyShaking)
        {
            StartCoroutine(Shake(duration, magnitude));
        }
    }

    private IEnumerator Shake(float duration, float magnitude)
    {
        is_CurrentlyShaking = true;

        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            cinemachineBasicMultiChannelPerlin0.m_AmplitudeGain = magnitude;
            cinemachineBasicMultiChannelPerlin1.m_AmplitudeGain = magnitude;
            cinemachineBasicMultiChannelPerlin2.m_AmplitudeGain = magnitude;
            elapsed += Time.deltaTime;

            yield return null;
        }
        cinemachineBasicMultiChannelPerlin0.m_AmplitudeGain = 0;
        cinemachineBasicMultiChannelPerlin1.m_AmplitudeGain = 0;
        cinemachineBasicMultiChannelPerlin2.m_AmplitudeGain = 0;
        is_CurrentlyShaking = false;
    }
}
