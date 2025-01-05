using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AsteroidGame : MonoBehaviour
{
    public GameObject player;
    public List<GameObject> asteroids;
    public GameObject asteroidPrefab;

    public int baseHealth = 10;
    public int baseHealthOffset = 1;
    public int maxHealth => (int)(baseHealth / ((JackGameManager.difficulty * 2)-1)) + baseHealthOffset;
    public int health = 0;

    public float timeElapsed = 0.0f;
    public float timeNeeded = 60.0f;

    public int clusterSize => 2 + (int)(JackGameManager.difficulty * 2) * 2;
    public float asteroidSpeed = 1.0f;
    public float asteroidSpawnCooldown = 1.0f;

    void OnReset()
    {
        health = maxHealth;
        player.transform.localPosition = Vector3.zero;
        player.transform.localRotation = Quaternion.identity;
        for (int i = 0; i < asteroids.Count; i++)
        {
            Destroy(asteroids[i]);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
