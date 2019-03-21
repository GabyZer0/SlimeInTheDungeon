using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    protected float pv;

    protected float attack;
    protected float attack_distance;
    protected float attack_cooldown;
    protected float attack_clock;

    // Start is called before the first frame update
    void Start()
    {
        pv = 1; //default value

        attack_clock = 0;
    }

    protected virtual void ReceiveDamage(float damage)
    {
        if((pv-=damage)<=0)
        {
            Death();
        }
    }

    protected virtual void Death()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Ennemy " + name + " triggered by " + other.name);

    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Enemy " + name + " collided by " + collision.collider.name);
    }


}
