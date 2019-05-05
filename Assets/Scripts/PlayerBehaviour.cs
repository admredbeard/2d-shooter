using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{

    void Start()
    {
        health = maxHealth;
        gc = GameObject.Find("GameController").GetComponent<GameController>();
    }

    public float visionRange = 10f;
    private float health = 0f;
    public float maxHealth = 100f;
    public int id = -1; // Should be private, use getters, this is only for debug
    public int team = 0; // Should be private, use getters, this is only for debug
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

    // Gets the team of a unit.
    public int GetTeam()
    {
        return team;
    }

    // Sets the team of a unit.
    public void SetTeam(int team)
    {
        this.team = team;
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
