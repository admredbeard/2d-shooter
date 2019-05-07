using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class APIScript : MonoBehaviour
{
    MapBehavior mb;
    GameController gc;
    void Start()
    {
        mb = GameObject.Find("MapController").GetComponent<MapBehavior>();
        gc = GameObject.Find("GameController").GetComponent<GameController> ();
    }

    public void Move(int unitId, float angle)
    {
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

    public List<GameObject> SenseNearby(int unitId)
    {
        //Unit must be on your team
        List<GameObject> nearbyUnits = new List<GameObject>();

        return nearbyUnits;
    }

    public List<GameObject> SenseNearbyByTeam(int unitId, int team)
    {
        //Unit must be on your team
        List<GameObject> nearbyUnits = new List<GameObject>();

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
        PlayerBehaviour player = gc.GetPlayerBehaviours()[unitId];
        StartCoroutine(player.FireWeapon());
    }

    public void SwapWeapon(int unitId, Weapon newWeapon)
    {
        PlayerBehaviour player = gc.GetPlayerBehaviours()[unitId];
        StartCoroutine(player.ChangeWeapon(newWeapon));
        //Unit must be on your team
    }

    public void ReloadWeapon(int unitId)
    {
        PlayerBehaviour player = gc.GetPlayerBehaviours()[unitId];
        StartCoroutine(player.ReloadWeapon());
    }

    public int GetReserveAmmunition(int unitId, Weapon weapon)
    {
        PlayerBehaviour player = gc.GetPlayerBehaviours()[unitId];
        return player.GetReserveAmmunition(weapon);
    }

    public int GetMagazineAmmunition(int unitId, Weapon weapon)
    {
        //Unit must be on your team
        PlayerBehaviour player = gc.GetPlayerBehaviours()[unitId];
        return player.GetMagazineAmmunition(weapon);
    }

    public Weapon GetWeapon(int unitId)
    {
        PlayerBehaviour player = gc.GetPlayerBehaviours()[unitId];
        return player.GetWeapon();
    }

    public float GetHealth(int unitId)
    {
        PlayerBehaviour player = gc.GetPlayerBehaviours()[unitId];
        return player.GetHealth();
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
        return mb.DistanceToUnit(unitId,targetId);
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
