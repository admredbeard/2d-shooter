using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public int teamCount;
    public int teamSize;
    private int team1Score;
    private int team2Score;
    List<PlayerBehaviour> players;



    public float GetHp(int unitID)
    {
        return players[unitID].GetHealth();
    }

    // Get a list of PlayerBehaviours of all players in the vision range.
    public List<PlayerBehaviour> VisiblePlayers(int unitID)
    {
        List<PlayerBehaviour> inRange = new List<PlayerBehaviour>();
        float visionRange = players[unitID].visionRange;
        foreach (PlayerBehaviour player in players)
        {
            if (player.GetID() != unitID)
            {
                if (Vector2.Distance(player.transform.position, players[unitID].transform.position) < visionRange){
                    inRange.Add(player);
                }
            }
        }
        return inRange;
    }

    // Returns true if the first object hit is the intended enemy, else returns false
    public bool FreeLineOfSight(int unitID, int enemyID) {
        RaycastHit2D[] lineOfSightObjects = Physics2D.RaycastAll(players[unitID].transform.position, players[enemyID].transform.position - players[unitID].transform.position);

        for (int i = 0; i < lineOfSightObjects.Length; i++) {
            if (lineOfSightObjects[i].transform.name == players[unitID].transform.name) {
                continue;
            }
            // If distance between an object hit by ray and the player is less than between player and intended target, the unit is not in a free line of sight.
            else if(Vector2.Distance(lineOfSightObjects[i].transform.position, players[unitID].transform.position) < Vector2.Distance(players[unitID].transform.position, players[enemyID].transform.position) {
                return false;   
            }
        }
        return true;
    }
    
    

    // Force unit to look in direction of the specified angle, counterclockwise clamps between 0 and 360. 
    public void LookInDir(int unitID, float angle) {
        angle = Mathf.Clamp(angle, 0, 360);
        players[unitID].transform.Rotate(new Vector3(0, 0, 1), angle);
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
