using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class APIScript : MonoBehaviour
{
    GameController gc;
    void Start()
    {
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

        Vector2 position = new Vector2();
        return position;
    }

    public Vector2Int GetGridPos(int unitId)
    {
        //Unit must be in vision range or in our team

        Vector2Int position = new Vector2Int();
        return position;
    }

    public bool TargetInSight(int unitId, int targetId)
    {
        //Unit must be on your team
        return false;
    }

    public bool WorldPositionInSight(int unitId, Vector2 worldPosition)
    {
        //Unit must be on your team
        return false;
    }

    public bool GridPositionInSight(int unitId, Vector2Int worldPosition)
    {
        //Unit must be on your team
        return false;
    }

    public void Attack(int unitId)
    {
        //Unit must be on your team
    }

    public void SwapWeapon(int unitId, Weapon newWeapon)
    {
        //Unit must be on your team
    }

    public void ReloadWeapon(int unitId)
    {
        //Unit must be on your team
    }

    public float GetTotalAmmunition(int unitId)
    {
        //Unit must be on your team
        return 0f;
    }

    public float GetMagazineAmmunition(int unitId)
    {
        //Unit must be on your team
        return 0f;
    }

    public Weapon GetWeapon(int unitId)
    {
        return Weapon.Knife;
    }

    public float GetHealth(int unitId)
    {
        return 0f;
    }

    public bool IsGridPositionTraversable(Vector2Int gridPosition)
    {
        return false;
    }

    public bool IsWorldPositionTraversable(Vector2 worldPosition)
    {
        return false;
    }

    public bool[,] GetMap()
    {
        bool[,] map = new bool[100, 100];
        return map;
    }

    public float GetDistanceToUnit(int unitId, int targetId)
    {
        //Unit must be in vision range or in our team
        return 0f;
    }

    public float GetDistanceToGridPosition(int unitId, Vector2Int gridPosition)
    {
        //Unit must be in vision range or in our team
        return 0f;
    }

    public float GetDistanceToWorldPosition(int unitId, Vector2 worldPosition)
    {
        //Unit must be in vision range or in our team
        return 0f;
    }
}
