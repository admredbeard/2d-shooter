using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI1 : MonoBehaviour
{
    APIScript api;
    List<int> myUnits; // These are the players you can control
    int myTeamId; //This id is used for certain API calls and is unique for your team
    Node[,] worldMap;
    bool[,] map;
    Vector2Int myGrid;
    Vector2 zonePos;
    float zoneRadius;
    int mapSize;
    Vector2Int targetPos;
    List<Unit> units; //Unit class found at bottom of script, used to store goals and paths
    int stuckCount = 20;
    void Start()
    {
        api = gameObject.GetComponent<APIScript>();
        worldMap = CreateMap();
        myTeamId = api.teamId;
        myUnits = api.GetPlayers(myTeamId);
        units = new List<Unit>();
        mapSize = api.GetMapSize();
        map = api.GetMap();
        targetPos = api.GetGridPos(myUnits[0]);
        zoneRadius = api.GetZoneRadius();
        foreach (int unitId in myUnits)
            units.Add(new Unit(unitId));

        StartCoroutine(WeaponCheck());
    }

    float shotGunRange = 10f;
    float pistolRange = 30f;

    // Update is called once per frame
    void FixedUpdate()
    {
        foreach (int unitId in myUnits)
        {
            Unit currentUnit = GetUnit(unitId);
            zonePos = GetZone();
            if ((currentUnit.myPath.Count == 0 && api.GetZonePosition() == Vector2.zero) || (currentUnit.myPath.Count > currentUnit.iteration && Vector2.Distance(currentUnit.myPath[currentUnit.iteration].position, api.GetWorldPosition(unitId)) > 10))
            {
                Vector2 middle = GetMapMiddle();
                worldMap[GetXPos(middle.x), GetYPos(middle.y)].center = true;
                currentUnit.goal = middle;
                GetNewPath(currentUnit, middle);
            }
            myGrid = api.GetGridPos(unitId);

            int target = GetBestVisualTarget(unitId);
            if (target != unitId)
            {
                if (currentUnit.knifeMode)
                {
                    if ((api.GetWorldPosition(unitId) - api.GetWorldPosition(target)).magnitude < 2f)
                    {
                        api.Attack(unitId);
                    }
                    else if (zonePos != Vector2.zero)
                    {
                        if ((api.GetWorldPosition(unitId) - zonePos).magnitude < zoneRadius)
                        {
                            Vector2 tarPos = api.GetWorldPosition(target);
                            if (currentUnit.goal != tarPos)
                            {
                                currentUnit.goal = tarPos;
                                GetNewPath(currentUnit, tarPos);
                            }
                        }
                        else if (currentUnit.goal != zonePos)
                        {
                            currentUnit.goal = zonePos;
                            GetNewPath(currentUnit, zonePos);
                        }
                    }
                }
                else if (api.WeaponSwapCooldown(unitId) < 0 && api.FireCooldown(unitId) < 0)
                {
                    float angle = api.AngleBetweenUnits(unitId, target);
                    api.LookAtDirection(unitId, angle);
                    float range = api.GetDistanceToUnit(unitId, target);
                    ReadyAimFire(unitId, range);
                }
                else
                {
                    Vector2 myPosition = api.GetWorldPosition(unitId);
                    if (zonePos != null && (myPosition - zonePos).magnitude < zoneRadius)
                    {
                        //We are in zone, enemy is clear line of sight, take cover since we cant shoot?
                    }
                    else if (currentUnit.goal != zonePos)
                    {
                        currentUnit.goal = zonePos;
                        GetNewPath(currentUnit, zonePos);
                    }
                }
            }
            else
            {
                List<int> nearby = api.SenseNearbyByTeam(unitId, -myTeamId);
                Vector2 myPosition = api.GetWorldPosition(unitId);
                if (nearby.Count > 0)
                {

                    List<int> nearbyTeammates = api.SenseNearbyByTeam(unitId, myTeamId);
                    int bestEnemy = FindNearestEnemy(unitId, nearby);
                    if (zonePos != Vector2.zero && (myPosition - zonePos).magnitude < zoneRadius)

                    {
                        if (nearbyTeammates.Count + 1 > nearby.Count)
                        {
                            Vector2 goalPos = FindLoSPosition(unitId, bestEnemy);
                            GetNewPath(currentUnit, goalPos);
                        }
                        else if (nearbyTeammates.Count + 1 == nearby.Count)
                        {
                            float myHp = api.GetHealth(unitId);
                            float enemyHp = api.GetHealth(bestEnemy);
                            if (myHp > enemyHp)
                            {
                                Vector2 goalPos = FindLoSPosition(unitId, bestEnemy);
                                GetNewPath(currentUnit, goalPos);
                                currentUnit.goal = goalPos;
                            }
                        }
                        else
                        {
                            //We are outnumbered or enemies with higher health, find cover?
                        }
                    }
                    else if (zonePos != Vector2.zero)
                    {
                        currentUnit.goal = zonePos;
                        GetNewPath(currentUnit, zonePos);
                    }
                }
                else if (zonePos != Vector2.zero)
                {
                    if ((myPosition - zonePos).magnitude < zoneRadius)
                    {
                        //We are in zone, do we chill?
                    }
                    else if (currentUnit.goal != zonePos)
                    {
                        currentUnit.goal = zonePos;
                        GetNewPath(currentUnit, zonePos);
                    }
                }
            }

            if (currentUnit.myPath.Count > 0)
            {
                LetsMove(currentUnit);
            }
        }
    }

    Unit GetUnit(int unitId)
    {
        foreach (Unit unit in units)
        {
            if (unit.myId == unitId)
                return unit;
        }

        return units[0];
    }

    int FindNearestEnemy(int unitId, List<int> enemies)
    {

        Vector2 myPos = api.GetWorldPosition(unitId);
        float bestRange = 10000f;
        float range = 0f;
        int bestTarget = unitId;

        foreach (int enemy in enemies)
        {
            range = (myPos - api.GetWorldPosition(enemy)).magnitude;
            if (range < bestRange)
            {
                bestTarget = enemy;
                bestRange = range;
            }
        }

        return bestTarget;
    }
    void LetsMove(Unit unit)
    {
        Vector2 pos = api.GetWorldPosition(unit.myId);
        if (unit.iteration < unit.myPath.Count)
        {
            stuckCount++;
            Vector2 target = unit.myPath[unit.iteration].position;
            if (Vector2.Distance(pos, target) < 1f)
            {
                unit.iteration++;
                if (unit.iteration < unit.myPath.Count)
                    target = unit.myPath[unit.iteration].position;
            }
            if (stuckCount > 100 && unit.myPath != null)
            {
                if (unit.iteration > 2 && (unit.lastPos - pos).magnitude < 1f)
                {
                    unit.goal = api.GetGridPos(unit.myId);
                    GetNewPath(unit, unit.goal);
                }
                unit.lastPos = pos;
                stuckCount = 0;
            }
            float moveAngle = ChangeAngle(unit.myId, api.GetGridPosFromWorldPos(target));
            api.Move(unit.myId, moveAngle);
        }
        else
        {
            unit.iteration = 0;
            unit.myPath.Clear();
            unit.lastPos = pos;
            unit.goal = pos;
        }
    }

    void GetNewPath(Unit unit, Vector2 goal)
    {
        unit.myPath.Clear();
        unit.goal = goal;
        unit.iteration = 0;
        unit.myPath = FindPath(unit.myId, goal);
    }

    float ChangeAngle(int unitId, Vector2Int gridPos)
    {

        float angle = api.AngleBetweenUnitGridpos(unitId, gridPos);
        if (angle >= -22.5f && angle < 22.5f)
        {
            if (myGrid.x + 1 < mapSize && map[myGrid.x + 1, myGrid.y])
            {
                targetPos = api.GetGridPos(unitId) + Vector2Int.right;
                return api.AngleBetweenUnitGridpos(unitId, api.GetGridPos(unitId) + Vector2Int.right);
            }
            else if (myGrid.y + 1 < mapSize && map[myGrid.x, myGrid.y + 1])
            {
                targetPos = api.GetGridPos(unitId) + Vector2Int.up;
                return api.AngleBetweenUnitGridpos(unitId, api.GetGridPos(unitId) + Vector2Int.up);
            }
            else if (myGrid.y != 0 && map[myGrid.x, myGrid.y - 1])
            {
                targetPos = api.GetGridPos(unitId) - Vector2Int.up;
                return api.AngleBetweenUnitGridpos(unitId, api.GetGridPos(unitId) - Vector2Int.up);
            }
        }
        else if (angle >= 22.5f && angle < 67.5f)
        {
            targetPos = api.GetGridPos(unitId) + Vector2Int.up;
            return api.AngleBetweenUnitGridpos(unitId, api.GetGridPos(unitId) + Vector2Int.up + Vector2Int.right);
        }
        else if (angle >= 67.5f && angle < 112.5f)
        {
            if (myGrid.y + 1 < mapSize && map[myGrid.x, myGrid.y + 1])
            {
                targetPos = api.GetGridPos(unitId) + Vector2Int.up;
                return api.AngleBetweenUnitGridpos(unitId, api.GetGridPos(unitId) + Vector2Int.up);
            }
            else if (myGrid.x + 1 < mapSize && map[myGrid.x + 1, myGrid.y])
            {
                targetPos = api.GetGridPos(unitId) + Vector2Int.right;
                return api.AngleBetweenUnitGridpos(unitId, api.GetGridPos(unitId) + Vector2Int.right);
            }
            else if (myGrid.x != 0 && map[myGrid.x - 1, myGrid.y])
            {
                targetPos = api.GetGridPos(unitId) - Vector2Int.right;
                return api.AngleBetweenUnitGridpos(unitId, api.GetGridPos(unitId) - Vector2Int.right);
            }
        }
        else if (angle >= 112.5f && angle < 157.5)
        {
            targetPos = api.GetGridPos(unitId) + Vector2Int.up;
            return api.AngleBetweenUnitGridpos(unitId, api.GetGridPos(unitId) + Vector2Int.up - Vector2Int.right);
        }
        else if (angle >= -112.5f && angle < -67.5f)
        {
            if (myGrid.y != 0 && map[myGrid.x, myGrid.y - 1])
            {
                targetPos = api.GetGridPos(unitId) - Vector2Int.up;
                return api.AngleBetweenUnitGridpos(unitId, api.GetGridPos(unitId) - Vector2Int.up);
            }
            else if (map[myGrid.x + 1, myGrid.y])
            {
                targetPos = api.GetGridPos(unitId) + Vector2Int.right;
                return api.AngleBetweenUnitGridpos(unitId, api.GetGridPos(unitId) + Vector2Int.right);
            }
            else if (myGrid.x != 0 && map[myGrid.x - 1, myGrid.y])
            {
                targetPos = api.GetGridPos(unitId) - Vector2Int.right;
                return api.AngleBetweenUnitGridpos(unitId, api.GetGridPos(unitId) - Vector2Int.right);
            }
        }
        else if (angle >= -67.5f && angle < -27.5f)
        {
            targetPos = api.GetGridPos(unitId) + Vector2Int.up;
            return api.AngleBetweenUnitGridpos(unitId, api.GetGridPos(unitId) - Vector2Int.up + Vector2Int.right);
        }
        else if (angle >= -157.5 && angle < -112.5f)
        {
            targetPos = api.GetGridPos(unitId) + Vector2Int.up;
            return api.AngleBetweenUnitGridpos(unitId, api.GetGridPos(unitId) - Vector2Int.up - Vector2Int.right);
        }
        else if (angle >= 157.5 || angle < -157.5)
        {
            if (myGrid.x != 0 && map[myGrid.x - 1, myGrid.y])
            {
                targetPos = api.GetGridPos(unitId) - Vector2Int.right;
                return api.AngleBetweenUnitGridpos(unitId, api.GetGridPos(unitId) - Vector2Int.right);
            }
            else if (myGrid.y + 1 < mapSize && map[myGrid.x, myGrid.y + 1])
            {
                targetPos = api.GetGridPos(unitId) + Vector2Int.up;
                return api.AngleBetweenUnitGridpos(unitId, api.GetGridPos(unitId) + Vector2Int.up);
            }
            else if (myGrid.y != 0 && map[myGrid.x, myGrid.y - 1])
            {
                targetPos = api.GetGridPos(unitId) - Vector2Int.up;
                return api.AngleBetweenUnitGridpos(unitId, api.GetGridPos(unitId) - Vector2Int.up);
            }
        }
        return 0;
    }

    void ReadyAimFire(int unitId, float range)
    {
        Weapon currentWeapon = api.GetWeapon(unitId);
        int currAmmo = GetTotalAmmo(unitId, currentWeapon);
        if (CanHit(range, currentWeapon) && currAmmo > 0)
        {
            if (api.CanFire(unitId))
                api.Attack(unitId);
            else if (api.GetMagazineAmmunition(unitId, currentWeapon) == 0 && api.GetReserveAmmunition(unitId, currentWeapon) > 0 && api.ReloadCooldown(unitId) < 0f)
                api.ReloadWeapon(unitId);
        }
        else
        {
            SwapWeapon(unitId, range);
        }
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
        int shotgunAmmo = GetTotalAmmo(unitId, Weapon.Shotgun);
        int pistolAmmo = GetTotalAmmo(unitId, Weapon.Pistol);
        int rifleAmmo = GetTotalAmmo(unitId, Weapon.Rifle);

        if (range < shotGunRange && shotgunAmmo > 0)
            api.SwapWeapon(unitId, Weapon.Shotgun);
        else if (range < pistolRange && pistolAmmo > 0)
            api.SwapWeapon(unitId, Weapon.Pistol);
        else if (rifleAmmo > 0)
            api.SwapWeapon(unitId, Weapon.Rifle);
        else if (pistolAmmo > 0)
            api.SwapWeapon(unitId, Weapon.Pistol);
        else if (shotgunAmmo > 0)
            api.SwapWeapon(unitId, Weapon.Pistol);
        else if (api.GetWeapon(unitId) != Weapon.Knife)
            api.SwapWeapon(unitId, Weapon.Knife);
        else
        {
            GetUnit(unitId).knifeMode = true;
        }
    }

    int GetTotalAmmo(int unitId, Weapon weapon)
    {
        return api.GetReserveAmmunition(unitId, weapon) + api.GetMagazineAmmunition(unitId, weapon);
    }

    int GetBestVisualTarget(int unitId)
    {

        float closestRange = 10000f;
        int bestTargetId = unitId;
        List<int> visitedEnemies = new List<int>();
        Vector2 unitPosition = api.GetWorldPosition(unitId);
        foreach (int id in myUnits)
        {
            List<int> enemies = api.SenseNearbyByTeam(id, -myTeamId);
            bool bestTargetInVision = false;

            if (enemies.Count > 0)
            {
                foreach (int enemy in enemies)
                {
                    if (!visitedEnemies.Contains(enemy))
                    {
                        visitedEnemies.Add(enemy);
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
                                closestRange = range;
                            }
                        }
                    }
                }
            }
        }

        return bestTargetId;
    }

    Vector2 FindLoSPosition(int unitId, int targetId)
    {

        bool[,] map = api.GetMap();
        Vector2 myPosition = api.GetWorldPosition(unitId);
        Node bestNode = worldMap[GetXPos(myPosition.x), GetYPos(myPosition.y)];

        float bestRange = 0f;

        foreach (Node node in GetNeighbourNodes(myPosition, 3))
        {
            if (api.WorldPositionInSight(targetId, node.position))
            {
                float range = (myPosition - node.position).magnitude;
                if (range < bestRange)
                {
                    bestRange = range;
                    bestNode = node;
                }
            }
        }

        return bestNode.position;
    }

    List<Node> GetNeighbourNodes(Vector2 currentPos, int range)
    {
        List<Node> neighbours = new List<Node>();
        Node currentNode = worldMap[GetXPos(currentPos.x), GetYPos(currentPos.y)];
        int size = api.GetMapSize();

        for (int i = -range + 1; i < range; i++)
        {
            for (int j = -range + 1; j < 2; j++)
            {
                int x = currentNode.xGrid + i;
                int y = currentNode.yGrid + j;
                if (x >= 0 && y >= 0 && size > x && y < size)
                {
                    if (j != 0 && i != 0)
                    {
                        if (worldMap[x, y].traversable)
                            neighbours.Add(worldMap[x, y]);
                    }
                }
            }
        }
        return neighbours;
    }

    List<Node> FindPath(int unitId, Vector2 _goal)
    {

        List<Node> path = new List<Node>();
        Vector2 playerPos = api.GetWorldPosition(unitId);
        Node goal = worldMap[GetXPos(_goal.x), GetYPos(_goal.y)];
        Node start = worldMap[GetXPos(playerPos.x), GetYPos(playerPos.y)];
        List<Node> open = new List<Node>();
        List<Node> closed = new List<Node>();
        open.Add(start);
        int count = 0;

        while (open.Count > 0 && count < 2500)
        {

            count++;
            Node current = open[0];

            for (int i = 1; i < open.Count; i++)
            {
                if (open[i].Cost < current.Cost || (open[i].Cost == current.Cost && open[i].hCost < current.hCost))
                    current = open[i];
            }

            open.Remove(current);
            closed.Add(current);

            if (current.position == goal.position)
            {
                return Path(start, goal);
            }

            foreach (Node neighbour in GetNeighbours(current))
            {
                if (!neighbour.traversable || closed.Contains(neighbour))
                    continue;

                float newCost = current.gCost + GetDistance(current, neighbour);
                if (newCost < neighbour.gCost || !open.Contains(neighbour))
                {
                    current.child = neighbour;
                    neighbour.parent = current;
                    neighbour.gCost = current.gCost + newCost;
                    neighbour.hCost = GetDistance(current, goal);

                    if (!open.Contains(neighbour))
                        open.Add(neighbour);

                }
            }
        }
        goal.goaled = true;
        start.goaled = true;
        print("goal: " + goal.position + " start: " + start.position);
        print("Could not find path after: " + count + " iterations");
        return new List<Node>();
    }

    int GetXPos(float x)
    {
        return (int)Mathf.Round(x / 2.5f);
    }

    int GetYPos(float y)
    {
        return (int)Mathf.Round(y / 2.5f);
    }
    List<Node> Path(Node start, Node goal)
    {
        List<Node> newPath = new List<Node>();

        Node current = goal;
        int count = 0;
        while (current.position != start.position && current != null && count < 100)
        {
            count++;
            newPath.Add(current);
            current = current.parent;

            if (current == null)
                break;
        }
        newPath.Reverse();
        return newPath;
    }
    float GetDistance(Node nodeA, Node nodeB)
    {
        float x = Mathf.Abs(nodeA.position.x - nodeB.position.x);
        float z = Mathf.Abs(nodeA.position.y - nodeB.position.y);
        if (x > z)
        {
            return (x - z) * 10 + z * 14;
        }
        else
        {
            return (z - x) * 10 + x * 14;
        }

    }

    List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();
        bool[,] map = api.GetMap();
        int size = api.GetMapSize();

        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {

                if (i == -1 && j == -1)
                {
                    int x = node.xGrid + i;
                    int y = node.yGrid + j;
                    if (x > 0 && y > 0 && y + 1 < size && x + 1 < size)
                    {
                        if (worldMap[x + 1, y].traversable && worldMap[x, y + 1].traversable)
                            neighbours.Add(worldMap[x, y]);
                    }
                }
                else if (i == -1 && j == 1)
                {
                    int x = node.xGrid + i;
                    int y = node.yGrid + j;
                    if (x > 0 && y - 1 > 0 && y < size && x + 1 < size)
                    {
                        if (worldMap[x + 1, y].traversable && worldMap[x, y - 1].traversable)
                            neighbours.Add(worldMap[x, y]);
                    }
                }
                else if (i == 1 && j == -1)
                {
                    int x = node.xGrid + i;
                    int y = node.yGrid + j;
                    if (x - 1 > 0 && y > 0 && y + 1 < size && x < size)
                    {
                        if (worldMap[x - 1, y].traversable && worldMap[x, y + 1].traversable)
                            neighbours.Add(worldMap[x, y]);
                    }
                }
                else if (i == 1 && j == 1)
                {
                    int x = node.xGrid + i;
                    int y = node.yGrid + j;
                    if (x - 1 > 0 && y - 1 > 0 && y < size && x < size)
                    {
                        if (worldMap[x - 1, y].traversable && worldMap[x, y - 1].traversable)
                            neighbours.Add(worldMap[x, y]);
                    }
                }
                else if (!(i == 0 && j == 0))
                {
                    int x = node.xGrid + i;
                    int y = node.yGrid + j;
                    if (x >= 0 && y >= 0 && size > x && size > y)
                    {
                        neighbours.Add(worldMap[x, y]);
                    }
                }
            }
        }
        return neighbours;
    }

    Vector2 GetMapMiddle()
    {
        bool[,] map = api.GetMap();
        float x = api.GetMapSize() * 2.5f / 2;
        float y = api.GetMapSize() * 2.5f / 2;
        Vector2 pos = new Vector2(x, y);
        if (map[GetXPos(x), GetYPos(y)])
            return pos;
        else
        {
            for (int i = -2; i < 3; i++)
            {
                for (int j = -2; j < 3; j++)
                {
                    if (map[GetXPos(x + (i * 2.5f)), GetYPos(y + (j * 2.5f))])
                        return new Vector2(pos.x + i * 2.5f, pos.y + j * 2.5f);
                }
            }
        }
        return new Vector2(0f, 0f);
    }

    Vector2 GetZone()
    {
        bool[,] map = api.GetMap();
        float x = api.GetZonePosition().x;
        float y = api.GetZonePosition().y;
        Vector2 pos = new Vector2(x, y);
        if (map[GetXPos(x), GetYPos(y)])
            return pos;
        else
        {
            for (int i = -2; i < 3; i++)
            {
                for (int j = -2; j < 3; j++)
                {
                    if (map[GetXPos(x + (i * 2.5f)), GetYPos(y + (j * 2.5f))])
                        return new Vector2(pos.x + i * 2.5f, pos.y + j * 2.5f);
                }
            }
        }
        return new Vector2(0f, 0f);
    }

    void OnDrawGizmos()
    {

        if (worldMap != null && units[0].myPath != null)
        {
            foreach (Node node in worldMap)
            {
                if (units[0].myPath.Contains(node))
                {
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawCube(node.position, new Vector3(1f, 1f, 1f));
                }
                if (node.center)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawCube(node.position, new Vector3(1f, 1f, 1f));
                }
            }
        }
        if (worldMap != null && units[1].myPath != null)
        {
            foreach (Node node in worldMap)
            {
                if (units[1].myPath.Contains(node))
                {
                    Gizmos.color = Color.gray;
                    Gizmos.DrawCube(node.position, new Vector3(1f, 1f, 1f));
                }
            }
        }
        if (worldMap != null && units[2].myPath != null)
        {
            foreach (Node node in worldMap)
            {
                if (units[2].myPath.Contains(node))
                {
                    Gizmos.color = Color.blue;
                    Gizmos.DrawCube(node.position, new Vector3(1f, 1f, 1f));
                }
            }
        }
        if (worldMap != null)
        {
            foreach (Node node in worldMap)
            {
                if (node.goaled)
                {
                    Gizmos.color = Color.black;
                    Gizmos.DrawCube(node.position, new Vector3(1.5f, 1.5f, 1.5f));
                }
            }
        }
    }

    IEnumerator WeaponCheck()
    {
        yield return new WaitForSeconds(2.5f);
        while (true)
        {

            foreach (int unitId in myUnits)
            {
                Weapon currentWeapon = api.GetWeapon(unitId);
                float swapTimer = api.WeaponSwapCooldown(unitId);
                float reloadTimer = api.ReloadCooldown(unitId);

                if (currentWeapon == Weapon.Knife && swapTimer < 0f && reloadTimer < 0f)
                {
                    if (GetTotalAmmo(unitId, Weapon.Pistol) > 0)
                        api.SwapWeapon(unitId, Weapon.Pistol);
                }
                else if (currentWeapon != Weapon.Knife && GetTotalAmmo(unitId, currentWeapon) == 0 && swapTimer < 0f && reloadTimer < 0f)
                {
                    int pistolAmmo = GetTotalAmmo(unitId, Weapon.Pistol);
                    int rifleAmmo = GetTotalAmmo(unitId, Weapon.Rifle);
                    int shotgunAmmo = GetTotalAmmo(unitId, Weapon.Shotgun);
                    if (pistolAmmo > 0)
                        api.SwapWeapon(unitId, Weapon.Pistol);
                    else if (rifleAmmo > 0)
                        api.SwapWeapon(unitId, Weapon.Rifle);
                    else if (shotgunAmmo > 0)
                        api.SwapWeapon(unitId, Weapon.Shotgun);
                    else
                        api.SwapWeapon(unitId, Weapon.Knife);
                }
                else if (currentWeapon == Weapon.Pistol && api.GetMagazineAmmunition(unitId, currentWeapon) < 2 && api.FireCooldown(unitId) < 0f && GetTotalAmmo(unitId, Weapon.Pistol) > 4)
                {
                    api.ReloadWeapon(unitId);
                }
                else if (GetTotalAmmo(unitId, currentWeapon) > 0 && api.GetMagazineAmmunition(unitId, currentWeapon) == 0)
                    api.ReloadWeapon(unitId);

            }

            yield return new WaitForSeconds(2f);
        }
    }

    Node[,] CreateMap()
    {
        int size = api.GetMapSize();
        bool[,] traversableMap = api.GetMap();
        Node[,] newMap = new Node[size, size];
        float gridSize = 2.5f;
        Vector2 pos = new Vector2(0f, 0f);

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                if (traversableMap[i, j])
                {
                    pos = new Vector2(i * gridSize, j * gridSize);
                    newMap[i, j] = new Node(true, pos, i, j);
                }
                else
                {
                    pos = new Vector2(i * gridSize, j * gridSize);
                    newMap[i, j] = new Node(false, pos, i, j);
                }
            }
        }
        return newMap;
    }
}


class Node
{
    public bool traversable = false;
    public Vector2 position;
    public float gCost = 0;
    public float hCost = 0;
    public bool center = false;
    public int xGrid;
    public int yGrid;
    public Node parent;
    public Node child;

    public bool goaled = false;
    public Node(bool _traversable, Vector2 _position, int x, int y)
    {
        traversable = _traversable;
        position = _position;
        xGrid = x;
        yGrid = y;
    }

    public float Cost
    {
        get { return gCost + hCost; }
    }
}

class Unit
{
    public List<Node> myPath;
    public Vector2 goal;
    public int myId;
    public int iteration;
    public Vector2 lastPos;
    public bool knifeMode = false;
    public Unit(int _id)
    {
        myPath = new List<Node>();
        lastPos = Vector2.zero;
        goal = Vector2.zero;
        myId = _id;
        iteration = 0;
    }
}
