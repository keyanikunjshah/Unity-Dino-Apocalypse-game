using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DinosaurWaveSystem : MonoBehaviour
{
    public GameObject[] dinosaurPrefabs;
    public Transform[] spawnPoints;
    public float timeBetweenWaves = 10f;
    [SerializeField] private float waveTimer = 0f;
    private int waveNumber = 1;
    private int dinosaursPerWave = 1;
    private int dinosaursKilled = 0; // Counter for dinosaurs killed
    private int totalDinosaurs = 14; // Total number of dinosaurs including initial ones

    void Update()
    {
        if (waveNumber == 4)
            return;

        waveTimer += Time.deltaTime;

        int intValue = Mathf.RoundToInt(waveTimer);

        if (waveTimer >= timeBetweenWaves)
        {
            StartNewWave();
        }
    }

    void StartNewWave()
    {
        waveTimer = 0f;
        dinosaursPerWave += 1;
        float minDistance = 4f;

        for (int i = 0; i < dinosaursPerWave; i++)
        {
            int randomSpawnIndex = Random.Range(0, spawnPoints.Length);
            Transform spawnPoint = spawnPoints[randomSpawnIndex];

            GameObject randomDinosaurPrefab = dinosaurPrefabs[Random.Range(0, dinosaurPrefabs.Length)];

            Vector3 spawnPosition = spawnPoint.position + Random.insideUnitSphere * minDistance;
            spawnPosition.y = spawnPoint.position.y;

            Instantiate(randomDinosaurPrefab, spawnPosition, spawnPoint.rotation);
        }

        waveNumber++;
    }

    public void DinosaurKilled(string v)
    {
        dinosaursKilled++;
        if (dinosaursKilled >= totalDinosaurs)
        {
            // Player wins
            Debug.Log("You Win! All dinosaurs are defeated.");
            // Implement any additional win logic here, such as stopping the game or displaying a UI message.
        }
    }
}
