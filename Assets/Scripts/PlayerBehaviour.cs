using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    void Start()
    {
        health = maxHealth;
        // gc = GetCompotent<GameController>(); do something like this.
    }

    public float visionRange = 10f;
    private float health = 0f;
    public float maxHealth = 100f;
    private int id = -1;
    GameController gc;


    // Gets the hp of a unit.
    public float GetHealth() {
        return health;
    }

    // Gets the id of a unit.
    public int GetID() {
        return id;
    }

    // Sets the id of a unit.
    public void SetID(int id) {
        this.id = id;
    }

    // Applies damage.
    public void TakeDamage(float damage)
    {
        health -= damage;

        if(health < 0f){

            Destroy(gameObject, 2f);
            Debug.Log("You Died");
            //Should probably do something else, Increment team score etc. 

        }
    }
}
