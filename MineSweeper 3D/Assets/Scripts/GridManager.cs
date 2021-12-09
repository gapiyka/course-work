using UnityEngine;

struct Tile
{
    public int counter;
    public GameObject gameObj;
}

public class GridManager : MonoBehaviour
{
    [SerializeField] private GameObject minePrefab;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Transform groundTransform;
    [SerializeField] private GameObject[] tilesPrefabs;

    private int gameDifficulty; // 3 levels of difficult
    private int gridSize; // range of tiles for axis
    private int mapSize; // axis * axis
    private float mapMiddle; // center of map (used to start point)
    private int mines—hance; // chance of generation mines on Map
    private int nMines; // number of mines at map
    private Tile[,] gridMatrix; // two-dimensional array to detect mines

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
        gridMatrix = new Tile[gridSize, gridSize];
        
        for (int x = 0; x < gridSize; x++)
        {
            for (int z = 0; z < gridSize; z++)
            {
                randomChance = UnityEngine.Random.Range(lowerBorder, upperBorder);
                if (randomChance < mines—hance)
                {
                    gridMatrix[x, z].gameObj = Instantiate(tilesPrefabs[9], new Vector3(x * tileSize + tileSize / 2, 0, z * tileSize + tileSize / 2f), Quaternion.identity, this.transform);
                    gridMatrix[x, z].counter = 9;
                    nMines++;
                }
            }
        }
        PaintMineArea();
        Debug.Log(nMines);
    }

    //should be optimized
    void PaintMineArea()
    {
        for (int x = 0; x < gridSize; x++)
        {
            for (int z = 0; z < gridSize; z++)
            {
                int n = 0;
                if (gridMatrix[x, z].counter == 9) continue;
                if(x != 0)
                {
                    if (gridMatrix[x - 1, z].counter == 9) n++;
                }
                if (z != 0)
                {
                    if (gridMatrix[x, z - 1].counter == 9) n++;
                }
                if (x != 0 && z != 0)
                {
                    if (gridMatrix[x - 1, z - 1].counter == 9) n++;
                }
                if (x != gridSize - 1)
                {
                    if (gridMatrix[x + 1, z].counter == 9) n++;
                }
                if (z != gridSize - 1)
                {
                    if (gridMatrix[x, z + 1].counter == 9) n++;
                }
                if (x != gridSize - 1 && z != gridSize - 1)
                {
                    if (gridMatrix[x + 1, z + 1].counter == 9) n++;
                }
                if (x != 0 && z != gridSize - 1)
                {
                    if (gridMatrix[x - 1, z + 1].counter == 9) n++;
                }
                if (x != gridSize - 1 && z !=0)
                {
                    if (gridMatrix[x + 1, z - 1].counter == 9) n++;
                }

                gridMatrix[x, z].gameObj = Instantiate(tilesPrefabs[n], new Vector3(x * tileSize + tileSize / 2, 0f, z * tileSize + tileSize / 2f), Quaternion.identity, this.transform); ;
            }
        }
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
