using UnityEngine;
using UnityEngine.UI;

public class DayNightCycle : MonoBehaviour
{
    [Header("Time Settings")]
    public float phaseDuration = 60f; 
    
    [Header("Required References")]
    public Light sun;                
    public Text messageText;  

    [Header("Color Settings")]
    public Color dayColor = Color.white; 
    public Color nightColor = new Color(0.1f, 0.1f, 0.2f); 

    private float timer = 0f;
    private enum Phase { Day1, Night, Day2, End }
    private Phase currentPhase = Phase.Day1;

    void Start()
    {
        if (messageText != null) messageText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (currentPhase == Phase.End) return;

        timer += Time.deltaTime;
        float t = Mathf.Clamp01(timer / phaseDuration); 
        
        float smoothT = Mathf.SmoothStep(0, 1, t);

        switch (currentPhase)
        {
            case Phase.Day1:
                UpdateSun(Mathf.Lerp(50f, 180f, smoothT), Mathf.Lerp(0.7f, 0.05f, smoothT));
                if (timer >= phaseDuration) SwitchPhase(Phase.Night);
                break;

            case Phase.Night:
       
                UpdateSun(Mathf.Lerp(180f, 340f, smoothT), 0.05f);
                if (timer >= phaseDuration) SwitchPhase(Phase.Day2);
                break;

            case Phase.Day2:
               
                UpdateSun(Mathf.Lerp(-20f, 50f, smoothT), Mathf.Lerp(0.05f, 0.7f, smoothT));
                if (timer >= phaseDuration) WinGame();
                break;
        }
    }

    void SwitchPhase(Phase newPhase)
    {
        currentPhase = newPhase;
        timer = 0f;
    }

    void UpdateSun(float angle, float intensity)
    {
        if (sun == null) return;
        sun.transform.rotation = Quaternion.Euler(angle, -30f, 0f);
        sun.intensity = intensity;
        RenderSettings.ambientIntensity = intensity;

        float fogStrength = Mathf.InverseLerp(0.7f, 0.05f, intensity); 
    
    RenderSettings.fog = true; 
    RenderSettings.fogDensity = fogStrength * 0.05f; 
    RenderSettings.fogColor = Color.Lerp(dayColor, nightColor, fogStrength);
    }

    void WinGame()
    {
        currentPhase = Phase.End;
        if (messageText != null)
        {
            messageText.gameObject.SetActive(true);
            messageText.text = "CONGRATULATIONS!\nYou Survived!";
        }
        Time.timeScale = 0f; 
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}