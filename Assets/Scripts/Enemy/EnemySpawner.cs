using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class EnemySpawner : MonoBehaviour
{
    // Whitelist of classes and types
    public List<EnemyClassGroup> spawnableClasses;
    public List<EnemyType> spawnableTypes;

    // Enemy types are stored in a dictionary to ensure no double ups
    private Dictionary<int, EnemyType> typesToSpawn = new Dictionary<int, EnemyType>();

    // All spawn timers this spawner loops through
    private List<SpawnTimer> spawnTimers = new List<SpawnTimer>();

    // An infinitely looping timer to spawn enemies based on spawn rate
    private class SpawnTimer
    {
        public float timer;
        public EnemyType type;
        public ObjectPool objectPool;

        public void Reset()
        {
            float spawnRate = type.GetModifiedSpawnRate();

            // Prevent dividing by 0, will decrement infinitely
            if (spawnRate == 0)
            {
                timer = Mathf.Infinity;
            }
            else
            {
                timer = 1f / spawnRate;
            }
        }
    }

    public void Start()
    {
        // Reset timers if time category cahnges
        DayNightCycle.onTimeCategoryChanged.AddListener(ResetAllTimers);

        Initialize();
    }

    public void Initialize()
    {
        ClearObjectPools();

        // Create spawn list dictionary
        GetSpawnables();

        // Create timers for each type this spawner spawns
        spawnTimers.Clear();
        foreach (EnemyType enemyType in typesToSpawn.Values)
        {
            SpawnTimer newSpawnTimer = CreateSpawnTimer(enemyType);
            spawnTimers.Add(newSpawnTimer);
        }
    }

    public void OnValidate()
    {
        if (Application.isPlaying)
        {
            StartCoroutine(InitializeAfterDelay());
        }
    }

    public void Update()
    {
        DecrementSpawnTimers();
    }

    // Reduces the time left on each spawn timer
    private void DecrementSpawnTimers()
    {
        foreach(SpawnTimer timer in spawnTimers)
        {
            DecrementSpawnTimer(timer);
        }
    }

    private void DecrementSpawnTimer(SpawnTimer spawnTimer)
    {
        // Spawn the enemy and restart the timer if it has run out
        spawnTimer.timer -= Time.deltaTime;
        if (spawnTimer.timer < 0)
        {
            SpawnEnemy(spawnTimer.type, spawnTimer.objectPool);
            spawnTimer.Reset();
        }
    }

    // Populates the spawnable dictionary with defined enemy types and types in classes
    public void GetSpawnables()
    {
        typesToSpawn.Clear();

        // Get all enemies from all spawnable classes
        foreach(EnemyClassGroup classGroup in spawnableClasses)
        {
            if (classGroup != null)
            {
                foreach (EnemyType enemyType in classGroup.groupTypes)
                {
                    if (enemyType != null)
                    {
                        typesToSpawn.TryAdd(enemyType.GetInstanceID(), enemyType);
                    }
                }
            }
        }

        // Get all enemies from spawnable types
        foreach (EnemyType enemyType in spawnableTypes)
        {
            if (enemyType != null)
            {
                typesToSpawn.TryAdd(enemyType.GetInstanceID(), enemyType);
            }
        }
    }

    private void SpawnEnemy(EnemyType type, ObjectPool pool)
    {
        // Spawn enemy from pool to prevent lag
        GameObject spawnedEnemy = pool.SpawnFromPool(transform.position, Quaternion.identity);

        // Pass modified stats to enemy
        var enemyComponent = spawnedEnemy.GetComponent<Enemy>();
        enemyComponent.stats = type.CreateModifiedStats();
        enemyComponent.enemyClass = type.enemyClass;
    }

    // Creates a new spawn timer that will spawn an enemy on a loop
    private SpawnTimer CreateSpawnTimer(EnemyType type)
    {
        // Create timer
        SpawnTimer newSpawnTimer = new SpawnTimer
        {
            type = type
        };

        GameObject poolObj = new GameObject("Object Pool");
        poolObj.transform.SetParent(transform);

        // Add a new object pool for this enemy
        ObjectPool objectPool = poolObj.AddComponent<ObjectPool>();
        objectPool.prefabToPool = type.enemyPrefab;

        // Set pool sized based on spawn rate
        objectPool.poolSize = Mathf.CeilToInt(type.spawnRate * 20);
        newSpawnTimer.objectPool = objectPool;

        // Start timer
        newSpawnTimer.Reset();

        return newSpawnTimer;
    }

    private void ClearObjectPools()
    {
        foreach (SpawnTimer timer in spawnTimers)
        {
            Destroy(timer.objectPool.gameObject);
        }
    }

    // Reset each spawn timer when the time changes to prevent infinite waiting
    private void ResetAllTimers()
    {
        foreach(SpawnTimer timer in spawnTimers)
        {
            timer.Reset();
        }
    }

    public IEnumerator InitializeAfterDelay()
    {
        yield return null;
        Initialize();
    }
}
