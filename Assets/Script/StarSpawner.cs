using UnityEngine;

public class StarSpawner : MonoBehaviour
{
    public GameObject starPrefab; 
    public int starCount = 800;   
    public float skyHeight = 1000f; 
    public float areaSize = 2000f;  

    void Start()
    {
        for (int i = 0; i < starCount; i++)
        {
            // We use Random.Range INSIDE the loop so every star gets a unique position
            Vector3 randomPos = new Vector3(
                Random.Range(-areaSize, areaSize), 
                Random.Range(skyHeight, skyHeight + 200f), // Adds depth to the sky
                Random.Range(-areaSize, areaSize)
            );
            
            GameObject newStar = Instantiate(starPrefab, randomPos, Quaternion.identity);
            
            // This puts them inside the StarSystem folder in your Hierarchy
            newStar.transform.parent = this.transform;
        }
    }
}