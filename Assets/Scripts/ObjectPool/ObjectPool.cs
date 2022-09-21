using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public List<GameObject> pooledObjects;
    public GameObject prefabToPool;
    public int poolSize;

    void Start()
    {
        pooledObjects = new List<GameObject>();

        // Make sure there is at least 1 object in the pool
        poolSize = Mathf.Max(poolSize, 1);

        // Populate pool
        GameObject tmp;
        for (int i = 0; i < poolSize; i++)
        {
            tmp = Instantiate(prefabToPool);
            tmp.SetActive(false);

            // Add pooled object component to pooled object so it can return on disable
            PooledObject pooledObject = tmp.AddComponent<PooledObject>();
            pooledObject.objectPool = this;

            // Set parent to pool for tidyness
            tmp.transform.SetParent(transform, true);
            pooledObjects.Add(tmp);
        }
    }

    // Spawns an object from the pool at given position and rotation
    public GameObject SpawnFromPool(Vector3 position, Quaternion rotation)
    {
        GameObject spawnedObject = null;

        // Find an inactive object in the pool
        bool maxSize = true;
        for (int i = 0; i < poolSize; i++)
        {
            if (!pooledObjects[i].activeInHierarchy)
            {
                spawnedObject = pooledObjects[i];
                maxSize = false;
            }
        }

        // If there were no objects the pool is completely used up, create a temporary object that will delete itself
        if (maxSize)
        {
            spawnedObject = Instantiate(prefabToPool);
            spawnedObject.AddComponent<PooledObject>();
        }

        // Activate object and move into position
        spawnedObject.SetActive(true);
        spawnedObject.transform.SetParent(null, true);
        spawnedObject.transform.position = position;
        spawnedObject.transform.rotation = rotation;

        return spawnedObject;
    }

    // This is done here because it can't be done in object disable
    public void ReturnObject(GameObject instance)
    {
        StartCoroutine(ReturnObjectCoroutine(instance));
    }

    // Return object after a frame when object is not in the process of being disabled
    private IEnumerator ReturnObjectCoroutine(GameObject instance)
    {
        yield return null;
        instance.transform.SetParent(transform, true);
    }
}
