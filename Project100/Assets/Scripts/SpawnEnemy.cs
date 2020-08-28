using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEnemy : MonoBehaviour
{
    public GameObject enemyPrefab;
    public int radius = 5;
    public Vector2 randomSpawn;
    void Start()
    {
        randomSpawn = Random.insideUnitCircle * radius;
        StartCoroutine(SpawnNewEnemy());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Spawn()
    {
        int spawnPointX = Random.Range(50,100);
        int spawnPointY = Random.Range(-100, -25);
        Vector3 spawnPos = new Vector3(spawnPointX, 2, spawnPointY);

        Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
    }

    IEnumerator SpawnNewEnemy()
    {
        for(int i = 0; i < 100; i++)
        {
            Spawn();
        }
        yield return new WaitForSeconds(1f);
    }
}
