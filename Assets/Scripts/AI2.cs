﻿using System.Collections;
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
            unit1Path = FindPath(initialized_map[api.GetGridPos(idPlayer1).x, api.GetGridPos(idPlayer1).y], initialized_map[api.GetGridPosFromWorldPos(api.GetZonePosition()).x, api.GetGridPosFromWorldPos(api.GetZonePosition()).y]);
            oldZone = api.GetZonePosition();
            if (debugPath)
            {
                ShowPath(unit1Path);
            }
            if (api.GetWeapon(idPlayer1) == Weapon.Knife)
            {
                Debug.Log("jhhusfsdj");
                if (api.WeaponSwapCooldown(idPlayer1) == 0)
                {
                    api.SwapWeapon(idPlayer1, Weapon.Pistol);
                    Debug.Log(api.GetWeapon(idPlayer1));
                    api.ReloadWeapon(idPlayer1);
                }
            }
        }
        
        List<int> enemies = api.SenseNearbyByTeam(idPlayer1, -myTeamId);
        if (enemies.Count > 0 && api.TargetInSight(idPlayer1, enemies[0]))
        {
            api.LookAtDirection(idPlayer1, api.AngleBetweenUnits(idPlayer1, enemies[0]));
            if (!api.CanFire(idPlayer1) && once)
            {
                api.ReloadWeapon(idPlayer1);
                once = false;
                //do some awesome evade tactic
            }
            else
            {
                api.Attack(idPlayer1);
            }
        }
        if(api.GetMagazineAmmunition(idPlayer1, Weapon.Pistol) == 0)
        {
            once = true;
        }
        else
        {
            unit1Path = FollowPath(idPlayer1, unit1Path);
        }
        /*

        foreach (int unitId in myUnits)
        {

            //Execute your code

        }
        */
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
            }
        }
        //Just some tests of A*:::
        Vector2Int aUnitPos = api.GetGridPos(myUnits[0]);
        Node testnode1 = initialized_map[aUnitPos.x, aUnitPos.y];
        Node testnode2 = initialized_map[0, 0];
 
        unit1Path = FindPath(testnode1, testnode2);


        if (debugPath)
        {
            ShowPath(unit1Path);
        }
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
                float newCost = current.gScore + Heuristic(current, neighbour); // add heuristic instead of hscore!
                if (newCost < neighbour.gScore || !open.Contains(neighbour))
                {
                    neighbour.gScore = newCost;
                    neighbour.hScore = Vector2.Distance(goal.position, neighbour.position);// Distance (goal, neighbour);
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
                return 10;
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
