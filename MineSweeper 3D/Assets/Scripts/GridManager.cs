using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] private GameObject minePrefab;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Transform groundTransform;

    private int gameDifficulty; // 3 levels of difficult
    private int gridSize; // range of tiles for axis
    private int mapSize; // axis * axis
    private float mapMiddle; // center of map (used to start point)
    private int mines—hance; // chance of generation mines on Map
    private int nMines; // number of mines at map
    private GameObject[,] gridMatrix; // two-dimensional array to detect mines

    private const int sizeMultiplier = 9; // multipler for payable size
    private const int minesMultiplier = 20; // diviner to count n of mines
    private const int tileSize = 5; // size of plate from plane object scale

    void Start()
    {
        // should take parameter `gameDifficulty` from settings
        ReloadMap(1);
    }

    void ReloadMap(int difficulty)
    {
        gameDifficulty = difficulty;

        gridSize = gameDifficulty * sizeMultiplier;
        mapSize = gridSize * gridSize;
        mapMiddle = gridSize / 2f;

        playerTransform.position = GetStartPos(1.1f);
        groundTransform.position = GetStartPos(0f);
        groundTransform.localScale = new Vector3(mapMiddle, 1f, mapMiddle);

        GenerateGrid();
    }

    // generation grid with mines
    void GenerateGrid()
    {
        const int lowerBorder = 0;
        const int upperBorder = 100;
        int randomChance;

        nMines = 0;
        mines—hance = gameDifficulty * minesMultiplier;
        gridMatrix = new GameObject[gridSize, gridSize];
        
        for (int x = 0; x < gridSize; x++)
        {
            for (int z = 0; z < gridSize; z++)
            {
                randomChance = UnityEngine.Random.Range(lowerBorder, upperBorder);
                if (randomChance < mines—hance)
                {
                    gridMatrix[x, z] = Instantiate(minePrefab, new Vector3(x * tileSize + tileSize / 2, 0, z * tileSize + tileSize / 2f), Quaternion.identity, this.transform);
                    nMines++;
                }
            }
        }

        Debug.Log(nMines);
    }

    Vector3 GetStartPos(float height)
    {
        return new Vector3(tileSize * mapMiddle, height, tileSize * mapMiddle);
    }

    public int GetMinesNumber()
    {
        return nMines;
    }
}
