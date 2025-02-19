using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public Transform spawnPoint;       // Single spawn point for simplicity
    public Transform targetLocation;
    public List<Wave> waves;           // List of waves
    public float timeBetweenWaves = 5f; // Time between waves

    private int currentWaveIndex = 0;
    private bool isSpawning = false;

    void Start()
    {
        // Start the wave spawning process once towers are placed
        if (AreTowersPlaced() && waves.Count > 0)
        {
            StartCoroutine(SpawnWaves());
        }
        else
        {
            GameManager.Instance.waveText.text = "Place A Tower";
        }
    }

    private void Update()
    {
        // Ensure we start the spawning process only once
        if (!isSpawning && AreTowersPlaced() && waves.Count > 0)
        {
            isSpawning = true;
            StartCoroutine(SpawnWaves());
        }
    }

    IEnumerator SpawnWaves()
    {
        while (currentWaveIndex < waves.Count && isSpawning)
        {
            Wave currentWave = waves[currentWaveIndex];
            GameManager.Instance.UpdateWaveText(currentWaveIndex);
            Debug.Log($"Starting Wave {currentWaveIndex + 1}");

            for (int i = 0; i < currentWave.enemyCount; i++)
            {
                if (!isSpawning) yield break; // Stop spawning if the game is over

                SpawnEnemy(spawnPoint, currentWave.movementType);
                yield return new WaitForSeconds(currentWave.spawnInterval);
            }

            Debug.Log($"Wave {currentWaveIndex + 1} Completed");
            currentWaveIndex++;

            if (currentWaveIndex < waves.Count)
            {
                yield return new WaitForSeconds(timeBetweenWaves); // Wait before starting the next wave
            }
        }

        Debug.Log("All Waves Completed");
    }

    void SpawnEnemy(Transform spawnPoint, MovementType moveType)
    {
        // Calculate random position within the spawn point's square bounds using Mathf.Lerp
        float randomX = Mathf.Lerp(-spawnPoint.localScale.x / 2f, spawnPoint.localScale.x / 2f, UnityEngine.Random.value);
        float randomY = Mathf.Lerp(-spawnPoint.localScale.y / 2f, spawnPoint.localScale.y / 2f, UnityEngine.Random.value);
        Vector3 randomPosition = spawnPoint.position + new Vector3(randomX, randomY, 0);

        // Instantiate enemy at the random position
        GameObject enemy = Instantiate(enemyPrefab, randomPosition, Quaternion.identity);
        EnemyBehavior movement = enemy.GetComponent<EnemyBehavior>();

        if (movement != null)
        {
            movement.Initialize(targetLocation, moveType);
        }
    }




    public void StopSpawning()
    {
        isSpawning = false;
        StopAllCoroutines();
    }

    private bool AreTowersPlaced()
    {
        return FindObjectsOfType<TowerBehavior>().Length > 0 || FindObjectsOfType<RadiusTower>().Length > 0;
    }
}
