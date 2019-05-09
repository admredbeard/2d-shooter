using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class APIScript : MonoBehaviour
{
    public int teamId;
    MapBehavior mb;
    GameController gc;
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

    public void Move(int unitId, float angle)
    {
        if(CheckIfCorrectTeam(unitId)){
            //do move
            float speed = 5f;
            float distance = 5f;
            Rigidbody2D rb = gc.GetPlayerBehaviours()[unitId].GetComponent<Rigidbody2D>();
            RaycastHit2D[] hit = new RaycastHit2D[2];
            Vector2 velocity = (Vector2)(Quaternion.AngleAxis(angle, Vector3.forward) * Vector3.right);
            
            int results = rb.Cast(velocity.normalized, hit, speed*Time.fixedDeltaTime*2);
            if(results > 0){
                distance = 0;
            }else if(results == 0){
                distance = speed*Time.fixedDeltaTime;
            }
            rb.MovePosition(rb.position + velocity.normalized*distance);
        }else{
            throw new System.UnauthorizedAccessException("Error: Can not move opponents units");
        }
        //Unit must be on our team
    }

    public void MoveAddforce(int unitId, float angle)
    {
        if(CheckIfCorrectTeam(unitId)){
            //do move
            Rigidbody2D rb = gc.GetPlayerBehaviours()[unitId].GetComponent<Rigidbody2D>();
            Vector3 test = Quaternion.AngleAxis(angle, Vector3.forward) * Vector3.right;
            Vector2 vel = new Vector2(test.x,test.y);
            Debug.Log(angle);
            rb.AddForce(vel*20);
        }else{
            throw new System.UnauthorizedAccessException("Error: Can not move opponents units");
        }
        //Unit must be on our team
    }

    public void LookAtDirection(int unitId, float angle)
    {
        //Unit must be on your team
    }

    public void AngleBetween(int unitId, int targetId)
    {
        //target must be within our vision
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

    public List<GameObject> GetTeammates(int unitId)
    {
        //Unit must be on your team
        List<GameObject> teammates = new List<GameObject>();

        return teammates;
    }

    public Vector2 GetWorldPosition(int unitId)
    {
        //Unit must be in vision range or in our team
        return mb.GetWorldPos(unitId);
    }

    public Vector2Int GetGridPos(int unitId)
    {
        //Unit must be in vision range or in our team
        return mb.GetGridPos(unitId);
    }

    public bool TargetInSight(int unitId, int targetId)
    {
        //Unit must be on your team
        return mb.TargetInSight(unitId, targetId);
    }

    public bool WorldPositionInSight(int unitId, Vector2 worldPosition)
    {
        //Unit must be on your team
        return mb.WorldPositionInSight(unitId, worldPosition);
    }

    public bool GridPositionInSight(int unitId, Vector2Int gridPosition)
    {
        //Unit must be on your team
        return mb.GridPositionInSight(unitId, gridPosition);
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
        List<PlayerBehaviour> list = gc.GetPlayerBehaviours();
        print(list.Count);
        PlayerBehaviour player = gc.GetPlayerBehaviours()[unitId];
        return player.GetHealth();
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
        {
            if (CheckIfCorrectTeam(unitId))
                return gc.GetPlayerBehaviours()[unitId].GetWeaponSwapCD();
            else
                return 1f;
        }
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
        //Unit must be in vision range or in our team
        return mb.DistanceToUnit(unitId, targetId);
    }

    public float GetDistanceToGridPosition(int unitId, Vector2Int gridPosition)
    {
        //Unit must be in vision range or in our team
        return mb.DistanceToGridPos(unitId, gridPosition);
    }

    public float GetDistanceToWorldPosition(int unitId, Vector2 worldPosition)
    {
        //Unit must be in vision range or in our team
        return mb.DistanceToWorldPos(unitId, worldPosition);
    }

    public bool IsUnitInZone(int unitId)
    {
        //Unit must be in vision range or in our team
        return mb.IsUnitInZone(unitId);
    }

    public bool IsWorldPosInZone(Vector2 worldPos)
    {
        //Unit must be in vision range or in our team
        return mb.IsWorldPosInZone(worldPos);
    }

    public bool IsGridPosInZone(Vector2Int gridPos)
    {
        //Unit must be in vision range or in our team
        return mb.IsGridPosInZone(gridPos);
    }

}
