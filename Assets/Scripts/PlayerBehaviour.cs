using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    void Start()
    {
        health = maxHealth;
    }

    public float visionRange = 10f;
    private float health = 0f;
    public float maxHealth = 100f;
    public void TakeDamage(float damage)
    {
        health -= damage;

        if(health < 0f){

            Destroy(gameObject, 2f);
            Debug.Log("You Died");
        }
    }
}
