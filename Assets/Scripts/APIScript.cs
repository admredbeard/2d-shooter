using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class APIScript : MonoBehaviour
{
    public int teamId;
    MapBehavior mb;
    GameController gc;
    float visionRange = 20f;
    void Start()
    {
        mb = GameObject.Find("MapController").GetComponent<MapBehavior>();
        gc = GameObject.Find("GameController").GetComponent<GameController>();
    }

    private bool CheckIfCorrectTeam(int unitId)
    {
        if (gc.GetPlayerBehaviours()[unitId].GetTeam() == teamId)
            return true;
        else
            return false;
    }

    private bool CheckIfInVision(int unitId)
    {
        GameObject enemy = gc.GetPlayers()[unitId];
        List<int> teamUnits = GetPlayers(teamId);
        foreach (int unit in teamUnits)
        {
            GameObject player = gc.GetPlayers()[unit];
            if (Vector3.Distance(player.transform.position, enemy.transform.position) <= visionRange)
                return true;
        }
        return false;
    }

    public void Move(int unitId, float angle)
    {
        if (CheckIfCorrectTeam(unitId))
        {
            //do move
            float movementSpeed = 20f;
            Rigidbody2D rb = gc.GetPlayerBehaviours()[unitId].GetComponent<Rigidbody2D>();
            Vector2 velocity = (Vector2)(Quaternion.AngleAxis(angle, Vector3.forward) * Vector3.right);
            rb.AddForce(velocity * movementSpeed);
        }
        else
        {
            throw new System.UnauthorizedAccessException("Error: Can not move opponents units");
        }
        //Unit must be on our team
    }

    public void LookAtDirection(int unitId, float angle)
    {
        if (CheckIfCorrectTeam(unitId))
        {
            //do rotate
            float offset = 90f; //change this to make the gun point towards where we shoot and stuff
            Rigidbody2D rb = gc.GetPlayerBehaviours()[unitId].GetComponent<Rigidbody2D>();
            rb.transform.rotation = Quaternion.Euler(0f, 0f, angle - offset);
        }
        else
        {
            throw new System.UnauthorizedAccessException("Error: Can not move opponents units");
        }
        //Unit must be on your team
    }

    public float AngleBetweenUnits(int unitId, int targetId)
    {
        return mb.AngleBetweenUnits(unitId, targetId);
    }

    public float AngleBetweenUnitWorldpos(int unitId, Vector2 targetPos)
    {
        return mb.AngleBetweenUnitWorldpos(unitId, targetPos);
    }

    public float AngleBetweenUnitGridpos(int unitId, Vector2Int targetPos)
    {
        return mb.AngleBetweenUnitGridpos(unitId, targetPos);
    }

    public List<int> SenseNearby(int unitId)
    {
        //Unit must be on your team
        List<int> nearbyUnits = gc.UnitsInVision(unitId);
        return nearbyUnits;
    }

    public List<int> SenseNearbyByTeam(int unitId, int team)
    {
        List<int> nearbyUnits = gc.UnitsInVisionByTeam(unitId, team);
        return nearbyUnits;
    }

    public List<int> GetPlayers(int myTeamId)
    {
        if (myTeamId == teamId)
        {
            List<int> ids = new List<int>();

            List<PlayerBehaviour> players = gc.GetPlayerBehaviours();
            foreach (PlayerBehaviour player in players)
            {
                if (player.GetTeam() == teamId)
                    ids.Add(player.GetID());
            }

            return ids;
        }
        else
        {
            throw new System.UnauthorizedAccessException("Error: Can not access other teams players");
        }
    }

    public List<int> GetTeammates(int unitId)
    {
        if (CheckIfCorrectTeam(unitId))
        {
            List<int> teammates = new List<int>();
            int myTeam = gc.GetPlayerBehaviours()[unitId].GetTeam();
            List<PlayerBehaviour> players = gc.GetPlayerBehaviours();

            foreach (PlayerBehaviour player in players)
            {
                if (player.GetTeam() == myTeam && player.GetID() != unitId)
                    teammates.Add(player.GetID());
            }

            return teammates;
        }
        else
        {
            throw new System.UnauthorizedAccessException("Error: Can not access other teams players");
        }

    }

    public Vector2 GetWorldPosition(int unitId)
    {
        if (CheckIfCorrectTeam(unitId) || CheckIfInVision(unitId))
            return mb.GetWorldPos(unitId);
        else
            throw new System.UnauthorizedAccessException("Error: Unit not in vision");
    }

    public Vector2Int GetGridPos(int unitId)
    {
        if (CheckIfCorrectTeam(unitId) || CheckIfInVision(unitId))
            return mb.GetGridPos(unitId);
        else
            throw new System.UnauthorizedAccessException("Error: Unit not in vision");
    }

    public bool TargetInSight(int unitId, int targetId)
    {
        if (CheckIfCorrectTeam(targetId) || CheckIfInVision(targetId))
            return mb.TargetInSight(unitId, targetId);
        else
            throw new System.UnauthorizedAccessException("Error: Unit not in vision");

    }

    public bool WorldPositionInSight(int unitId, Vector2 worldPosition)
    {
        if (CheckIfCorrectTeam(unitId) || CheckIfInVision(unitId))
            return mb.WorldPositionInSight(unitId, worldPosition);
        else
            throw new System.UnauthorizedAccessException("Error: Unit must on your team");
    }

    public bool GridPositionInSight(int unitId, Vector2Int gridPosition)
    {
        //Unit must be on your team
        if (CheckIfCorrectTeam(unitId) || CheckIfInVision(unitId))
            return mb.GridPositionInSight(unitId, gridPosition);
        else
            throw new System.UnauthorizedAccessException("Error: Unit must on your team");
        
    }

    public void Attack(int unitId)
    {
        if (CheckIfCorrectTeam(unitId))
        {
            PlayerBehaviour player = gc.GetPlayerBehaviours()[unitId];
            StartCoroutine(player.FireWeapon());
        }
        else
            throw new System.UnauthorizedAccessException("Error: Can not attack using enemy unit");
    }

    public void SwapWeapon(int unitId, Weapon newWeapon)
    {
        if (CheckIfCorrectTeam(unitId))
        {
            PlayerBehaviour player = gc.GetPlayerBehaviours()[unitId];
            StartCoroutine(player.ChangeWeapon(newWeapon));
        }
        else
            throw new System.UnauthorizedAccessException("Error: Can not swap enemy weapon ");
    }

    public void ReloadWeapon(int unitId)
    {
        if (CheckIfCorrectTeam(unitId))
        {
            PlayerBehaviour player = gc.GetPlayerBehaviours()[unitId];
            StartCoroutine(player.ReloadWeapon());
        }
        else
            throw new System.UnauthorizedAccessException("Error: Can not access reload enemy weapon ");
    }
    public int GetReserveAmmunition(int unitId, Weapon weapon)
    {
        if (CheckIfCorrectTeam(unitId))
        {
            PlayerBehaviour player = gc.GetPlayerBehaviours()[unitId];
            return player.GetReserveAmmunition(weapon);
        }

        throw new System.UnauthorizedAccessException("Error: Can not access enemy reserve ammunition");
    }

    public int GetMagazineAmmunition(int unitId, Weapon weapon)
    {
        if (CheckIfCorrectTeam(unitId))
        {
            PlayerBehaviour player = gc.GetPlayerBehaviours()[unitId];
            return player.GetMagazineAmmunition(weapon);
        }
        throw new System.UnauthorizedAccessException("Error: Can not access enemy magazine ammunition");
    }

    public Weapon GetWeapon(int unitId)
    {
        if (CheckIfCorrectTeam(unitId))
        {
            PlayerBehaviour player = gc.GetPlayerBehaviours()[unitId];
            return player.GetWeapon();
        }
        throw new System.UnauthorizedAccessException("Error: Can not access enemy Weapon");
    }

    public float GetHealth(int unitId)
    {
        if(CheckIfInVision(unitId)){
            List<PlayerBehaviour> list = gc.GetPlayerBehaviours();
            PlayerBehaviour player = gc.GetPlayerBehaviours()[unitId];
            return player.GetHealth();
        }
        else
            throw new System.UnauthorizedAccessException("Error: Can not access enemy health, not in vision");
    }

    public bool CanFire(int unitId)
    {
        if (CheckIfCorrectTeam(unitId))
        {
            if (gc.GetPlayerBehaviours()[unitId].CanFire())
                return true;
            else
                return false;
        }
        else
            throw new System.UnauthorizedAccessException("Error: Can not access enemy fire eligibility");
    }

    public float FireCooldown(int unitId)
    {
        if (CheckIfCorrectTeam(unitId))
            return gc.GetPlayerBehaviours()[unitId].GetFireCd();
        else
            throw new System.UnauthorizedAccessException("Error: Can not access enemy fire cooldown");
    }

    public float ReloadCooldown(int unitId)
    {
        if (CheckIfCorrectTeam(unitId))
            return gc.GetPlayerBehaviours()[unitId].GetReloadCD();
        else
            throw new System.UnauthorizedAccessException("Error: Can not reload enemy weapon");
    }

    public float WeaponSwapCooldown(int unitId)
    {
        if (CheckIfCorrectTeam(unitId))
            return gc.GetPlayerBehaviours()[unitId].GetWeaponSwapCD();
        else
            return 1f;
    }

    public bool IsGridPositionTraversable(Vector2Int gridPosition)
    {
        return mb.IsGridPosTraversable(gridPosition);
    }

    public bool IsWorldPositionTraversable(Vector2 worldPosition)
    {
        return mb.IsWorldPosTraversable(worldPosition);
    }

    public bool[,] GetMap()
    {
        return mb.GetTraversable();
    }

    public float GetDistanceToUnit(int unitId, int targetId)
    {
        if (CheckIfCorrectTeam(unitId) || CheckIfInVision(unitId))
            return mb.DistanceToUnit(unitId, targetId);
        else
            throw new System.UnauthorizedAccessException("Error: Unit must be in Vision");
    }

    public float GetDistanceToGridPosition(int unitId, Vector2Int gridPosition)
    {
        if (CheckIfCorrectTeam(unitId))
            return mb.DistanceToGridPos(unitId, gridPosition);
        else
            throw new System.UnauthorizedAccessException("Error: Unit must on your team");
    }

    public float GetDistanceToWorldPosition(int unitId, Vector2 worldPosition)
    {
        if (CheckIfCorrectTeam(unitId))
            return mb.DistanceToWorldPos(unitId, worldPosition);
        else
            throw new System.UnauthorizedAccessException("Error: Unit must on your team");
    }

    public Vector2 GetZonePosition()
    {
        return mb.ZoneCenter();
    }

    public float GetZoneRadius()
    {
        return mb.ZoneRadius();
    }

    public bool IsUnitInZone(int unitId)
    {
        if (CheckIfCorrectTeam(unitId) || CheckIfInVision(unitId))
            return mb.IsUnitInZone(unitId);
        else
            throw new System.UnauthorizedAccessException("Error: Unit must be in Vision");
    }

    public bool IsWorldPosInZone(Vector2 worldPos)
    {
        return mb.IsWorldPosInZone(worldPos);
    }

    public bool IsGridPosInZone(Vector2Int gridPos)
    {
        return mb.IsGridPosInZone(gridPos);
    }

    public int GetMapSize()
    {
        return mb.GetMapSize();
    }
    
    public Vector2Int GetGridPosFromWorldPos(Vector2 worldPos)
    {
        // NOT IMPLEMENTED YET
        return mb.GetGridPosFromWorldPos(worldPos);
    }

    public Vector2 GetWorldPosFromGridPos(Vector2Int gridIndex)
    {
        // NOT IMPLEMENTED YET
        return mb.GetGridPosFromWorldPos(gridIndex);
    }

    public Vector2 GetWorldPosFromGridPos(int x, int y)
    {
        // NOT IMPLEMENTED YET
        return mb.GetWorldPosFromGridPos(x, y);
    }

    public int GetOurTeamScore()
    {
        return gc.GetTeamScore(teamId);
    }

    public int GetEnemyTeamScore()
    {
        return gc.GetTeamScore(teamId*-1);
    }

}
