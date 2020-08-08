using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CurrentTimeState { Day, Night}
/// <summary>
/// Lerped day/night cycle based on a duration. Context menu item created for an easy reset to the original form of the shaders.
/// </summary>
public class DayNight_Manager : MonoBehaviour
{
    public static DayNight_Manager Instance;

    [SerializeField]
    [Tooltip("All lights that must be enabled/disabled when swapping from day to night and vice versa.")]
    private List<Light> lightsList;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }

    [SerializeField]
    private Material nightSkybox, daySkybox;
    [SerializeField]
    [Tooltip("The main light.")]
    private Light sunLight;
    private Color32 daySunColor;
    private Color32 nightSunColor;
    private Color32 ambientDayColor;
    private Color32 ambientNightColor;
    [SerializeField]
    [Tooltip("How long it will take to swap between day and night.")]
    private float cycleDuration = 5f;
    [SerializeField]
    [Range(0, 1)]
    [Tooltip("After switching to another time state (day/night), the cycle will pause for a certain duration (in order to keep the game longer in the darkest/brightest state). It is a percentage based on the cycle duration time.")]
    private float cyclePauseMultiplier = 0.2f;
    private float cyclePauseDuration; //the final result after applying the pauseMultiplier to the duration
    private bool isCurrentlyPaused;
    private bool isLightsOn;
    private float elapsedCycleTime = 0f;

    private CurrentTimeState currentTimeState;

    private void Start()
    {
        currentTimeState = CurrentTimeState.Day;

        daySunColor = new Color32(245, 241, 115, 255);
        nightSunColor = new Color32(0, 0, 0, 255);

        ambientDayColor = new Color32(152, 152, 152, 255);
        ambientNightColor = new Color32(30, 30, 30, 255);

        cyclePauseDuration = cycleDuration * cyclePauseMultiplier;
        Debug.Log("Pause is " + cyclePauseDuration);
        isCurrentlyPaused = false;
    }

    private void Update()
    {
        //if(Input.GetKeyDown(KeyCode.N))
        //{
        //    ToggleTimeState();
        //}
        if (isCurrentlyPaused)
            return;

        elapsedCycleTime += Time.deltaTime;
        
        if (currentTimeState == CurrentTimeState.Day && elapsedCycleTime <= cycleDuration)
        {
            //lerp from Day brightness to Night darkness
            RenderSettings.ambientLight = Color.Lerp(RenderSettings.ambientLight, ambientNightColor, Time.deltaTime / cycleDuration);
            sunLight.color = Color.Lerp(sunLight.color, nightSunColor, Time.deltaTime / cycleDuration * 2);
            RenderSettings.reflectionIntensity = Mathf.Lerp(RenderSettings.reflectionIntensity, 0, Time.deltaTime / cycleDuration * 2);
            daySkybox.SetFloat("_Exposure", Mathf.Lerp(daySkybox.GetFloat("_Exposure"), 0.2f, Time.deltaTime / cycleDuration * 2));

            //after a certain stage is reached and if the lights are off then turn them on although it is still during the day phase 
            if(elapsedCycleTime / cycleDuration >= 0.8 && !isLightsOn)
            {
                foreach (Light light in lightsList)
                {
                    light.gameObject.SetActive(true);
                }
                isLightsOn = true;
            }
        }
        else if(currentTimeState == CurrentTimeState.Night && elapsedCycleTime <= cycleDuration)
        {
            //lerp from Night darkness to Day brightness
            RenderSettings.ambientLight = Color.Lerp(RenderSettings.ambientLight, ambientDayColor, Time.deltaTime / cycleDuration);
            sunLight.color = Color.Lerp(sunLight.color, daySunColor, Time.deltaTime / cycleDuration * 2);
            RenderSettings.reflectionIntensity = Mathf.Lerp(RenderSettings.reflectionIntensity, 1, Time.deltaTime / cycleDuration * 2);
            nightSkybox.SetFloat("_Exposure", Mathf.Lerp(nightSkybox.GetFloat("_Exposure"), 2f, Time.deltaTime / cycleDuration * 4));

            //after a certain stage is reached and if the lights are on then turn them off and put on the day skybox 
            //with a lower than normal exposure (to make a seamless transition) although it is still during the night phase
            if (elapsedCycleTime / cycleDuration >= 0.65 && isLightsOn)
            {
                foreach (Light light in lightsList)
                {
                    light.gameObject.SetActive(false);
                }
                isLightsOn = false;
                RenderSettings.skybox = daySkybox;
                daySkybox.SetFloat("_Exposure", 0.75f);
            }

            //if lights are off and the day skybox is on during the night phase then increase the exposure before officially 
            //switching to day state
            if(!isLightsOn)
            {
                daySkybox.SetFloat("_Exposure", Mathf.Lerp(daySkybox.GetFloat("_Exposure"), 1.2f, Time.deltaTime / cycleDuration * 3));
            }
        }
        else
        {      
            //switch the time state and set the settings accordingly in case something has not lerped fully to the desired values
            //(lerping will still make the transitions smooth)
            if(currentTimeState == CurrentTimeState.Day)
            {
                SetNightSettings();
            }
            else
            {
                SetDaySettings();
            }
            elapsedCycleTime = 0f;
            StartCoroutine(PauseAndResumeCycleAfter(cyclePauseDuration));
        }
    }

    //right click on the script to quickly reset the exposures through the editor
    [ContextMenu("Chochosan/Reset skybox exposures")]
    public void ResetSkyboxExposures()
    {
        nightSkybox.SetFloat("_Exposure", 0.7f);
        daySkybox.SetFloat("_Exposure", 1.2f);
        Debug.Log("ChochosanTools: Skybox exposures reset.");
    }

    private void ToggleTimeState()
    {
        switch(currentTimeState)
        {
            case CurrentTimeState.Day:
                SetNightSettings();
                break;
            case CurrentTimeState.Night:
                SetDaySettings();
                break;
        }
    }

    private void SetDaySettings()
    {
        SetCurrentTimeState(CurrentTimeState.Day);
        RenderSettings.reflectionIntensity = 1;
        sunLight.shadows = LightShadows.Soft;
        RenderSettings.skybox = daySkybox;
        daySkybox.SetFloat("_Exposure", 1.2f);
        foreach (Light light in lightsList)
        {
            light.gameObject.SetActive(false);
        }
        Debug.Log("ITS DAY");
    }

    private void SetNightSettings()
    {
        SetCurrentTimeState(CurrentTimeState.Night);
        RenderSettings.reflectionIntensity = 0;
        sunLight.shadows = LightShadows.None;
        RenderSettings.skybox = nightSkybox;
        nightSkybox.SetFloat("_Exposure", 0.7f);
        foreach (Light light in lightsList)
        {
            light.gameObject.SetActive(true);
        }
        Debug.Log("ITS NIGHT");
    }

    private IEnumerator PauseAndResumeCycleAfter(float pauseDuration)
    {
        isCurrentlyPaused = true;
        Debug.Log("Cycle paused.");
        yield return new WaitForSeconds(pauseDuration);
        Debug.Log("Cycle resumed");
        isCurrentlyPaused = false;
    }

    private void SetCurrentTimeState(CurrentTimeState timeState) => currentTimeState = timeState;
    public CurrentTimeState GetCurrentTimeState() => currentTimeState;
}
