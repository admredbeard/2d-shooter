using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI1 : MonoBehaviour
{
    APIScript api;
    GameController gc;
    List<int> myUnits; // These are the players you can control
    int myTeamId; //This id is used for certain API calls and is unique for your team
    int globalTarget = 10; // We use 10 for saying that there is no global target

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
            int target = GetBestVisualTarget(unitId);
            if (target != unitId)
            {
                if (api.WeaponSwapCooldown(unitId) < 0 && api.FireCooldown(unitId) < 0)
                {
                    float angle = api.AngleBetweenUnits(unitId, target);
                    api.LookAtDirection(unitId, angle);
                    float range = api.GetDistanceToUnit(unitId, target);
                    ReadyAimFire(unitId, range);
                }
                else
                {
                    Vector2 myPosition = api.GetWorldPosition(unitId);
                    Vector2 zonePosition = api.GetZonePosition();
                    if (zonePosition != null && (myPosition - zonePosition).magnitude < 15f)
                    {
                        //We are in zone, enemy is clear line of sight, take cover since we cant shoot?
                    }
                    else
                    {
                        //Move out of line of sight towards zone?
                    }
                }
            }
            else
            {
                List<int> nearby = api.SenseNearbyByTeam(unitId, -myTeamId);
                if (nearby.Count > 0)
                {
                    Vector2 myPosition = api.GetWorldPosition(unitId);
                    Vector2 zonePosition = api.GetZonePosition();
                    if (zonePosition != null && (myPosition - zonePosition).magnitude < 15f)
                    {
                        //We are in zone, enemy is probabyly too, do we chill?
                    }
                    else
                    {
                        //Move to Zone, Watch out for enemies
                    }
                }
                else
                {
                    Vector2 myPosition = api.GetWorldPosition(unitId);
                    Vector2 zonePosition = api.GetZonePosition();
                    if (zonePosition != null && (myPosition - zonePosition).magnitude < 15f)
                    {
                        //We are in zone, do we chill?
                    }
                    else
                    {
                        //Move to Zone
                    }

                }
            }
        }
    }

    void ReadyAimFire(int unitId, float range)
    {
        Weapon currentWeapon = api.GetWeapon(unitId);

        if (CanHit(range, currentWeapon))
        {
            if (api.CanFire(unitId))
                api.Attack(unitId);
            else if (api.GetMagazineAmmunition(unitId, currentWeapon) == 0 && api.GetReserveAmmunition(unitId, currentWeapon) > 0)
                api.ReloadWeapon(unitId);
        }
        else
            SwapWeapon(unitId, range);
    }

    bool CanHit(float range, Weapon currentWeapon)
    {

        if (currentWeapon == Weapon.Shotgun && range < shotGunRange)
            return true;
        else if (currentWeapon == Weapon.Pistol && range < pistolRange)
            return true;
        else if (currentWeapon == Weapon.Rifle)
            return true;
        else
            return false;

    }

    void SwapWeapon(int unitId, float range)
    {
        if (range < shotGunRange && GetTotalAmmo(unitId, Weapon.Shotgun) > 0)
            api.SwapWeapon(unitId, Weapon.Shotgun);
        else if (range < pistolRange && GetTotalAmmo(unitId, Weapon.Pistol) > 0)
            api.SwapWeapon(unitId, Weapon.Pistol);
        else if (GetTotalAmmo(unitId, Weapon.Rifle) > 0)
            api.SwapWeapon(unitId, Weapon.Rifle);
        else
            api.SwapWeapon(unitId, Weapon.Knife);
    }

    int GetTotalAmmo(int unitId, Weapon weapon)
    {
        return api.GetReserveAmmunition(unitId, weapon) + api.GetMagazineAmmunition(unitId, weapon);
    }
    int GetBestVisualTarget(int unitId)
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
