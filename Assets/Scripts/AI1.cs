using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI1 : MonoBehaviour
{
    APIScript api;
    GameController gc;
    List<int> myUnits; // These are the players you can control
    int myTeamId; //This id is used for certain API calls and is unique for your team
    int globalTarget = 10;
    bool[,] map;
    Vector2Int myGrid;
    Vector2 zonePos;
    int mapSize;
    void Start()
    {
        api = gameObject.GetComponent<APIScript>();
        myTeamId = api.teamId;
        myUnits = api.GetPlayers(myTeamId);
        map = api.GetMap();
        mapSize = api.GetMapSize();
    }

    float shotGunRange = 10f;
    float pistolRange = 30f;

    // Update is called once per frame
    void FixedUpdate()
    {
        foreach (int unitId in myUnits)
        {
            zonePos = api.GetZonePosition();
            myGrid = api.GetGridPos(unitId);
            LetsMove(unitId);
            int target = GetBestTargetNearby(unitId);
            if (target != unitId)
            {
                float angle = 0f;
                api.LookAtDirection(unitId, angle);
                float range = api.GetDistanceToUnit(unitId, target);
                if (range < shotGunRange && api.GetWeapon(unitId) == Weapon.Shotgun)
                {
                    if (api.CanFire(unitId))
                    {
                        api.Attack(unitId);
                    }
                }
            }
        }
    }
    void LetsMove(int unitId)
    {
        UberMoveFunction(unitId, zonePos);
    }

    
    void UberMoveFunction(int unitId, Vector2 targetPos)
    {
        float angle = api.AngleBetweenUnitWorldpos(unitId, targetPos);
        if(angle >= -45 && angle < 45)
        {
            if(myGrid.x + 1 < mapSize && map[myGrid.x + 1, myGrid.y])
            {
                api.Move(unitId, 0);
                return;
            }
            else if (myGrid.y + 1 < mapSize && map[myGrid.x, myGrid.y + 1])
            {
                api.Move(unitId, 90);
                return;
            }
            else if (myGrid.y != 0 && map[myGrid.x, myGrid.y - 1])
            {
                api.Move(unitId, -90);
                return;
            }
        }
        else if(angle >= 45 && angle < 135)
        {
            if (myGrid.y + 1 < mapSize && map[myGrid.x, myGrid.y + 1])
            {
                api.Move(unitId, 90);
                return;
            }
            else if (myGrid.x + 1 < mapSize && map[myGrid.x + 1, myGrid.y])
            {
                api.Move(unitId, 0);
                return;
            }
            else if (myGrid.x != 0 && map[myGrid.x - 1, myGrid.y])
            {
                api.Move(unitId, 180);
                return;
            }
        }
        else if (angle >= -135 && angle < -45)
        {
            if (myGrid.y != 0 && map[myGrid.x, myGrid.y - 1])
            {
                api.Move(unitId, -90);
                return;
            }
            else if (map[myGrid.x + 1, myGrid.y])
            {
                api.Move(unitId, 0);
                return;
            }
            else if (myGrid.x != 0 && map[myGrid.x - 1, myGrid.y])
            {
                api.Move(unitId, 180);
                return;
            }
        }
        else if (angle >= 135 || angle < -135)
        {
            if (myGrid.x != 0 && map[myGrid.x - 1, myGrid.y])
            {
                api.Move(unitId, 180);
                return;
            }
            else if (myGrid.y + 1 < mapSize && map[myGrid.x, myGrid.y + 1])
            {
                api.Move(unitId, 90);
                return;
            }
            else if (myGrid.y != 0 && map[myGrid.x, myGrid.y - 1])
            {
                api.Move(unitId, -90);
                return;
            }
        }
    }

    int GetBestTargetNearby(int unitId)
    {
        List<int> enemies = api.SenseNearbyByTeam(unitId, -myTeamId);
        float closestRange = 10000f;
        int bestTargetId = unitId;
        bool bestTargetInVision = false;

        if (enemies.Count > 0)
        {
            Vector2 unitPosition = api.GetWorldPosition(unitId);
            foreach (int enemy in enemies)
            {
                float range = Vector3.Distance(unitPosition, api.GetWorldPosition(enemy));
                if (api.TargetInSight(unitId, enemy))
                {
                    if (bestTargetInVision)
                    {
                        if (range < closestRange)
                            bestTargetId = enemy;
                    }
                    else
                    {
                        bestTargetInVision = true;
                        bestTargetId = enemy;
                    }
                }
                else if (range < closestRange)
                {
                    bestTargetId = enemy;
                    closestRange = range;
                }
            }
        }

        return bestTargetId;
    }
}
