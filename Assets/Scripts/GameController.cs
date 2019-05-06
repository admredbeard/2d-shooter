using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public int teamCount;
    public int teamSize;
    public GameObject playerPrefab;
    private int team1Score;
    private int team2Score;
    List<GameObject> players;
    MapBehavior mb;



    public float GetHp(int unitID)
    {

        return players[unitID].GetComponent<PlayerBehaviour>().GetHealth();
    }

    // Get a list of PlayerBehaviours of all players in the vision range.
    public List<GameObject> VisiblePlayers(int unitID)
    {
        List<GameObject> inRange = new List<GameObject>();
        float visionRange = players[unitID].GetComponent<PlayerBehaviour>().visionRange;
        foreach (GameObject player in players)
        {
            if (player.GetComponent<PlayerBehaviour>().GetID() != unitID)
            {
                if (Vector2.Distance(player.transform.position, players[unitID].transform.position) < visionRange)
                {
                    inRange.Add(player);
                }
            }
        }
        return inRange;
    }

    // Returns true if the first object hit is the intended enemy, else returns false
    public bool FreeLineOfSight(int unitID, int enemyID)
    {
        RaycastHit2D[] lineOfSightObjects = Physics2D.RaycastAll(players[unitID].transform.position, players[enemyID].transform.position - players[unitID].transform.position);

        for (int i = 0; i < lineOfSightObjects.Length; i++)
        {
            if (lineOfSightObjects[i].transform.name == players[unitID].transform.name)
            {
                continue;
            }
            // If distance between an object hit by ray and the player is less than between player and intended target, the unit is not in a free line of sight.
            else if (Vector2.Distance(lineOfSightObjects[i].transform.position, players[unitID].transform.position) < Vector2.Distance(players[unitID].transform.position, players[enemyID].transform.position))
            {
                return false;
            }
        }
        return true;
    }



    // Force unit to look in direction of the specified angle, counterclockwise clamps between 0 and 360. 
    public void LookInDir(int unitID, float angle)
    {
        angle = Mathf.Clamp(angle, 0, 360);
        players[unitID].transform.Rotate(new Vector3(0, 0, 1), angle);
    }

    // Spawning teamSize units for each team and assigning unique UnitIDs aswell as Team with values -1 or 1.
    private void SpawnTeams()
    {
        Debug.Log(players.Count);
        Debug.Log(players.Count);
        for (int i = 0; i < 2 * teamSize; i++)
        {
            GameObject temp = Instantiate(playerPrefab) as GameObject;
            temp.name = "CoolDude";
            PlayerBehaviour tempBehaviour = temp.GetComponent<PlayerBehaviour>();
            tempBehaviour.SetID(i);

            players.Add(temp);
            if (i < teamSize)
            {
                tempBehaviour.SetTeam(-1);
            }
            else
            {
                tempBehaviour.SetTeam(1);
            }
            //Respawn(i);
        }

    }

    private void Respawn(int unitID) {
        GameObject respawnUnit = players[unitID];

        int tempXIdx = 0;
        int tempYIdx = 0;
        for (int i = 0; i < players.Count; i++) { 
            if (respawnUnit.GetComponent<PlayerBehaviour>().team != players[i].GetComponent<PlayerBehaviour>().team) {
                tempXIdx += (int)mb.GetGridPosFromWorldPos(players[i].transform.position).x;
                tempYIdx += (int)mb.GetGridPosFromWorldPos(players[i].transform.position).y;
            }
        }
        tempXIdx = tempXIdx / teamSize;
        tempYIdx = tempYIdx / teamSize;

        Vector2 meanPos = mb.GetWorldPosFromGridPos(tempXIdx, tempYIdx);
        bool [,] traverability = mb.GetTraversable();
        Vector2[] corners = { mb.GetWorldPosFromGridPos(0, 0), mb.GetWorldPosFromGridPos(traverability.GetLength(0) - 1, 0),
                            mb.GetWorldPosFromGridPos(traverability.GetLength(0) - 1, 0), mb.GetWorldPosFromGridPos(traverability.GetLength(0) - 1, traverability.GetLength(0) - 1) };
        float tempDist = 0;
        float maxdist = 0;
        Vector2 spawnPos = meanPos;
        for (int i = 0; i < corners.Length; i++) {
            tempDist = Vector2.Distance(meanPos, corners[i]);
            if (tempDist > maxdist) {
                maxdist = tempDist;
                spawnPos = corners[i];
            }
        }

        respawnUnit.transform.position = spawnPos;
        respawnUnit.GetComponent<PlayerBehaviour>().ResetStats();
    }

    // Start is called before the first frame update
    void Start()
    {
        players = new List<GameObject>();
        mb = GameObject.Find("MapController").GetComponent<MapBehavior>();
        SpawnTeams();

    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < players.Count; i++) {
            if (GetHp(i) < 0) Respawn(i);
        }
    }
}
