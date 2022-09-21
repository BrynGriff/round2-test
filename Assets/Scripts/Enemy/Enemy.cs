using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float moveSpeed;
    public Rigidbody rb;

    public EnemyStats stats;
    public EnemyClassGroup.EnemyClass enemyClass;

    private Vector3 moveDirection;

    void OnEnable()
    {
        // Set direction it will move in until it falls off
        moveDirection = Quaternion.Euler(new Vector3(0, Random.Range(0, 360), 0)) * Vector3.forward;
        rb.velocity = Vector3.zero;
    }

    void Update()
    {
        // Move by velocity
        Vector3 velocity = moveDirection * stats.speed * 5f;
        rb.velocity = new Vector3(velocity.x, Mathf.Min(rb.velocity.y, 0), velocity.z);

        // When fallen off deactivate to return to pool
        if (transform.position.y < -50)
        {
            gameObject.SetActive(false);
        }
    }
}
