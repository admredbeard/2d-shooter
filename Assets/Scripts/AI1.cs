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

    void Start()
    {
        api = gameObject.GetComponent<APIScript>();
        myTeamId = api.teamId;
        myUnits = api.GetPlayers(myTeamId);
    }

    float shotGunRange = 10f;
    float pistolRange = 30f;

    // Update is called once per frame
    void FixedUpdate()
    {
        foreach (int unitId in myUnits)
        {
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
