using UnityEngine;
using UnityEngine.UI;

public class DayNightCycle : MonoBehaviour
{
    public float phaseDuration = 60f;

    public Light sun;
    public Light moon;
    public Material skyboxMaterial;
    public GameObject moonVisual;
    
    // --- ADDED THIS LINE ---
    public GameObject starSystem; 

    [Header("UI")]
    public Text messageText;

    [Header("Colors")]
    public Color nightColor = new Color(0.005f, 0.015f, 0.005f, 1f);
    public Color dayColor = Color.white;

    [Header("Fog")]
    public bool useFog = true;
    public Color nightFogColor = new Color(0f, 0.01f, 0f, 1f);
    public float nightFogDensity = 0.02f;

    private float timer = 0f;

    private enum Phase
    {
        Day1,
        Night,
        Day2,
        End
    }

    private Phase currentPhase = Phase.Day1;

    void Start()
    {
        SetDayVisuals(1f);
        
        if (messageText != null)
        {
            messageText.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        timer += Time.deltaTime;

        float t = timer / phaseDuration;
        float intensity = 0f;

        switch (currentPhase)
        {
            case Phase.Day1:
                intensity = Mathf.Lerp(1f, 0f, t);
                if (timer >= phaseDuration) SwitchPhase(Phase.Night);
                break;

            case Phase.Night:
                intensity = 0f; 
                if (timer >= phaseDuration) SwitchPhase(Phase.Day2);
                break;

            case Phase.Day2:
                intensity = Mathf.Lerp(0f, 1f, t);
                if (timer >= phaseDuration) WinGame();
                break;

            case Phase.End:
                return;
        }

        UpdateLighting(intensity);
    }

    void SwitchPhase(Phase newPhase)
    {
        currentPhase = newPhase;
        timer = 0f;
    }

    void WinGame()
    {
        currentPhase = Phase.End;

        if (messageText != null)
        {
            messageText.gameObject.SetActive(true);
            messageText.text = "CONGRATULATIONS!\nYou have successfully survived the island!";
            messageText.color = Color.yellow; 
            messageText.horizontalOverflow = HorizontalWrapMode.Overflow;
            messageText.verticalOverflow = VerticalWrapMode.Overflow;
            messageText.alignment = TextAnchor.MiddleCenter;
        }

        Debug.Log("CONGRATULATIONS! You survived the island!");

        sun.intensity = 1f;
        moon.intensity = 0f;
        if (moonVisual != null) moonVisual.SetActive(false);
        
        // --- HIDE STARS ON WIN ---
        if (starSystem != null) starSystem.SetActive(false); 

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Time.timeScale = 0f; 
    }

    void UpdateLighting(float intensity)
    {
        float sunAngle = (intensity * 180f) - 90f;
        sun.transform.rotation = Quaternion.Euler(sunAngle, -30f, 0f);
        moon.transform.rotation = Quaternion.Euler(sunAngle + 180f, -30f, 0f);

        sun.intensity = intensity;

        // --- MOON AND STAR VISIBILITY LOGIC ---
        if (currentPhase == Phase.Night)
        {
            moon.intensity = 1.0f;
            if (moonVisual != null) moonVisual.SetActive(true);
            
            // --- SHOW STARS AT NIGHT ---
            if (starSystem != null) starSystem.SetActive(true); 
        }
        else
        {
            moon.intensity = 0f;
            if (moonVisual != null) moonVisual.SetActive(false);
            
            // --- HIDE STARS DURING DAY ---
            if (starSystem != null) starSystem.SetActive(false); 
        }

        RenderSettings.ambientLight = Color.Lerp(nightColor, dayColor, intensity);

        if (useFog)
        {
            RenderSettings.fog = true;
            RenderSettings.fogColor = Color.Lerp(nightFogColor, Color.white, intensity);
            RenderSettings.fogDensity = Mathf.Lerp(nightFogDensity, 0.0001f, intensity);
        }

        if (skyboxMaterial != null)
        {
            skyboxMaterial.SetFloat("_Exposure", Mathf.Lerp(0.01f, 1.3f, intensity));
        }

        DynamicGI.UpdateEnvironment();
    }

    void SetDayVisuals(float intensity)
    {
        sun.intensity = intensity;
        moon.intensity = 0f;
        if (moonVisual != null) moonVisual.SetActive(false);
        
        // --- ENSURE STARS HIDDEN AT START ---
        if (starSystem != null) starSystem.SetActive(false); 

        RenderSettings.ambientLight = dayColor;
    }
}