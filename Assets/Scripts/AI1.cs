using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI1 : MonoBehaviour
{
    APIScript api;
    GameController gc;
    List <int> myUnits; // These are the players you can control
    int myTeamId; //This id is used for certain API calls and is unique for your team

    void Start()
    {
        api = gameObject.GetComponent<APIScript> ();
        myTeamId = api.teamId;
        myUnits = api.GetPlayers(myTeamId);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        foreach(int unitId in myUnits){

            //Execute your code
            
        }
    }
}
