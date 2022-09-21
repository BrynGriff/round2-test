using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PooledObject : MonoBehaviour
{
    public ObjectPool objectPool;
    public bool active = false;

    public void OnDisable()
    {
        // If there is no pool reference this is a temporary object, destroy it
        if (objectPool == null)
        {
            Destroy(gameObject);
        }
        else
        {
            // Return object to the pool
            objectPool.ReturnObject(gameObject);
        }
    }
}
