using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CurrentTimeState { Day, Night}
/// <summary>
/// TODO: LERP SHADER SKYBOX EXPOSURE AS WELL
/// </summary>
public class DayNight_Manager : MonoBehaviour
{
    public static DayNight_Manager Instance;

    [SerializeField]
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
    private Light sunLight;
    private Color32 daySunColor;
    private Color32 nightSunColor;
    private Color32 ambientDayColor;
    private Color32 ambientNightColor;
    [SerializeField]
    private float cycleDuration = 5f;
    private float elapsedCycleTime = 0f;
    float step = 0f;

    private CurrentTimeState currentTimeState;

    private void Start()
    {
        currentTimeState = CurrentTimeState.Day;

        daySunColor = new Color32(245, 241, 115, 255);
        nightSunColor = new Color32(0, 0, 0, 255);

        ambientDayColor = new Color32(152, 152, 152, 255);
        ambientNightColor = new Color32(90, 90, 90, 255);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.N))
        {
            ToggleTimeState();
        }

        elapsedCycleTime += Time.deltaTime;
        

        if (currentTimeState == CurrentTimeState.Day && elapsedCycleTime <= cycleDuration)
        {
            RenderSettings.ambientLight = Color.Lerp(RenderSettings.ambientLight, ambientNightColor, Time.deltaTime / cycleDuration);
            sunLight.color = Color.Lerp(sunLight.color, nightSunColor, Time.deltaTime / cycleDuration);
            daySkybox.SetFloat("_Exposure", Mathf.Lerp(daySkybox.GetFloat("_Exposure"), 0.2f, Time.deltaTime / cycleDuration));
        }
        else if(currentTimeState == CurrentTimeState.Night && elapsedCycleTime <= cycleDuration)
        {
            RenderSettings.ambientLight = Color.Lerp(RenderSettings.ambientLight, ambientDayColor, Time.deltaTime / cycleDuration);
            sunLight.color = Color.Lerp(sunLight.color, daySunColor, Time.deltaTime / cycleDuration);
            nightSkybox.SetFloat("_Exposure", Mathf.Lerp(daySkybox.GetFloat("_Exposure"), 1.2f, Time.deltaTime / cycleDuration));
        }
        else
        {      
            if(currentTimeState == CurrentTimeState.Day)
            {
                currentTimeState = CurrentTimeState.Night;
                RenderSettings.reflectionIntensity = 0;
                sunLight.shadows = LightShadows.Hard;
                RenderSettings.skybox = nightSkybox;
                nightSkybox.SetFloat("_Exposure", 0.78f);
                foreach (Light light in lightsList)
                {
                    light.gameObject.SetActive(true);
                }
                Debug.Log("ITS NIGHT");
            }
            else
            {
                currentTimeState = CurrentTimeState.Day;
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
            step = 0f;
            elapsedCycleTime = 0f;
            Debug.Log("CYCLE RESET");
        }
        
            
        //if (elapsedCycleTime >= cycleDuration)
        //{
        //    elapsedCycleTime = 0f;
        //    // ToggleTimeState();
        //}
    }

    public void ToggleTimeState()
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
        RenderSettings.ambientLight = ambientDayColor;
        RenderSettings.reflectionIntensity = 1;
        sunLight.color = daySunColor;
        sunLight.shadows = LightShadows.Soft;
        RenderSettings.skybox = daySkybox;
        currentTimeState = CurrentTimeState.Day;

        foreach(Light light in lightsList)
        {
            light.gameObject.SetActive(false);
        }
    }

    private void SetNightSettings()
    {      
        RenderSettings.ambientLight = ambientNightColor;
        RenderSettings.reflectionIntensity = 0;
        sunLight.color = nightSunColor;
        sunLight.shadows = LightShadows.None;
        RenderSettings.skybox = nightSkybox;
        currentTimeState = CurrentTimeState.Night;

        foreach (Light light in lightsList)
        {
            light.gameObject.SetActive(true);
        }
    }

    private void SetCurrentTimeState(CurrentTimeState timeState)
    {
        currentTimeState = timeState;
    }
}
