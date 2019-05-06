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
    public GameObject rifleBullet;
    public GameObject pistolBullet;
    public GameObject shotgunBullet;
    public float rifleDamage = 0f;
    public float pistolDamage = 0f;
    public float shotgunDamage = 0f;
    public float rifleBulletSpeed = 100f;
    public float pistolBulletSpeed = 70f;
    public float shotgunBulletSpeed = 80f;
    public float startRifleAmmunition = 0f;
    public float startPistolAmmunition = 0f;
    public float startShotgunAmmunition = 0f;

    public float rifleMagazineSize = 0f;
    public float pistolMagazineSize = 0f;
    public float shotgunMagazineSize = 0f;
    public float reloadTime = 1f;
    public float pistolCD = 1f;
    public float shotgunCD = 1f;
    public float rifleCD = 0.1f;
    
    
    public GameObject zoneObj;
    private MapBehavior map;
    
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
            GameObject temp = GameObject.Instantiate(playerPrefab) as GameObject;
            PlayerBehaviour tempBehaviour = temp.GetComponent<PlayerBehaviour>();
            tempBehaviour.SetID(i);
            if (i < teamSize)
            {
                tempBehaviour.SetTeam(-1);
            }
            else
            {
                tempBehaviour.SetTeam(1);
            }

        }
    }

    // Start is called before the first frame update
    void Start()
    {
        SpawnTeams();
        map = GameObject.Find("MapController").GetComponent<MapBehavior>();
        StartCoroutine("ZoneHandler");
    }

    // Update is called once per frame
    void Update()
    {

    }

    private Vector3 GetRandomZonePosition(){
        int mapSize = map.GetMapSize();
        float worldSize = mapSize*2.5f;
        float x_cord = Random.Range(5f,worldSize-5f);
        float y_cord = Random.Range(5f,worldSize-5f);
        return new Vector3(x_cord, y_cord, -2);
    }

    IEnumerator ZoneHandler(){
        yield return new WaitForSeconds(10f);
        while(true){
            GameObject zone = Instantiate(zoneObj, GetRandomZonePosition(), Quaternion.identity);
            yield return new WaitForSeconds(30f);
            Destroy(zone);
        }
    }
}
