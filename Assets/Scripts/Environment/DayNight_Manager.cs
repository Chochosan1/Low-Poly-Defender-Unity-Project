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
    private float skyboxRotationSpeed = 1f;
    [SerializeField]
    [Tooltip("The main light.")]
    private Light sunLight;
    private Color32 daySunColor;
    private Color32 nightSunColor;
    private Color32 ambientDayColor;
    private Color32 ambientNightColor;
    private float fullNightSkyboxExposure = 0.9f;
    private float fullDaySkyboxExposure = 0.7f;
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
    private bool isShadowsOn;
    private float elapsedCycleTime = 0f;

    private CurrentTimeState currentTimeState;

    private void Start()
    {
        currentTimeState = CurrentTimeState.Day;

        daySunColor = new Color32(245, 241, 115, 255);
        nightSunColor = new Color32(19, 45, 85, 255);

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
        {
            if(currentTimeState == CurrentTimeState.Night)
            {
                nightSkybox.SetFloat("_Rotation", nightSkybox.GetFloat("_Rotation") + (Time.deltaTime * skyboxRotationSpeed));
            }
            return;
        }
 

        elapsedCycleTime += Time.deltaTime;
        
        if (currentTimeState == CurrentTimeState.Day && elapsedCycleTime <= cycleDuration)
        {
            //if lights are on and the night skybox is on during the day phase then reduce the exposure before officially 
            //switching to night state
            if (isLightsOn)
            {
                nightSkybox.SetFloat("_Exposure", Mathf.Lerp(nightSkybox.GetFloat("_Exposure"), fullNightSkyboxExposure, Time.deltaTime / cycleDuration * 6));
                nightSkybox.SetFloat("_Rotation", nightSkybox.GetFloat("_Rotation") + (Time.deltaTime * skyboxRotationSpeed));
            }
            else
            {
                daySkybox.SetFloat("_Exposure", Mathf.Lerp(daySkybox.GetFloat("_Exposure"), 0.2f, Time.deltaTime / cycleDuration * 3));
                daySkybox.SetFloat("_Rotation", daySkybox.GetFloat("_Rotation") + (Time.deltaTime * skyboxRotationSpeed));
            }
            //lerp from Day brightness to Night darkness
            RenderSettings.ambientLight = Color.Lerp(RenderSettings.ambientLight, ambientNightColor, Time.deltaTime / cycleDuration);
            sunLight.color = Color.Lerp(sunLight.color, nightSunColor, Time.deltaTime / cycleDuration * 2);
            RenderSettings.reflectionIntensity = Mathf.Lerp(RenderSettings.reflectionIntensity, 0, Time.deltaTime / cycleDuration * 2);
          
            //after a certain stage is reached and if the lights are off then turn them on although it is still during the day phase 
            //and put on the night skybox with a higher than normal exposure (to make a seamless transition) although it is still during the day phase
            if (elapsedCycleTime / cycleDuration >= 0.8 && !isLightsOn)
            {
                foreach (Light light in lightsList)
                {
                    light.gameObject.SetActive(true);
                }
                isLightsOn = true;
                sunLight.shadows = LightShadows.Hard;
                isShadowsOn = false;
                RenderSettings.skybox = nightSkybox;
                nightSkybox.SetFloat("_Exposure", 1f);    
            }

            
        }
        else if(currentTimeState == CurrentTimeState.Night && elapsedCycleTime <= cycleDuration)
        {
            //if lights are off and the day skybox is on during the night phase then increase the exposure before officially 
            //switching to day state
            if (!isLightsOn)
            {
                daySkybox.SetFloat("_Exposure", Mathf.Lerp(daySkybox.GetFloat("_Exposure"), fullDaySkyboxExposure, Time.deltaTime / cycleDuration * 6));
                daySkybox.SetFloat("_Rotation", daySkybox.GetFloat("_Rotation") + (Time.deltaTime * skyboxRotationSpeed));
            }
            else
            {
                nightSkybox.SetFloat("_Exposure", Mathf.Lerp(nightSkybox.GetFloat("_Exposure"), 2f, Time.deltaTime / cycleDuration * 4));
                nightSkybox.SetFloat("_Rotation", nightSkybox.GetFloat("_Rotation") + (Time.deltaTime * skyboxRotationSpeed));
            }
            //lerp from Night darkness to Day brightness
            RenderSettings.ambientLight = Color.Lerp(RenderSettings.ambientLight, ambientDayColor, Time.deltaTime / cycleDuration);
            sunLight.color = Color.Lerp(sunLight.color, daySunColor, Time.deltaTime / cycleDuration * 2);
            RenderSettings.reflectionIntensity = Mathf.Lerp(RenderSettings.reflectionIntensity, 1, Time.deltaTime / cycleDuration * 2);
           

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
                daySkybox.SetFloat("_Exposure", 0.4f); //0.75
            }

            if(elapsedCycleTime / cycleDuration >= 0.1 && !isShadowsOn)
            {
                isShadowsOn = true;
                sunLight.shadows = LightShadows.Soft;
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
        nightSkybox.SetFloat("_Exposure", fullNightSkyboxExposure);
        daySkybox.SetFloat("_Exposure", fullDaySkyboxExposure);
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
        daySkybox.SetFloat("_Exposure", fullDaySkyboxExposure);
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
        sunLight.shadows = LightShadows.Hard;
        RenderSettings.skybox = nightSkybox;
        nightSkybox.SetFloat("_Exposure", fullNightSkyboxExposure);
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
