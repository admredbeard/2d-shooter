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
    List<PlayerBehaviour> playerBehaviours;
    public GameObject zoneObj;
    MapBehavior mb;
    bool[,] traverability;
    public void GiveScoreToTeamOne(int score)
    {
        team1Score = team1Score + score;
    }

    public void GiveScoreToTeamTwo(int score)
    {
        team2Score = team2Score + score;
    }

    public int GetTeamOneScore()
    {
        return team1Score;
    }

    public int GetTeamTwoScore()
    {
        return team2Score;
    }

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

    public List<GameObject> GetPlayers()
    {
        return players;
    }

    public List<PlayerBehaviour> GetPlayerBehaviours()
    {
        return playerBehaviours;
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

        for (int i = 0; i < 2 * teamSize; i++)
        {
            GameObject temp = Instantiate(playerPrefab) as GameObject;
            temp.name = "Player" + (i + 1).ToString();
            PlayerBehaviour tempBehaviour = temp.GetComponent<PlayerBehaviour>();
            tempBehaviour.SetID(i);
            players.Add(temp);
            playerBehaviours.Add(tempBehaviour);
            if (i % 2 == 0)
            {
                tempBehaviour.SetTeam(-1);
                temp.transform.position = mb.GetWorldPosFromGridPos(traverability.GetLength(0) - 1, traverability.GetLength(0) - 1);
            }
            else
            {
                tempBehaviour.SetTeam(1);
                temp.transform.position = mb.GetWorldPosFromGridPos(0, 0);
            }
        }

    }

    private Vector2 GetRespawnPos(int unitID)
    {
        GameObject respawnUnit = players[unitID];

        int tempXIdx = 0;
        int tempYIdx = 0;
        int playerCount = 0;
        for (int i = 0; i < players.Count; i++)
        {
            if (respawnUnit.GetComponent<PlayerBehaviour>().GetTeam() != players[i].GetComponent<PlayerBehaviour>().GetTeam())
            {
                tempXIdx += (int)mb.GetGridPosFromWorldPos(players[i].transform.position).x;
                tempYIdx += (int)mb.GetGridPosFromWorldPos(players[i].transform.position).y;
                playerCount++;
            }
        }
        tempXIdx = tempXIdx / playerCount;
        tempYIdx = tempYIdx / playerCount;

        Vector2 meanPos = mb.GetWorldPosFromGridPos(tempXIdx, tempYIdx);

        Vector2[] corners = { mb.GetWorldPosFromGridPos(0, 0), mb.GetWorldPosFromGridPos(traverability.GetLength(0) - 1, 0),
                            mb.GetWorldPosFromGridPos(traverability.GetLength(0) - 1, 0), mb.GetWorldPosFromGridPos(traverability.GetLength(0) - 1, traverability.GetLength(0) - 1) };
        float tempDist = 0;
        float maxdist = 0;
        Vector2 spawnPos = meanPos;
        for (int i = 0; i < corners.Length; i++)
        {
            tempDist = Vector2.Distance(meanPos, corners[i]);
            if (tempDist > maxdist)
            {
                maxdist = tempDist;
                spawnPos = corners[i];
            }
        }

        return spawnPos;
    }

    private void Respawn(int unitID)
    {

        Vector2 spawnPos = GetRespawnPos(unitID);

        players[unitID].transform.position = spawnPos;
        players[unitID].GetComponent<PlayerBehaviour>().ResetStats();
    }

    // Start is called before the first frame update
    void Start()
    {
        players = new List<GameObject>();
        playerBehaviours = new List<PlayerBehaviour>();
        mb = GameObject.Find("MapController").GetComponent<MapBehavior>();
        traverability = mb.GetTraversable();
        SpawnTeams();
        StartCoroutine("ZoneHandler");
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].GetComponent<PlayerBehaviour>().GetHealth() < 0)
            {
                Respawn(i);
            }
        }
    }

    private Vector3 GetRandomZonePosition()
    {
        int mapSize = mb.GetMapSize();
        float worldSize = mapSize * 2.5f;
        float x_cord = Random.Range(5f, worldSize - 5f);
        float y_cord = Random.Range(5f, worldSize - 5f);
        return new Vector3(x_cord, y_cord, -2);
    }

    IEnumerator ZoneHandler()
    {
        yield return new WaitForSeconds(10f);
        while (true)
        {
            GameObject zone = Instantiate(zoneObj, GetRandomZonePosition(), Quaternion.identity);
            yield return new WaitForSeconds(30f);
            Destroy(zone);
        }
    }
}
