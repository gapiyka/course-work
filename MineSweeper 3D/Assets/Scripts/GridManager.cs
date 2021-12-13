using UnityEngine;

public struct Tile
{
    public GameObject gameObj;
    public int minesCounter;
    public bool IsChecked;
    public bool IsFlag;
}

public class GridManager : MonoBehaviour
{
    [SerializeField] private GameObject flagPrefab;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Transform groundTransform;
    [SerializeField] private GameObject[] tilesPrefabs;

    private int gameDifficulty; // 3 levels of difficult
    private int gridSize; // range of tiles for axis
    private int mapSize; // axis * axis
    private float mapMiddle; // center of map (used to start point)
    private int mines—hance; // chance of generation mines on Map
    private int nMines; // number of mines at map

    private const int sizeMultiplier = 9; // multipler for payable size
    private const int minesMultiplier = 20; // diviner to count n of mines
    private const int tileSize = 5; // size of plate from plane object scale

    public Tile[,] gridMatrix; // two-dimensional array to detect mines

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

        playerTransform.position = GetStartPos(1.1f); // spawn palyer at middle of map

        GenerateGrid();
    }

    // generation grid with mines
    void GenerateGrid()
    {
        const int randomLowerBorder = 0;
        const int randomUpperBorder = 100;
        int randomChance;

        nMines = 0;
        mines—hance = gameDifficulty * minesMultiplier;
        gridMatrix = new Tile[gridSize, gridSize];
        
        for (int x = 0; x < gridSize; x++)
        {
            for (int z = 0; z < gridSize; z++)
            {
                randomChance = UnityEngine.Random.Range(randomLowerBorder, randomUpperBorder);
                if (randomChance < mines—hance)
                {
                    gridMatrix[x, z].gameObj = Instantiate(tilesPrefabs[9], new Vector3(x * tileSize + tileSize / 2, 0, z * tileSize + tileSize / 2f), Quaternion.identity, this.transform);
                    gridMatrix[x, z].minesCounter = 9;
                    nMines++;
                }
                gridMatrix[x, z].IsChecked = false; // set tile not checked
                gridMatrix[x, z].IsFlag = false; // set tile without flag
            }
        }
        PaintMineArea();
    }

    //should be optimized
    void PaintMineArea()
    {
        for (int x = 0; x < gridSize; x++)
        {
            for (int z = 0; z < gridSize; z++)
            {
                int n = 0;
                if (gridMatrix[x, z].minesCounter == 9) continue;
                if(x != 0)
                {
                    if (gridMatrix[x - 1, z].minesCounter == 9) n++;
                }
                if (z != 0)
                {
                    if (gridMatrix[x, z - 1].minesCounter == 9) n++;
                }
                if (x != 0 && z != 0)
                {
                    if (gridMatrix[x - 1, z - 1].minesCounter == 9) n++;
                }
                if (x != gridSize - 1)
                {
                    if (gridMatrix[x + 1, z].minesCounter == 9) n++;
                }
                if (z != gridSize - 1)
                {
                    if (gridMatrix[x, z + 1].minesCounter == 9) n++;
                }
                if (x != gridSize - 1 && z != gridSize - 1)
                {
                    if (gridMatrix[x + 1, z + 1].minesCounter == 9) n++;
                }
                if (x != 0 && z != gridSize - 1)
                {
                    if (gridMatrix[x - 1, z + 1].minesCounter == 9) n++;
                }
                if (x != gridSize - 1 && z !=0)
                {
                    if (gridMatrix[x + 1, z - 1].minesCounter == 9) n++;
                }

                gridMatrix[x, z].gameObj = Instantiate(tilesPrefabs[n], new Vector3(x * tileSize + tileSize / 2, 0f, z * tileSize + tileSize / 2f), Quaternion.identity, this.transform); ;
            }
        }
    }

    Vector3 GetStartPos(float height)
    {
        return new Vector3(tileSize * mapMiddle, height, tileSize * mapMiddle);
    }

    void SwitchTileState(Tile tile, int button, int xInArray, int zInArray)
    {
        bool IsAlreadyChecked = tile.IsChecked;
        bool IsAlreadyFlag = tile.IsFlag;
        Transform CheckerTransform = tile.gameObj.transform.GetChild(0);
        Transform BodyTransform = tile.gameObj.transform.GetChild(1);

        if (!IsAlreadyChecked)
        {
            if (button == 1 && !IsAlreadyFlag)
            {
                CheckerTransform.gameObject.SetActive(false);
                BodyTransform.gameObject.SetActive(true);
                gridMatrix[xInArray, zInArray].IsChecked = true;
            }

            if (button == 2)
            {
                if (IsAlreadyFlag)
                {
                    Destroy(CheckerTransform.Find("Flag(Clone)").gameObject);// can be changed on SetActive
                    gridMatrix[xInArray, zInArray].IsFlag = false;
                }
                else
                {
                    Instantiate(flagPrefab, CheckerTransform.position, Quaternion.identity, CheckerTransform);
                    gridMatrix[xInArray, zInArray].IsFlag = true;
                }
            }
        }   
    }

    public void FindPointedTile(GameObject obj, int button)
    {
        for (int x = 0; x < gridMatrix.GetLength(0); x++)
        {
            for (int z = 0; z < gridMatrix.GetLength(1); z++)
            {
                Tile tile = gridMatrix[x, z];
                if (tile.gameObj.transform.position == obj.transform.position) SwitchTileState(tile, button, x, z);
            }
        }   
    }
}
