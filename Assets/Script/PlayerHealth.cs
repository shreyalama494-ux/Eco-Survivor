using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float health = 2.5f;     
    public float maxHealth = 20.0f; 
    public Text hpDisplay; 

    [Header("Hunger Settings")]
    public float healthDropRate = 0.01f; // Losing HP very slowly
    public bool isStarving = true;

    private float gracePeriodTimer = 0f; // Timer for the 3-minute pause

    void Start()
    {
        if (hpDisplay != null) hpDisplay.gameObject.SetActive(true);
        UpdateUI();
    }

    void Update()
    {
        // 1. Handle the 3-minute Grace Period Timer
        if (gracePeriodTimer > 0)
        {
            gracePeriodTimer -= Time.deltaTime;
        }
        // 2. Only drop health if the timer has run out
        else if (isStarving && health > 0)
        {
            health -= healthDropRate * Time.deltaTime;
            UpdateUI();

            if (health <= 0) Die();
        }
    }

    public void Heal(float amount)
    {
        health += amount;
        if (health > maxHealth) health = maxHealth;

        // NEW: Set the timer to 180 seconds (3 minutes) whenever fruit is eaten
        gracePeriodTimer = 180f; 
        
        Debug.Log("Ate fruit! Hunger paused for 3 minutes.");
        UpdateUI();
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        UpdateUI();
        if (health <= 0) Die();
    }

    void Die()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1f; 
    }

    void UpdateUI()
    {
        if (hpDisplay != null)
        {
            hpDisplay.text = "HP: " + Mathf.Max(health, 0).ToString("F1");
            
            // Visual feedback: If timer is active, maybe make HP green to show it's "Safe"
            if (gracePeriodTimer > 0) hpDisplay.color = Color.green;
            else if (health < 1.5f) hpDisplay.color = Color.red;
            else hpDisplay.color = Color.white;
        }
    }
}