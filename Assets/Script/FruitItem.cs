Created Consumable Resource System
using UnityEngine;

public class FruitItem : MonoBehaviour
{
    [Header("Settings")]
    public float healValue = 1.0f; // Amount of HP added per fruit

    // This runs when the Player walks into the fruit
    private void OnTriggerEnter(Collider other)
    {
        // 1. Check if the object has the "Player" tag
        if (other.CompareTag("Player"))
        {
            // 2. Try to find the PlayerHealth component on that object
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();

            // 3. If we found the script, heal the player and destroy the fruit
            if (playerHealth != null)
            {
                playerHealth.Heal(healValue);
                
                // Optional: Log to console to confirm it worked
                Debug.Log("Fruit consumed! +1 HP added to Player.");

                // 4. Make the fruit disappear
                Destroy(gameObject);
            }
            else
            {
                Debug.LogWarning("Fruit touched something tagged 'Player', but no PlayerHealth script was found!");
            }
        }
    }
}