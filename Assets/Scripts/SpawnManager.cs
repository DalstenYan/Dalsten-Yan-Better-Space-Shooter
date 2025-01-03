using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{

    [Header("Prefabs")]
    [SerializeField]
    private GameObject _enemyPrefab;
    [SerializeField]
    private List<GameObject> _powerupPrefabs;

    [Header("Spawn Delays (inclusive)")]
    [SerializeField]
    private float _enemySpawnDelay;
    [SerializeField]
    private int _powerupMinSpawnDelay;
    [SerializeField]
    private int _powerupMaxSpawnDelay;

    [Header("Instantiation Organization")]
    [SerializeField]
    private Transform _enemyParent;
    [SerializeField]
    private Transform _powerupParent;

    //Used for video but not here
    private bool _stopSpawning;

    public void StartSpawning() 
    {
        StartCoroutine(SpawnEnemy());
        StartCoroutine(SpawnPowerup());
    }

    //Spawn game objects every 5 seconds with Coroutines.
    IEnumerator SpawnEnemy() 
    {
        yield return new WaitForSeconds(3.0f);
        //Create Enemy based on prefab
        var newEnemy = Instantiate(_enemyPrefab, RandomTopPosition(), Quaternion.identity);
        newEnemy.transform.parent = _enemyParent;

        //Wait for static amount of time before repeating
        yield return new WaitForSeconds(_enemySpawnDelay);
        StartCoroutine(SpawnEnemy());
    }

    IEnumerator SpawnPowerup() 
    {
        yield return new WaitForSeconds(8.0f);

        //Find random powerup and create powerup based on prefabs
        int randPowerupIndex = Random.Range(0, _powerupPrefabs.Count);
        var powerup = Instantiate(_powerupPrefabs[randPowerupIndex], RandomTopPosition(), Quaternion.identity);
        powerup.transform.parent = _powerupParent;

        //Wait for dynamic amount of time in a range before repeating
        float newPowerupSpawnDelay = Random.Range(_powerupMinSpawnDelay, _powerupMaxSpawnDelay + 1);
        yield return new WaitForSeconds(newPowerupSpawnDelay);
        StartCoroutine(SpawnPowerup());
    }

    public void OnPlayerDeath() 
    {
        StopAllCoroutines();
        for (int i = 0; i < _enemyParent.childCount; i++) 
        {
            var currentEnemy = _enemyParent.GetChild(i);
            currentEnemy.GetComponent<Enemy>().Freeze();
        }
    }

    public static Vector3 RandomTopPosition()
    {
        float randX = Random.Range(-9.48f, 9.51f);
        return new Vector3(randX, 7.39f, 0);
    }
}
