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
    
    bool[,] traversable;
    List<Vector2> obstaclePositions;


    // Start is called before the first frame update
    void Start()
    {
        mapSize = Random.Range(mapMinSize, mapMaxSize);
        obstaclePositions = new List<Vector2>();
        InitEditor();
        traversable = new bool[mapSize, mapSize];
        GenerateMap();
        GenerateWalls();
        GenerateObstacles();
    }

    void InitEditor()
    {
        Ground = Instantiate(new GameObject(), this.transform);
        Ground.name = "Ground";
        Obstacles = Instantiate(new GameObject(), this.transform);
        Obstacles.name = "Obstacles";
        Walls = Instantiate(new GameObject(), this.transform);
        Walls.name = "Walls";
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

    public Vector2 GetGridPosFromWorldPos(Vector3 worldPos)
    {
        return new Vector2(Mathf.Round(worldPos.x / 2.5f), Mathf.Round(worldPos.y / 2.5f));
    }

    public Vector2 GetWorldPosFromGridPos(int x, int y)
    {
        return new Vector2(Mathf.Round(x * 2.5f), Mathf.Round(y * 2.5f));
    }

    public bool IsGridPosTraversable(int x, int y)
    {
        return traversable[x, y];
    }

    public bool IsWorldPosTraversable(Vector3 worldPos)
    {
        return traversable[(int)Mathf.Round(worldPos.x / 2.5f), (int)Mathf.Round(worldPos.y / 2.5f)];
    }

    public bool[,] GetTraversable()
    {
        return traversable;
    }

}
