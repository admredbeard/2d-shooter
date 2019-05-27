using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapBehavior : MonoBehaviour
{
    public List<GameObject> groundClutter;
    public GameObject wall;
    public GameObject stone;
    public GameObject grass;

    public float clutterPercentage;
    public float obstaclesPercentage;
    public int mapMinSize;
    public int mapMaxSize;

    int mapSize;
    GameObject Map;
    GameObject Ground;
    GameObject Obstacles;
    GameObject Walls;

    [System.NonSerialized]
    public Vector2 zonePos;
    [System.NonSerialized]
    public float zoneRadius;
    
    bool[,] traversable;

    GameObject gameController;
    GameController gc;

    // Start is called before the first frame update
    void Start()
    {
        zoneRadius = 0;
        zonePos = Vector2.zero;
        gc = GameObject.Find("GameController").GetComponent<GameController>();
        mapSize = Random.Range(mapMinSize, mapMaxSize);
        InitEditor();
        traversable = new bool[mapSize, mapSize];
        GenerateMap();
        GenerateWalls();
        GenerateObstacles();
    }

    void InitEditor()
    {
        GameObject temp = new GameObject();
        Ground = Instantiate(temp, this.transform);
        Ground.name = "Ground";
        Obstacles = Instantiate(temp, this.transform);
        Obstacles.name = "Obstacles";
        Walls = Instantiate(temp, this.transform);
        Walls.name = "Walls";
        Destroy(temp);
    }

    void GenerateMap()
    {
        GameObject clutter;
        for (int x = 0; x < mapSize; x++)
        {
            for (int y = 0; y < mapSize; y++)
            {
                if (Random.Range(0, 100) < clutterPercentage * 100)
                {
                    clutter = groundClutter[Random.Range(0, groundClutter.Count)];
                    Instantiate(clutter, new Vector3(x * 2.5f, y * 2.5f, 0), clutter.transform.rotation, Ground.transform);
                }
                else
                {
                    Instantiate(grass, new Vector3(x * 2.5f, y * 2.5f, 0), grass.transform.rotation, Ground.transform);
                }
                traversable[x, y] = true;
            }
        }
    }

    void GenerateWalls()
    {
        for (int i = -1; i < mapSize+1; i++)
        {
            Instantiate(wall, new Vector2(i * 2.5f, -1 * 2.5f), wall.transform.rotation, Walls.transform);
            Instantiate(wall, new Vector2(-1 * 2.5f, i * 2.5f), wall.transform.rotation, Walls.transform);
            Instantiate(wall, new Vector2(i * 2.5f, mapSize * 2.5f), wall.transform.rotation, Walls.transform);
            Instantiate(wall, new Vector2(mapSize * 2.5f, i * 2.5f), wall.transform.rotation, Walls.transform);
        }
    }

    void GenerateObstacles()
    {
        for (int x = 1; x < mapSize-1; x++)
        {
            for (int y = 1; y < mapSize-1; y++)
            {
                if (Random.Range(0, 100) < obstaclesPercentage * 100)
                {
                    Instantiate(stone, new Vector3(x * 2.5f, y * 2.5f, 0), stone.transform.rotation, Obstacles.transform);
                    traversable[x, y] = false;
                }
                else
                {
                    traversable[x, y] = true;
                }
            }
        }
    }

    // Help functions for the API calls

    public int GetMapSize()
    {
        return mapSize;
    }

    public Vector2 GetMapMiddle()
    {
        return new Vector2(mapSize * 2.5f / 2, mapSize * 2.5f / 2);
    }

    public Vector2Int GetGridPosFromWorldPos(Vector2 worldPos)
    {
        return new Vector2Int((int)Mathf.Round(worldPos.x / 2.5f), (int)Mathf.Round(worldPos.y / 2.5f));
    }

    public Vector2 GetWorldPosFromGridPos(int x, int y)
    {
        return new Vector2((float) x * 2.5f,(float) y * 2.5f);
    }

    public Vector2 GetWorldPosFromGridPos(Vector2Int gridIndex)
    {
        return new Vector2((float)gridIndex.x * 2.5f, (float)gridIndex.y * 2.5f);
    }

    public bool IsGridPosTraversable(int x, int y)
    {
        return traversable[x, y];
    }

    public bool IsGridPosTraversable(Vector2Int v)
    {
        return traversable[v.x, v.y];
    }

    public bool IsWorldPosTraversable(Vector2 worldPos)
    {
        return traversable[(int)Mathf.Round(worldPos.x / 2.5f), (int)Mathf.Round(worldPos.y / 2.5f)];
    }

    public bool[,] GetTraversable()
    {
        return traversable;
    }

    public Vector2 GetWorldPos(int unidId)
    {
        return new Vector2(gc.GetPlayers()[unidId].transform.position.x, gc.GetPlayers()[unidId].transform.position.y);
    }

    public Vector2Int GetGridPos(int unitId)
    {
        return GetGridPosFromWorldPos(new Vector2(gc.GetPlayers()[unitId].transform.position.x, gc.GetPlayers()[unitId].transform.position.y));
    }

    public float DistanceToUnit(int fromId, int toId)
    {
        return Vector2.Distance(GetWorldPos(fromId), GetWorldPos(toId));
    }

    public float DistanceToWorldPos(int unitId, Vector2 worldPos)
    {
        return Vector2.Distance(GetWorldPos(unitId), worldPos);
    }

    public float DistanceToGridPos(int unitId, Vector2Int gridIndex)
    {
        return Vector2.Distance(GetWorldPos(unitId), GetWorldPosFromGridPos(gridIndex));
    }

    public bool TargetInSight(int unitId, int targetId)
    {
        Vector2 targetPos = new Vector2(gc.GetPlayers()[targetId].transform.position.x, gc.GetPlayers()[targetId].transform.position.y);
        return FreeLineOfSight2D(unitId, targetPos);
    }

    public bool WorldPositionInSight(int unitId, Vector2 worldPosition)
    {
        return FreeLineOfSight2D(unitId, worldPosition);
    }

    public bool GridPositionInSight(int unitId, Vector2Int gridPosition)
    {
        return FreeLineOfSight2D(unitId, GetWorldPosFromGridPos(gridPosition.x,gridPosition.y));
    }

    public bool IsUnitInZone(int unitId)
    {
        return Vector2.Distance(GetWorldPos(unitId), zonePos) < zoneRadius;
    }

    public bool IsWorldPosInZone(Vector2 worldPos)
    {
        return Vector2.Distance(worldPos, zonePos) < zoneRadius;
    }

    public bool IsGridPosInZone(Vector2Int gridPos)
    {
        return Vector2.Distance(GetWorldPosFromGridPos(gridPos), zonePos) < zoneRadius;
    }

    public Vector2 ZoneCenter()
    {
        return zonePos;
    }

    public float ZoneRadius()
    {
        return zoneRadius;
    }

    bool FreeLineOfSight2D(int unitID, Vector2 targetPos)
    {
        Vector2 unitPos = new Vector2(gc.GetPlayers()[unitID].transform.position.x, gc.GetPlayers()[unitID].transform.position.y);
        RaycastHit2D[] lineOfSightObjects = Physics2D.RaycastAll(unitPos, targetPos - unitPos, Vector2.Distance(unitPos, targetPos));

        if (lineOfSightObjects.Length < 3)
        {
            return true;
        }

        for (int i = 0; i < lineOfSightObjects.Length; i++)
        {
            if (lineOfSightObjects[i].transform.name == gc.GetPlayers()[unitID].transform.name)
            {
                continue;
            }
            else
            {
                return false;
            }
        }
        return true;
    }

    public float AngleBetweenUnits(int unitId, int targetId)
    {
        Vector2 begin = GetWorldPos(unitId);
        Vector2 end = GetWorldPos(targetId);
        return Vector2.SignedAngle(Vector2.right, end - begin);
    }

    public float AngleBetweenUnitWorldpos(int unitId, Vector2 targetPos)
    {
        Vector2 begin = GetWorldPos(unitId);
        return Vector2.SignedAngle(Vector2.right, targetPos - begin);
    }

    public float AngleBetweenUnitGridpos(int unitId, Vector2Int targetPos)
    {
        Vector2 begin = GetWorldPos(unitId);
        Vector2 end = GetWorldPosFromGridPos(targetPos);
        return Vector2.SignedAngle(Vector2.right, end - begin);
    }

}
