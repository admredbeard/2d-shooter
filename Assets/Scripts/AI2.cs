using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI2 : MonoBehaviour
{
    APIScript api;
    GameController gc;
    List<int> myUnits; // These are the players you can control
    int myTeamId; //This id is used for certain API calls and is unique for your team
    bool[,] map;
    int mapSize;
    private Node[,] initialized_map;

    public bool debugPath = false;

    int maxMagAmmoRifle = 1;
    int maxMagAmmoShotgun = 2;
    int maxMagAmmoPistol = 5;

    int idPlayer1;
    int idPlayer2;
    int idPlayer3;
    
    void Start()
    {
        api = gameObject.GetComponent<APIScript>();
        map = api.GetMap();
        myTeamId = api.teamId;
        myUnits = api.GetPlayers(myTeamId);
        mapSize = (int)Mathf.Sqrt(map.Length);
        
        Init();
    }

    Vector2 oldZone;
    bool once = true;
    // Update is called once per frame
    void FixedUpdate()
    {
        
        if(api.GetZonePosition() != oldZone)
        {
            oldZone = api.GetZonePosition();
        }
        
        List<int> enemies = api.SenseNearbyByTeam(idPlayer1, -myTeamId);
        enemies.AddRange(api.SenseNearbyByTeam(idPlayer2, -myTeamId));
        enemies.AddRange(api.SenseNearbyByTeam(idPlayer3, -myTeamId));

        foreach (int unit in myUnits)
        {
            //using try for unauthorizedaccessexception
            try
            {
                if (enemies.Count > 0 && api.TargetInSight(unit, enemies[0]))
                {
                    api.LookAtDirection(unit, api.AngleBetweenUnits(unit, enemies[0]));
                    if (api.GetWeapon(unit) == Weapon.Knife)
                    {
                        api.Attack(unit);
                    }
                    else if (api.CanFire(unit) && api.GetDistanceToUnit(unit, enemies[0]) < GetWeaponRange(api.GetWeapon(unit)))
                    {
                        api.Attack(unit);
                    }
                }
                //Do some local goal;
                if (unit == idPlayer1)
                {
                    unit1Path = FollowPath(idPlayer1, unit1Path);
                }
                else if (unit == idPlayer2)
                {
                    unit2Path = FollowPath(idPlayer2, unit2Path);
                }
                else if (unit == idPlayer3)
                {
                    unit3Path = FollowPath(idPlayer3, unit3Path);
                }
               
            }
            catch(System.UnauthorizedAccessException e)
            {
                Debug.Log(e);
            }
            if (api.WeaponSwapCooldown(unit) < 0)
            {
                SwapWeapon(unit, enemies);
            }
            Reload(unit, enemies);
        }
    }

    public void Reload(int unitID, List<int> enemies)
    {
        int magAmmo = api.GetMagazineAmmunition(unitID, api.GetWeapon(unitID));
        if (api.ReloadCooldown(unitID) < 0 && api.WeaponSwapCooldown(unitID) < 0 && magAmmo < GetCurrentWeaponMaxAmmo(api.GetWeapon(unitID)) && api.GetReserveAmmunition(unitID, api.GetWeapon(unitID)) > 0)
        {
            if(enemies.Count == 0 || api.GetDistanceToUnit(unitID, enemies[0]) > 30 || !api.TargetInSight(unitID, GetBestEnemy(unitID, enemies)) || magAmmo == 0)
            {
                api.ReloadWeapon(unitID);
            }
        }
    }

    private float GetWeaponRange(Weapon wep)
    {
        if(wep == Weapon.Pistol)
        {
            return 30;
        }
        else if(wep == Weapon.Shotgun)
        {
            return 20;
        }
        else if(wep == Weapon.Rifle)
        {
            return 60;
        }
        else
        {
            return 3;
        }
    }

    public int GetCurrentWeaponMaxAmmo(Weapon wep)
    {
        if(wep == Weapon.Pistol)
        {
            return maxMagAmmoPistol;
        }else if (wep == Weapon.Shotgun)
        {
            return maxMagAmmoShotgun;
        }else if(wep == Weapon.Rifle)
        {
            return maxMagAmmoRifle;
        }
        else
        {
            return 0;
        }
    }

    List<Node> unit1Path;
    List<Node> unit2Path;
    List<Node> unit3Path;

    public void Init()
    {
        initialized_map = new Node[mapSize, mapSize];
        oldZone = api.GetZonePosition();
        unit1Path = new List<Node>();
        unit2Path = new List<Node>();
        unit3Path = new List<Node>();

        idPlayer1 = myUnits[0];
        idPlayer2 = myUnits[1];
        idPlayer3 = myUnits[2];

        for (int i = 0; i < mapSize; i++)
        {
            for (int j = 0; j < mapSize; j++)
            {
                bool trav = map[i, j];
                Vector2 worldPos = api.GetWorldPosFromGridPos(i, j);
                initialized_map[i, j] = new Node(worldPos, i, j, trav);
                initialized_map[i, j].visionScore = CalculateNodeValue(new Vector2(i, j));
            }
        }
        Debug.Log(mapSize);
        Debug.Log(initialized_map[0, 0].visionScore);

        averageScore = totalScore / mapSize * mapSize;

        //Just some tests of A*:::
        Vector2Int aUnitPos = api.GetGridPos(myUnits[0]);
        Node testnode1 = initialized_map[aUnitPos.x, aUnitPos.y];
        Node testnode2 = initialized_map[10, 10];
        Node testnode3 = initialized_map[0, 10];
        Node testnode4 = initialized_map[10, 0];


        unit1Path = FindPath(testnode1, testnode2);
        unit2Path = FindPath(testnode1, testnode3);
        unit3Path = FindPath(testnode1, testnode4);

        if (debugPath)
        {
            ShowPath(unit1Path);
        }
    }

    int totalScore = 0;
    float averageScore = 0;

    private int CalculateNodeValue(Vector2 from)
    {

        List<Vector2Int> visited = new List<Vector2Int>();
        for(int j = 0; j < 4; j++)
        {
            for (int i = 0; i < mapSize; i++)
            {
                if (j == 0)
                {
                    //do with x == 0
                    //line equation---- y-y / x-x  om x1 != x2
                    Vector2 to = new Vector2Int(0, i);
                    Vector2 line = to - from;
                    int paramLimit = (int)Mathf.Floor(line.magnitude);
                    line.Normalize();
                    for (int k = 1; k < paramLimit; k++)
                    {
                        Vector2 next = from + line * k;
                        int x = (int)Mathf.Round(next.x);
                        int y = (int)Mathf.Round(next.y);
                        Vector2Int candidate = new Vector2Int(x, y);
                        
                        if (map[x, y] && !visited.Contains(candidate))
                        {
                            visited.Add(candidate);
                        }
                        else if(!map[x,y])
                        {
                            break;
                        }
                    }
                }
                else if (j == 1)
                {
                    //do with x == 40
                    Vector2 to = new Vector2Int(mapSize - 1, i);
                    Vector2 line = to - from;
                    int paramLimit = (int)Mathf.Floor(line.magnitude);
                    line.Normalize();
                    for (int k = 1; k < paramLimit; k++)
                    {
                        Vector2 next = from + line * k;
                        int x = (int)Mathf.Round(next.x);
                        int y = (int)Mathf.Round(next.y);
                        Vector2Int candidate = new Vector2Int(x, y);

                        if (map[x, y] && !visited.Contains(candidate))
                        {
                            visited.Add(candidate);
                        }
                        else if (!map[x, y])
                        {
                            break;
                        }
                    }
                }
                else if (j == 2)
                {
                    //do with y == 0
                    Vector2 to = new Vector2Int(i, 0);
                    Vector2 line = to - from;
                    int paramLimit = (int)Mathf.Floor(line.magnitude);
                    line.Normalize();
                    if(i == 0)
                    {
                        continue;
                    }
                    for (int k = 1; k < paramLimit; k++)
                    {
                        if(paramLimit == mapSize - 1)
                        {
                            break;
                        }
                        Vector2 next = from + line * k;
                        int x = (int)Mathf.Round(next.x);
                        int y = (int)Mathf.Round(next.y);
                        Vector2Int candidate = new Vector2Int(x, y);

                        if (map[x, y] && !visited.Contains(candidate))
                        {
                            visited.Add(candidate);
                        }
                        else if (!map[x, y])
                        {
                            break;
                        }
                    }
                }
                else if (j == 3)
                {
                    //do with y == 40
                    Vector2 to = new Vector2Int(i, mapSize-1);
                    Vector2 line = to - from;
                    int paramLimit = (int)Mathf.Floor(line.magnitude);
                    line.Normalize();
                    if(i == 0)
                    {
                        continue;
                    }
                    for (int k = 1; k < paramLimit; k++)
                    {
                        if (paramLimit == mapSize - 1)
                        {
                            break;
                        }
                        Vector2 next = from + line * k;
                        int x = (int)Mathf.Round(next.x);
                        int y = (int)Mathf.Round(next.y);
                        Vector2Int candidate = new Vector2Int(x, y);

                        if (map[x, y] && !visited.Contains(candidate))
                        {
                            visited.Add(candidate);
                        }
                        else if (!map[x, y])
                        {
                            break;
                        }
                    }
                }
            }
        }
        totalScore += visited.Count;
        return visited.Count;
    }

    public void SwapWeapon(int unitID, List<int> enemies)
    {
        int highestThreatID = -1;
        float distanceToThreat = -1;
        bool enemyInSight = false;
        int rifleAmmo = api.GetReserveAmmunition(unitID, Weapon.Rifle) + api.GetMagazineAmmunition(unitID, Weapon.Rifle);
        int shotgunAmmo = api.GetReserveAmmunition(unitID, Weapon.Shotgun) + api.GetMagazineAmmunition(unitID, Weapon.Shotgun);
        int pistolAmmo = api.GetReserveAmmunition(unitID, Weapon.Pistol) + api.GetMagazineAmmunition(unitID, Weapon.Pistol);

        if (enemies.Count > 0)
        {
            highestThreatID = GetBestEnemy(unitID, enemies);
            enemyInSight = api.TargetInSight(unitID, highestThreatID);
            distanceToThreat = api.GetDistanceToUnit(unitID, highestThreatID);
        }
        if(distanceToThreat < 3 && distanceToThreat != -1)
        {
            api.SwapWeapon(unitID, Weapon.Knife);
        }
        else if(pistolAmmo > 0 && DecisionPistol(distanceToThreat, enemyInSight, shotgunAmmo, rifleAmmo))
        {
            if(api.GetWeapon(unitID) != Weapon.Pistol)
            {
                api.SwapWeapon(unitID, Weapon.Pistol);
            }
        }
        else if(shotgunAmmo > 0 && DecisionShotgun(distanceToThreat, api.GetMagazineAmmunition(unitID, Weapon.Shotgun),enemyInSight,api.IsUnitInZone(unitID),pistolAmmo,rifleAmmo))
        {
            if (api.GetWeapon(unitID) != Weapon.Shotgun)
            {
                api.SwapWeapon(unitID, Weapon.Shotgun);
            }
        }
        else if(rifleAmmo > 0 && DecisionRifle(distanceToThreat, pistolAmmo, shotgunAmmo))
        {
            if (api.GetWeapon(unitID) != Weapon.Rifle)
            {
                api.SwapWeapon(unitID, Weapon.Rifle);
            }
        }
        else if(rifleAmmo == 0 && pistolAmmo == 0 && shotgunAmmo == 0)
        {
            if (api.GetWeapon(unitID) != Weapon.Knife)
            {
                api.SwapWeapon(unitID, Weapon.Knife);
            }
        }

    }

    private bool DecisionPistol(float distanceToThreat, bool enemyInSight, int shotgunAmmo, int rifleAmmo)
    {
        //medium quarter, might change this cus we prioritize pistol instead of rifle atm
        if (distanceToThreat > 10 || distanceToThreat == -1)
        {
            //change if we cant shoot
            if (!enemyInSight)
            {
                return true;
            }
        }
        //no other weapon available
        if (shotgunAmmo == 0 && rifleAmmo == 0)
        {
            return true;
        }
        return false;
    }

    private bool DecisionShotgun(float distanceToThreat, float magAmmo, bool enemyInSight, bool isInZone, int pistolAmmo, int rifleAmmo)
    {
        //Close quarter
        if (distanceToThreat < 10 && distanceToThreat != -1)
        {
            //we can swap if we dont wanna shoot
            if (!enemyInSight)
            {
                return true;
            }
            //Change to shotgun if there is ammo in mag (saving reload time) instead of rifle
            if (pistolAmmo == 0 && magAmmo > 0)
            {
                return true;
            }
        }
        //no enemies, we in zone, lets get ready for close quarter
        if (isInZone && (distanceToThreat == -1 || !enemyInSight))
        {
            return true;
        }
        //no other weapon available
        if (pistolAmmo == 0 && rifleAmmo == 0)
        {
            return true;
        }
        return false;
    }
    private bool DecisionRifle(float distanceToThreat, int pistolAmmo, int shotgunAmmo)
    {
        //rifle for long range might be bad, pistol is bad
        if (distanceToThreat != -1 && distanceToThreat > 20)
        {
            return true;
        }
        //rifle instead of pistol when no more ammo
        if (pistolAmmo == 0 && (distanceToThreat == -1 || distanceToThreat > 10))
        {
            return true;
        }
        //no other weapon available
        if (pistolAmmo == 0 && shotgunAmmo == 0)
        {
            return true;
        }
        return false;
    }

    public int GetBestEnemy(int unitID, List<int> enemies)
    {
        int bestEnemy = 10000;
        int bestPathLenght = 10000;
        //Finding the closest unit at the moment
        foreach(int enemyID in enemies)
        {
            if(api.TargetInSight(unitID, enemyID))
            {
                return enemyID;
            }
            Vector2Int enemyGridPos = api.GetGridPos(enemyID);
            Vector2Int unitGridPos = api.GetGridPos(unitID);
            List<Node> pathToEnemy = FindPath(initialized_map[unitGridPos.x, unitGridPos.y], initialized_map[enemyGridPos.x, enemyGridPos.y]);
            if(pathToEnemy.Count < bestPathLenght)
            {
                bestEnemy = enemyID;
                bestPathLenght = pathToEnemy.Count;
            }
        }
        return bestEnemy;

    }

    public List<Node> FollowPath(int unitID, List<Node> path)
    {
        if (path.Count > 0)
        {
            MoveTowards(unitID, path[0].position);
            if (Vector2.Distance(api.GetWorldPosition(unitID), path[0].position) < 1.5f)
            {
                path.RemoveAt(0);
            }
        }
        return path;
    }

    public void MoveTowards(int unitID, Vector2 targetWorldPos)
    {
        float direction = api.AngleBetweenUnitWorldpos(unitID, targetWorldPos);
        api.Move(unitID, direction);
        api.LookAtDirection(unitID, direction);
    }

    public Node GetClosestTraversableNode(Node start, Node goal)
    {
        //Hittar den nod som ligger närmst start, bredvid goal, som är traversable
        Vector2Int nodedir = AngleToNode(Vector2.SignedAngle(Vector2.right, goal.position - start.position));
        return initialized_map[goal.x_grid + nodedir.x, goal.y_grid + nodedir.y];
    }

    public Vector2Int AngleToNode(float angle)
    {
        if(Mathf.Abs(angle) < 22.5f)
        {
            return new Vector2Int(1, 0);
        }
        else if(angle < 67.5f && angle > 0)
        {
            return new Vector2Int(1, 1);
        }
        else if(angle > -67.5f && angle < 0)
        {
            return new Vector2Int(1, -1);
        }
        else if(angle < 112.5f && angle > 0)
        {
            return new Vector2Int(0, 1);
        }
        else if(angle > -112.5f && angle < 0)
        {
            return new Vector2Int(0, -1);
        }
        else if(angle < 157.5f && angle > 0)
        {
            return new Vector2Int(-1, 1);
        }
        else if(angle > -157.5f && angle < 0)
        {
            return new Vector2Int(-1, -1);
        }
        else
        {
            return new Vector2Int(-1, 0);
        }
    }

    public List<Node> FindPath(Node start, Node goal)
    {
        bool shouldCover = false;
       
        if (!goal.traversable)
        {
            goal = GetClosestTraversableNode(start, goal);
        }
        List<Node> open = new List<Node>();
        List<Node> closed = new List<Node>();
        open.Add(start);

        while (open.Count > 0)
        {

            Node current = open[0];
            for (int i = 1; i < open.Count; i++)
            {
                if ((open[i].gScore + open[i].hScore) < (current.gScore + current.hScore) || (open[i].gScore + open[i].hScore) == (current.gScore + current.hScore) && open[i].hScore < current.hScore)
                {
                    current = open[i];
                }
            }
            open.Remove(current);
            closed.Add(current);
            if (Vector2.Distance(current.position, goal.position) < mapSize * 2.5f / 4)
            {
                shouldCover = true;
            }
            if (current.position == goal.position)
            {
                return CreatePath(start, goal);

            }
            foreach (Node neighbour in Neighbours(current))
            {
                if (!neighbour.traversable || closed.Contains(neighbour) || IsChoke(current, neighbour))
                {
                    continue;
                }
                float newCost = current.gScore + Heuristic(current, neighbour);
                if (shouldCover)
                {
                    newCost = current.gScore + Heuristic(current, neighbour) + (neighbour.visionScore - averageScore); // add heuristic instead of hscore!
                }
                if (newCost < neighbour.gScore || !open.Contains(neighbour))
                {
                    neighbour.gScore = newCost;
                    neighbour.hScore = Vector2.Distance(goal.position, neighbour.position)*20;// Distance (goal, neighbour);
                    neighbour.cameFrom = current;

                    if (!open.Contains(neighbour))
                    {
                        open.Add(neighbour);
                    }
                }
            }
        }
        return null;
    }

    public float Heuristic(Node from, Node to)
    {
        Vector2 diagonal = new Vector2(to.x_grid - from.x_grid, to.y_grid - from.y_grid);
        if(diagonal.x != 0 && diagonal.y != 0)
        {
            if(!initialized_map[from.x_grid, to.y_grid].traversable || !initialized_map[to.x_grid, from.y_grid].traversable)
            {
                return 1000;
            }
        }

        return 0;
    }

    public List<Node> CreatePath(Node start, Node end)
    {
        List<Node> path = new List<Node>();
        Node current = end;
        while (current != start && initialized_map[current.x_grid, current.y_grid].cameFrom != null)
        {
            path.Add(current);
            current = initialized_map[current.x_grid, current.y_grid].cameFrom;
        }
        path.Reverse();
        return path;
    }

    public List<Node> Neighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0)
                {
                    continue;
                }
                int adjacentX = (int)node.x_grid + i;
                int adjacentY = (int)node.y_grid + j;
                if (adjacentX >= 0 && adjacentX < mapSize && adjacentY >= 0 && adjacentY < mapSize)
                {
                    neighbours.Add(initialized_map[adjacentX, adjacentY]);
                }
            }
        }
        return neighbours;
    }

    public bool IsChoke(Node frompos, Node toPos)
    {
        Vector2 direction = new Vector2(toPos.x_grid - frompos.x_grid, toPos.y_grid - frompos.y_grid);
        if (direction.x != 0 && direction.y != 0)
        {
            if (!initialized_map[frompos.x_grid, toPos.y_grid].traversable && !initialized_map[toPos.x_grid, frompos.y_grid].traversable)
            {
                return true;
            }
        }
        return false;

    }
    

    void ShowPath(List<Node> path)
    {
        int i = 0;
        LineRenderer show = GetComponent<LineRenderer>();

        show.positionCount = path.Count;
        while (i < path.Count)
        {
            Vector3 position = new Vector3(path[i].position.x, path[i].position.y, -5f);
            show.SetPosition(i, position);
            i++;
        }
    }

    public class Node
    {
        public Vector2 position;
        public int x_grid;
        public int y_grid;

        public float gScore;
        public float hScore;
        public float visionScore;
    
        public Node cameFrom;

        public bool traversable;

        public Node(Vector2 worldPos, int gridX, int gridY, bool traverse)
        {
            this.position = worldPos;
            this.x_grid = gridX;
            this.y_grid = gridY;
            this.traversable = traverse;
        }
    }
}
