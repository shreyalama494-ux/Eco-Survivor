Created Consumable Resource System
using UnityEngine;

public class FruitItem : MonoBehaviour
{
    [Header("Settings")]
    public float healValue = 1.0f;
    private void OnTriggerEnter(Collider other)
    {
       
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.Heal(healValue);
                Debug.Log("Fruit consumed! +1 HP added to Player.");
                Destroy(gameObject);
            }
            else
            {
                Debug.LogWarning("Fruit touched something tagged 'Player', but no PlayerHealth script was found!");
            }
        }
    }
}
