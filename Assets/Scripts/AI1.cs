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
    Node[,] worldMap;
    bool[,] map;
    Vector2Int myGrid;
    Vector2 zonePos;
    int mapSize;
    Vector2Int targetPos;
    List<Node> path;
    List<Unit> units; //Unit class found at bottom of script, used to store goals and paths
    int stuckCount = 20;
    void Start()
    {
        path = new List<Node>();
        api = gameObject.GetComponent<APIScript>();
        worldMap = CreateMap();
        myTeamId = api.teamId;
        myUnits = api.GetPlayers(myTeamId);
        units = new List<Unit>();
        mapSize = api.GetMapSize();
        map = api.GetMap();
        targetPos = api.GetGridPos(myUnits[0]);

        foreach (int unitId in myUnits)
            units.Add(new Unit(unitId));

    }

    float shotGunRange = 10f;
    float pistolRange = 30f;

    // Update is called once per frame
    void FixedUpdate()
    {
        foreach (int unitId in myUnits)
        {
            Vector2 middle = GetMapMiddle();
            worldMap[GetXPos(middle.x), GetYPos(middle.y)].center = true;
            Unit currentUnit = GetUnit(unitId);
            if (currentUnit.myPath.Count == 0)
                currentUnit.myPath = FindPath(unitId, middle);

            zonePos = api.GetZonePosition();
            myGrid = api.GetGridPos(unitId);
            LetsMove(unitId, currentUnit);

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

    Unit GetUnit(int unitId)
    {
        foreach (Unit unit in units)
        {
            if (unit.myId == unitId)
                return unit;
        }

        return units[0];
    }

    void LetsMove(int unitId, Unit unit)
    {
        if (unit.iteration < unit.myPath.Count)
        {
            stuckCount++;
            Vector2 target = unit.myPath[unit.iteration].position;
            Vector2 pos = api.GetWorldPosition(unitId);

            if (Vector2.Distance(pos, target) < 2f)
            {
                unit.iteration++;
                if (unit.iteration < unit.myPath.Count)
                    target = unit.myPath[unit.iteration].position;
            }
            if (stuckCount > 100 && unit.myPath != null)
            {
                if (unit.iteration > 2 && (unit.lastPos - pos).magnitude < 2f)
                {
                    print("reverting");
                    unit.iteration -= 2;
                }
                unit.lastPos = pos;
                stuckCount = 0;
            }
            float moveAngle = ChangeAngle(unitId, api.GetGridPosFromWorldPos(target));
            api.Move(unitId, moveAngle);
        }
    }


    float ChangeAngle(int unitId, Vector2Int gridPos)
    {

        float angle = api.AngleBetweenUnitGridpos(unitId, gridPos);
        if (angle >= -45 && angle < 45)
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
        else if (angle >= 45 && angle < 135)
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
        else if (angle >= -135 && angle < -45)
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
        else if (angle >= 135 || angle < -135)
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
                    //bestTargetId = enemy;
                    closestRange = range;
                }
            }
        }

        return bestTargetId;
    }

    Vector2 FindLoSPosition(int unitId, int targetId)
    {

        bool[,] map = api.GetMap();
        Vector2Int myPosition = api.GetGridPos(unitId);
        Vector2Int bestPos = myPosition;
        float bestRange = 0f;

        foreach (Vector2Int grid in GetNeighbourGrids(myPosition, map, 5))
        {
            if (api.GridPositionInSight(targetId, grid))
            {
                float range = (myPosition - grid).magnitude;
                if (range < bestRange)
                {
                    bestRange = range;
                    bestPos = grid;
                }
            }
        }

        return api.GetWorldPosFromGridPos(bestPos);
    }

    List<Vector2Int> GetNeighbourGrids(Vector2Int currentPos, bool[,] map, int range)
    {
        List<Vector2Int> neighbours = new List<Vector2Int>();
        int size = api.GetMapSize();

        for (int i = -range + 1; i < range; i++)
        {
            for (int j = -range + 1; j < 2; j++)
            {
                if (currentPos.x + i >= 0 && currentPos.y + j >= 0 && size > currentPos.x + i && currentPos.y + j < size)
                {
                    if (j != 0 && i != 0)
                    {
                        if (map[currentPos.x + i, currentPos.y + j])
                            neighbours.Add(new Vector2Int(currentPos.x + i, currentPos.y + j));
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

        while (open.Count > 0 && count < 1500)
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
                print("path found!");
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
        print("Could not find path after: " + count + " iterations");
        return new List<Node>();
    }

    int GetXPos(float x)
    {
        return (int)(x / 2.5f);
    }

    int GetYPos(float y)
    {
        return (int)(y / 2.5f);
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
            return (x - z) * 14 + z * 10;
        }
        else
        {
            return (z - x) * 14 + x * 10;
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
                    if (x + 1 > 0 && y + 1 > 0 && y + 1 < size && x + 1 < size)
                    {
                        if (worldMap[x + 1, y].traversable && worldMap[x, y + 1].traversable)
                            neighbours.Add(worldMap[x, y]);
                    }
                }
                else if (i == -1 && j == 1)
                {
                    int x = node.xGrid + i;
                    int y = node.yGrid + j;
                    if (x + 1 > 0 && y - 1 > 0 && y - 1 < size && x + 1 < size)
                    {
                        if (worldMap[x + 1, y].traversable && worldMap[x, y - 1].traversable)
                            neighbours.Add(worldMap[x, y]);
                    }
                }
                else if (i == 1 && j == -1)
                {
                    int x = node.xGrid + i;
                    int y = node.yGrid + j;
                    if (x - 1 > 0 && y + 1 > 0 && y + 1 < size && x - 1 < size)
                    {
                        if (worldMap[x - 1, y].traversable && worldMap[x, y + 1].traversable)
                            neighbours.Add(worldMap[x, y]);
                    }
                }
                else if (i == 1 && j == 1)
                {
                    int x = node.xGrid + i;
                    int y = node.yGrid + j;
                    if (x - 1 > 0 && y - 1 > 0 && y - 1 < size && x - 1 < size)
                    {
                        if (worldMap[x - 1, y].traversable && worldMap[x, y - 1].traversable)
                            neighbours.Add(worldMap[x, y]);
                    }
                }
                else
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
                if (units[0].myPath.Contains(node))
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
                if (units[0].myPath.Contains(node))
                {
                    Gizmos.color = Color.blue;
                    Gizmos.DrawCube(node.position, new Vector3(1f, 1f, 1f));
                }
            }
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
    public Unit(int _id)
    {
        myPath = new List<Node>();
        lastPos = Vector2.zero;
        goal = Vector2.zero;
        myId = _id;
        iteration = 0;
    }
}
