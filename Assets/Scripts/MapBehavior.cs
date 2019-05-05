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
    public bool[,] traversable;

    // Start is called before the first frame update
    void Start()
    {
        InitEditor();
        traversable = new bool[mapSize,mapSize];
        mapSize = Random.Range(mapMinSize, mapMaxSize);
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
            Instantiate(wall, new Vector3(i * 2.5f, -1 * 2.5f, 0), wall.transform.rotation, Walls.transform);
            Instantiate(wall, new Vector3(-1 * 2.5f, i * 2.5f, 0), wall.transform.rotation, Walls.transform);
            Instantiate(wall, new Vector3(i * 2.5f, mapSize * 2.5f, 0), wall.transform.rotation, Walls.transform);
            Instantiate(wall, new Vector3(mapSize * 2.5f, i * 2.5f, 0), wall.transform.rotation, Walls.transform);
        }
    }

    void GenerateObstacles()
    {
        for (int x = 0; x < mapSize; x++)
        {
            for (int y = 0; y < mapSize; y++)
            {
                if (Random.Range(0, 100) < obstaclesPercentage * 100)
                {
                    Instantiate(stone, new Vector3(x * 2.5f, y * 2.5f, 0), stone.transform.rotation, Obstacles.transform);
                }
            }
        }
    }

}
