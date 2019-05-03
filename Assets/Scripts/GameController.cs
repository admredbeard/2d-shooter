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

    public bool FreeLineOfSight(int unitID, int enemyID) {
        Ray2D(players[unitID].transform.position, players[enemyID].transform.position);
        
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
