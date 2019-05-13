using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI1 : MonoBehaviour
{
    APIScript api;
    GameController gc;
    int aplayerID;
    // Start is called before the first frame update
    void Start()
    {
        api = GetComponent<APIScript>();
        gc = GameObject.Find("GameController").GetComponent<GameController>();
        GameObject aPlayer = gc.GetPlayers()[0];
        if (aPlayer.GetComponent<PlayerBehaviour>().GetTeam() == -1){
            aPlayer = gc.GetPlayers()[1];
        }
        aplayerID = aPlayer.GetComponent<PlayerBehaviour>().GetID();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //if(Input.GetMouseButtonDown(0)){
        Vector2 diff = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - api.GetWorldPosition(aplayerID);
        //diff.Normalize();
        float angletest = Vector2.SignedAngle(Vector2.right, diff);
        
        //api.Move(aplayerID, angletest);
        api.LookAtDirection(aplayerID, angletest);
        api.MoveAddforce(aplayerID, angletest);
        //}
    }
}
